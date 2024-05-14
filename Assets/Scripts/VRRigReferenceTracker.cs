using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferenceTracker : MonoBehaviour
{
    public static VRRigReferenceTracker Singleton;

    public Transform headtarget;
    public Transform lefttarget;
    public Transform righttarget;
    public Transform tracker1;
    public Transform tracker2;
    public Transform tracker3;

    private void Awake()
    {
        Singleton = this;
    }
}
