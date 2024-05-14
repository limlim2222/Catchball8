using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class External_cam : MonoBehaviour
{
    private Transform cameraRig; // CameraRig Transform
    private Transform mainCamera; // CameraRig의 메인 카메라 Transform
    private Transform externalCamera; // 외부 카메라 Transform

    public float distanceAhead = 0.2f; // 외부 카메라를 앞으로 이동시킬 거리

    private void Start()
    {
        // CameraRig과 메인 카메라를 찾습니다.
        cameraRig = GameObject.FindObjectOfType<SteamVR_PlayArea>().transform;
        mainCamera = cameraRig.GetComponentInChildren<Camera>().transform;

        // 외부 카메라를 찾습니다.
        externalCamera = transform; // 외부 카메라는 이 스크립트가 부착된 게임 오브젝트의 하위에 있습니다.
    }

    private void Update()
    {
        // 카메라 릴의 위치와 회전을 외부 카메라에 적용합니다.
        if (cameraRig != null && mainCamera != null && externalCamera != null)
        {
            // 외부 카메라를 앞으로 이동시킵니다.
            Vector3 aheadPosition = mainCamera.position + mainCamera.forward * distanceAhead;

            // 외부 카메라의 위치와 회전을 메인 카메라와 동일하게 설정합니다.
            externalCamera.position = aheadPosition;
            externalCamera.rotation = mainCamera.rotation;
        }
    }
}
