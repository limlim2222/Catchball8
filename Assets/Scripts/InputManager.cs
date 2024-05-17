using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class InputManager : MonoBehaviour
{
    public UnityAction OnPinchDownAny, OnPinchDownLeft, OnPinchDownRight;
    public UnityAction OnPinchReleaseAny, OnPinchReleaseLeft, OnPinchReleaseRight;

    public static InputManager Instance { get; private set; }
    public SteamVR_Action_Boolean enumPinchAction;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        bool pinchedDown = false;
        if (enumPinchAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            OnPinchDownLeft?.Invoke();
            pinchedDown = true;
        }
        if (enumPinchAction.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            OnPinchDownRight?.Invoke();
            pinchedDown = true;
        }
        if (pinchedDown)
            OnPinchDownAny?.Invoke();
        bool pinchedRelease = false;
        if (enumPinchAction.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            OnPinchReleaseLeft?.Invoke();
            pinchedRelease = true;
        }
        if (enumPinchAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            OnPinchReleaseRight?.Invoke();
            pinchedRelease = true;
        }
        if (pinchedRelease)
            OnPinchReleaseAny?.Invoke();
    }
}