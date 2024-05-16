using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDataSender : MonoBehaviour
{
    ModelIntegrationManager mim;
    public Transform hmd, lhc, rhc;
    List<Matrix4x4> mylist = new List<Matrix4x4>();
    Queue<ViveTriplet> myqueue = new Queue<ViveTriplet> ();
    int windowsize = 41;
    bool bSent;
    // Start is called before the first frame update
    void Start()
    {
        mim = ModelIntegrationManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(mylist.Count < windowsize * 3)
        {
            mylist.Add(hmd.localToWorldMatrix);
            mylist.Add(lhc.localToWorldMatrix);
            mylist.Add(rhc.localToWorldMatrix);
        }
        if(myqueue.Count < windowsize)
        {
            myqueue.Enqueue(new ViveTriplet(
                    hmd.localToWorldMatrix,
                    lhc.localToWorldMatrix,
                    rhc.localToWorldMatrix
            ));
        }
        if (mylist.Count == windowsize * 3 && myqueue.Count == windowsize && !bSent)
        {
            bSent = true;
            Send();
        }

    }

    void Send()
    {
        mim.SendFrame(JsonUtility.ToJson(new Matrix4x4SerializableList(mylist)));
        mim.ReceiveFrame();
        foreach(ViveTriplet vt in myqueue)
        {
            mim.SendFrame(JsonUtility.ToJson(vt.ConvertToSerializable()));
            mim.ReceiveFrame();
        }
    }
}
