using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class InputManager : MonoBehaviour
{
    public UnityAction OnPinch;
    public static InputManager Instance { get; private set; }

    private void Start()
    {
        if (Instance) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SteamVR_Actions.default_GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
            OnPinch.Invoke();
    }
}