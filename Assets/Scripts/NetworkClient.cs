using UnityEngine;
using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Photon.Pun;
using UnityEngine.Events;
using System.IO;
using System.Text;

public partial class NetworkClient : ThingWithAvatarHiarchy
{
    private bool calibrated = false;
    private string input_list;

    [SerializeField]
    Transform hmd, l_controller, r_controller, l_tracker, r_tracker; // 트래커 추가
    float timeAccumulator = 0.0f;
    float fixedDeltaTime = 1.0f / 24.0f;
    private Quaternion hmdRotOffset;
    private Matrix4x4 initial_global_matrix_l_M;
    private Matrix4x4 initial_global_matrix_r_M;
    private Matrix4x4 initial_wrist_matrix_l_W;
    private Matrix4x4 initial_wrist_matrix_r_W;
    [SerializeField] private int[] constraint_joints;
    [SerializeField] private float constraint_weight;
    [SerializeField] private float constraint_pelvis_weight = 0.7f;
    float prev_frame_pelv_pos;
    private Matrix4x4[] _prev_joints = new Matrix4x4[22];
    [SerializeField] float twimDistance = 0.2f;
    private bool do_after = false;
    [SerializeField] float hmd_zaxis_offset = 0.05f;
    [SerializeField] float l_controller_xaxis_offset = 0.15f;
    [SerializeField] float r_controller_xaxis_offset = 0.15f;
    [SerializeField] bool draw_input_gizmos = true;

    public CCDIK ccdikLeftFoot; // 왼발 CCDIK
    public CCDIK ccdikRightFoot; // 오른발 CCDIK
    public Transform leftFootTarget; // 왼발 타겟
    public Transform rightFootTarget; // 오른발 타겟

    public float footRotationWeight = 1f; // 발 회전 가중치

    private Quaternion leftFootInitialRotation; // 왼발 초기 로컬 회전값
    private Quaternion rightFootInitialRotation; // 오른발 초기 로컬 회전값

    private Vector3[] previousFrameJointsPositions = new Vector3[22];

    int window_size = 41;
    Queue<ViveTriplet> frames = new Queue<ViveTriplet>();
    ViveTriplet frame_t, frame_t1;
    List<Matrix4x4> prev_input;

    public PhotonView photonView;
    public InputManager inputManager;
    public ModelIntegrationManager modelIntegrationManager;
    public UnityAction OnCompensatedFixedIntervalElapsed;
    public const string DUMMY_RESPONSE = "dummy";

    void Start()
    {
        if (!photonView.IsMine) return;
        modelIntegrationManager = ModelIntegrationManager.Instance;
        inputManager = InputManager.Instance;
        inputManager.OnPinch += Calibrate;
        OnCompensatedFixedIntervalElapsed += DoOnCompensatedFixedIntervalElapsed;

        do_after = false;
        constraint_joints = new int[] { 3, 1, 2, 4, 5, 7, 8, 10, 11, 13, 14, 16, 17 };
        constraint_weight = 0.2f;

        ccdikLeftFoot = GetComponent<CCDIK>();
        ccdikRightFoot = GetComponent<CCDIK>();

        leftFootInitialRotation = leftFootTarget.localRotation;
        rightFootInitialRotation = rightFootTarget.localRotation;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (IsCCDIKAllSet)
            ApplyFootTrackerIK();
        CheckFixedIntervalAndDo();
    }

    bool IsCCDIKAllSet
        => ccdikLeftFoot != null && ccdikRightFoot != null && leftFootTarget != null && rightFootTarget != null;
    void ApplyFootTrackerIK()
    {
        UpdateFootPos(ccdikLeftFoot, leftFootTarget);           // 왼발의 위치 업데이트
        UpdateFootPos(ccdikRightFoot, rightFootTarget);         // 오른발의 위치 업데이트

        leftFootTarget.position -= new Vector3(0, 0.05f, 0);    // 왼발의 위치 보정
        rightFootTarget.position -= new Vector3(0, 0.05f, 0);   // 오른발의 위치 보정

        leftFootTarget.localRotation                            // 왼발의 회전 업데이트
            = Quaternion.Lerp(leftFootInitialRotation, leftFootTarget.localRotation, footRotationWeight);
        rightFootTarget.localRotation                           // 오른발의 회전 업데이트
            = Quaternion.Lerp(rightFootInitialRotation, rightFootTarget.localRotation, footRotationWeight);
    }
    void UpdateFootPos(CCDIK foot, Transform target)
    {
        foot.solver.IKPosition = target.position;
        foot.solver.target = target;
        foot.solver.Update();
    }

