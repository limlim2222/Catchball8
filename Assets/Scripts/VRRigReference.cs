using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReference : MonoBehaviour
{
    public static VRRigReference Singleton;

    public Transform headtarget;
    public Transform lefttarget;
    public Transform righttarget;
    //public Transform tracker1;
    public Transform leftlegtarget;
    public Transform rightlegtarget;

    private void Awake()
    {
        Singleton = this;
    }

}
