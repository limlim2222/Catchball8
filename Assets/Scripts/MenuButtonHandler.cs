using UnityEngine;
using Valve.VR;
using AsyncIO;
using UnityEngine.XR.Management;
using System.Collections;
using UnityEngine.XR;

public class MenuButtonHandler : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        // Vive 컨트롤러 메뉴 버튼을 누를 때마다 일시 정지 토글
        if (SteamVR_Actions.default_MenuButton.GetStateDown(SteamVR_Input_Sources.Any))
        {
            if (isPaused)
            {
                Time.timeScale = 1.0f; // 게임 시간을 다시 진행
                isPaused = false;
            }
            else
            {
                Time.timeScale = 0.0f; // 게임 시간을 정지
                isPaused = true;
            }
        }
    }
}