    void CheckFixedIntervalAndDo()
    {
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator < fixedDeltaTime) return;
        OnCompensatedFixedIntervalElapsed.Invoke();
        timeAccumulator -= fixedDeltaTime;
    }

    void DoOnCompensatedFixedIntervalElapsed()
    {
        if (!calibrated) return;
        UpdateTrackersOffset();
        CollectFrame();
        LinearSmoothing();
        input_list = GenerateInput();

        if (frames.Count < window_size) return;
        if (draw_input_gizmos)
        {
            DrawInputTransform(hmd.position, hmd.rotation);
            DrawInputTransform(l_controller.position, l_controller.rotation);
            DrawInputTransform(r_controller.position, r_controller.rotation);
            // 트래커에도 좌표축 그리기
            DrawInputTransform(l_tracker.position, l_tracker.rotation);
            DrawInputTransform(r_tracker.position, r_tracker.rotation);
        }

        PredictLowerPose();
    }

    // 조건1 : hmd와 controller 사이의 거리
    // 조건2 : controller의 현재 프레임과 전 프레임 사이 거리
    bool CheckControllerDistance(Transform controller)
        => Vector3.SqrMagnitude(controller.position - hmd.position) > 1.5f;
    void LinearSmoothing()
    {
        if (CheckControllerDistance(l_controller))
            l_controller.position
                = Vector3.Lerp(frame_t1.Item2.GetPosition(), frame_t.Item2.GetPosition(), 0.01f);
        if (CheckControllerDistance(r_controller))
            r_controller.position
                = Vector3.Lerp(frame_t1.Item3.GetPosition(), frame_t.Item3.GetPosition(), 0.01f);
    }

    bool TwimCheck(float current_pelv_y_pos)
    {
        if (Mathf.Abs(current_pelv_y_pos - prev_frame_pelv_pos) <= twimDistance)
            return false;
        Debug.LogWarning("Twim");
        return true;
    }

    void Calibrate()
    {
        if (calibrated) return;
        Debug.Log("Calibration Start");
        CalibrateHeight();
        CalibrateTrackers();
        calibrated = true;
    }

    void CalibrateHeight()
    {
        //// Calibrate Height
        Debug.Log("height is : " + hmd.position.y * 100);
        float newScale = hmd.position.y / _jointTransforms[15].position.y;
        _jointTransforms[0].localScale *= newScale;
    }

    void CalibrateTrackers()
    {
        //// HMD, LR Controllers position offset
        // CalibrateHeight();

        //// HMD rotation offset
        hmdRotOffset = _jointTransforms[15].rotation * Quaternion.Inverse(hmd.rotation);

        //// Initial XYZ global axis of controller
        Vector3 l_controller_x = l_controller.right;
        Vector3 l_controller_y = l_controller.up;
        Vector3 l_controller_z = l_controller.forward;

        Vector3 r_controller_x = r_controller.right;
        Vector3 r_controller_y = r_controller.up;
        Vector3 r_controller_z = r_controller.forward;

        //// Initial XYZ global axis of wrist
        Vector3 l_wrist_x = _jointTransforms[20].right;
        Vector3 l_wrist_y = _jointTransforms[20].up;
        Vector3 l_wrist_z = _jointTransforms[20].forward;

        Vector3 r_wrist_x = _jointTransforms[21].right;
        Vector3 r_wrist_y = _jointTransforms[21].up;
        Vector3 r_wrist_z = _jointTransforms[21].forward;

        //// Make it to homogenous matrix
        initial_global_matrix_l_M = Matrix4x4.identity;
        initial_global_matrix_l_M.SetColumn(0, new Vector4(l_controller_x.x, l_controller_x.y, l_controller_x.z, 0));
        initial_global_matrix_l_M.SetColumn(1, new Vector4(l_controller_y.x, l_controller_y.y, l_controller_y.z, 0));
        initial_global_matrix_l_M.SetColumn(2, new Vector4(l_controller_z.x, l_controller_z.y, l_controller_z.z, 0));

        initial_global_matrix_r_M = Matrix4x4.identity;
        initial_global_matrix_r_M.SetColumn(0, new Vector4(r_controller_x.x, r_controller_x.y, r_controller_x.z, 0));
        initial_global_matrix_r_M.SetColumn(1, new Vector4(r_controller_y.x, r_controller_y.y, r_controller_y.z, 0));
        initial_global_matrix_r_M.SetColumn(2, new Vector4(r_controller_z.x, r_controller_z.y, r_controller_z.z, 0));

        initial_wrist_matrix_l_W = Matrix4x4.identity;
        initial_wrist_matrix_l_W.SetColumn(0, new Vector4(l_wrist_x.x, l_wrist_x.y, l_wrist_x.z, 0));
        initial_wrist_matrix_l_W.SetColumn(1, new Vector4(l_wrist_y.x, l_wrist_y.y, l_wrist_y.z, 0));
        initial_wrist_matrix_l_W.SetColumn(2, new Vector4(l_wrist_z.x, l_wrist_z.y, l_wrist_z.z, 0));

        initial_wrist_matrix_r_W = Matrix4x4.identity;
        initial_wrist_matrix_r_W.SetColumn(0, new Vector4(r_wrist_x.x, r_wrist_x.y, r_wrist_x.z, 0));
        initial_wrist_matrix_r_W.SetColumn(1, new Vector4(r_wrist_y.x, r_wrist_y.y, r_wrist_y.z, 0));
        initial_wrist_matrix_r_W.SetColumn(2, new Vector4(r_wrist_z.x, r_wrist_z.y, r_wrist_z.z, 0));
    }

    void UpdateTrackersOffset()
    {
        /// Post-Multiplication
        Matrix4x4 l_matrix = Matrix4x4.TRS(l_controller.localPosition, l_controller.localRotation, Vector3.one) * initial_global_matrix_l_M.transpose * initial_wrist_matrix_l_W;
        Matrix4x4 r_matrix = Matrix4x4.TRS(r_controller.localPosition, r_controller.localRotation, Vector3.one) * initial_global_matrix_r_M.transpose * initial_wrist_matrix_r_W;

        hmd.localRotation = hmdRotOffset * hmd.localRotation;

        l_controller.transform.localRotation = l_matrix.rotation;
        r_controller.transform.localRotation = r_matrix.rotation;

        hmd.position += hmd.forward * hmd_zaxis_offset;
        l_controller.position += l_controller.right * l_controller_xaxis_offset;
        r_controller.position += -r_controller.right * r_controller_xaxis_offset;

        // Rotate the trackers to align with other devices
        l_tracker.Rotate(Vector3.right, 90); // Rotate around x axis
        r_tracker.Rotate(Vector3.right, 90); // Rotate around x axis
    }

    void CollectFrame()
    {
        if(frames.Count > window_size)
            frames.Dequeue();
        frame_t1 = frame_t;
        frame_t = new ViveTriplet(
            hmd.localToWorldMatrix,
            l_controller.localToWorldMatrix,
            r_controller.localToWorldMatrix
        );
        frames.Enqueue(frame_t);
    }
    public string GenerateInput() => SerializeLastFrame();

    void PredictLowerPose()
    {
        modelIntegrationManager.SendFrame(input_list);
        string response = modelIntegrationManager.ReceiveFrame();
        if(response == DUMMY_RESPONSE)
        {
            Debug.Log(response);
            return;
        }
        
        ManipulateCharacter(response);

        try
        {
            string[] stringArray = response.Split(',');
            float[] floatArray = new float[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
                if (float.TryParse(stringArray[i].Trim('[', ']', '"').Trim(), out float result))
                    floatArray[i] = result;


            if (do_after && TwimCheck(floatArray[1]))
            {
                _jointTransforms[0].localPosition = _prev_joints[0].GetPosition();

                for (int j = 0; j < 22; j++)
                    if (_jointTransforms[j] != null) // end effector joint may be null
                        _jointTransforms[j].transform.localRotation = _prev_joints[j].rotation;
            }
            else
            {
                _jointTransforms[0].localPosition = new Vector3(floatArray[0], floatArray[1], floatArray[2]);

                for (int j = 0; j < 22; j++)
                {
                    if (_jointTransforms[j] != null) // end effector joint may be null
                    {

                        Vector3 upward = new Vector3(floatArray[9 * j + 1 + 3], floatArray[9 * j + 4 + 3], floatArray[9 * j + 7 + 3]);
                        Vector3 forward = new Vector3(floatArray[9 * j + 2 + 3], floatArray[9 * j + 5 + 3], floatArray[9 * j + 8 + 3]);
                        _jointTransforms[j].transform.localRotation = Quaternion.LookRotation(forward, upward);

                        _prev_joints[j] = Matrix4x4.TRS(_jointTransforms[j].localPosition, _jointTransforms[j].localRotation, Vector3.one);
                        prev_frame_pelv_pos = _jointTransforms[0].localPosition.y;
                        do_after = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("JSON parsing error: " + e.Message);
        }
    }

    //public string SerializeLastFrame() => JsonUtility.ToJson(frame_t.ConvertToSerializable());
    public string SerializeLastFrame()
    {
        ViveTripletSerializable vts = frame_t.ConvertToSerializable();
        if (!IntegrityTest(frame_t, vts))
            Debug.Log("Incorrect Conversion");
        return JsonUtility.ToJson(vts);
    }

    bool IntegrityTest(ViveTriplet vt, ViveTripletSerializable vts)
    {
        Matrix4x4[] m = { vt.Item1, vt.Item2, vt.Item3 };
        Matrix4x4Serializable[] ms = { vts.Item1, vts.Item2, vts.Item3 };
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 16; ++j)
                if (m[i][j] != ms[i].At(j))
                    return false;
        return true;
    }

    public void DrawInputTransform(Vector3 origin, Quaternion rotation)
    {
        float length = 0.15f;

        DebugExtension.DebugArrow(origin, rotation * Vector3.right * length, Color.red);
        DebugExtension.DebugArrow(origin, rotation * Vector3.up * length, new Color(0, .6f, 0));
        DebugExtension.DebugArrow(origin, rotation * Vector3.forward * length, Color.blue);
    }

    private Vector3[] PredictJointsPositions(float[] predictedPositions)
    {
        // 예측된 관절 위치 정보를 저장할 배열
        Vector3[] predictedJointPositions = new Vector3[22];

        // 예측된 관절 위치 정보를 이용하여 predictedJointPositions 배열에 저장
        for (int i = 0; i < 22; i++)
        {
            // 서버로부터 받은 예측된 위치 값을 사용하여 Vector3로 변환하여 저장
            int startIndex = 3 * i;
            float x = predictedPositions[startIndex];
            float y = predictedPositions[startIndex + 1];
            float z = predictedPositions[startIndex + 2];
            predictedJointPositions[i] = new Vector3(x, y, z);
        }

        // 예측된 관절 위치를 담은 배열을 반환합니다.
        return predictedJointPositions;
    }

    void ManipulateCharacter(string response)
    {
        try
        {
            // 서버 응답을 파싱하여 예측된 관절 위치를 추출합니다.
            string[] stringArray = response.Split(',');
            Vector3[] predictedPositions = new Vector3[stringArray.Length / 3]; // 예측된 관절 위치를 담을 Vector3 배열 생성
            int index = 0;
            for (int i = 0; i < stringArray.Length; i += 3)
            {
                if (float.TryParse(stringArray[i].Trim('[', ']', '"').Trim(), out float x) &&
                    float.TryParse(stringArray[i + 1].Trim('[', ']', '"').Trim(), out float y) &&
                    float.TryParse(stringArray[i + 2].Trim('[', ']', '"').Trim(), out float z))
                {
                    //predictedPositions[index++] = new Vector3(x, y, z); // 예측된 관절 위치를 Vector3로 변환하여 배열에 저장
                    predictedPositions[index] = new Vector3(x, y, z); // 예측된 관절 위치를 Vector3로 변환하여 배열에 저장
                    Debug.Log(predictedPositions[index++]);
                }
                else
                {
                    Debug.LogError("Error parsing float values from response.");
                    return;
                }
            }

            // 예측된 위치를 적용합니다.
            foreach (var jointTransform in _jointTransforms)
                if (jointTransform != null && index < predictedPositions.Length)
                    jointTransform.localPosition = predictedPositions[index++];
        }
        catch (Exception e)
        {
            Debug.LogError("Error while predicting and manipulating character: " + e.Message);
        }
    }
}
