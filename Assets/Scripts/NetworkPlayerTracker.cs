using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

public class NetworkPlayerTracker : MonoBehaviour
{
    public Transform headtarget;
    public Transform lefttarget;
    public Transform righttarget;
    public Transform tracker1;
    public Transform tracker2;
    public Transform tracker3;

    public PhotonView photonView;

    void Awake()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            headtarget.position = VRRigReferenceTracker.Singleton.headtarget.position;
            headtarget.rotation = VRRigReferenceTracker.Singleton.headtarget.rotation;

            righttarget.position = VRRigReferenceTracker.Singleton.righttarget.position;
            righttarget.rotation = VRRigReferenceTracker.Singleton.righttarget.rotation;

            lefttarget.position = VRRigReferenceTracker.Singleton.lefttarget.position;
            lefttarget.rotation = VRRigReferenceTracker.Singleton.lefttarget.rotation;

            tracker1.position = VRRigReferenceTracker.Singleton.tracker1.position;
            tracker1.rotation = VRRigReferenceTracker.Singleton.tracker1.rotation;

            tracker2.position = VRRigReferenceTracker.Singleton.tracker2.position;
            tracker2.rotation = VRRigReferenceTracker.Singleton.tracker2.rotation;

            tracker3.position = VRRigReferenceTracker.Singleton.tracker3.position;
            tracker3.rotation = VRRigReferenceTracker.Singleton.tracker3.rotation;
        }
    }
}
