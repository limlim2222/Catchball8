using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class ModelIntegrationManager : MonoBehaviour
{
    private List<XRNodeState> nodeStates = new List<XRNodeState>();
    private RequestSocket requestSocket;
    public static ModelIntegrationManager Instance { get; private set; }

    private void Start()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            SetupServer();
        }
    }

    private void OnApplicationQuit()
    {
        requestSocket.Close();
        NetMQConfig.Cleanup();
    }

    void SetupServer()
    {
        ForceDotNet.Force();
        InputTracking.GetNodeStates(nodeStates);
        StartCoroutine(InitXR());
        requestSocket = new RequestSocket();
        requestSocket.Connect("tcp://127.0.0.1:3550");
        Debug.Log("server initiated");
    }

    public IEnumerator InitXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
    }

    internal void SendFrame(string data) => requestSocket.SendFrame(data);
    internal string ReceiveFrame() => requestSocket.ReceiveFrameString();
}