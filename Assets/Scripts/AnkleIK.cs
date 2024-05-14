using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class AnkleIK : MonoBehaviour
{
    public int ChainLength;
    public Transform LastObject;
    public Transform IKTarget;

    float[] BonesLength;
    Transform[] Bones;
    Vector3[] Positions;
    float CompleteLength;
    void Awake()
    {
        Init();
    }

    void Init()
    {
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength];

        Transform nowBone = LastObject;
        CompleteLength = 0;
        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = nowBone;
            if (i == Bones.Length - 1)
            {
            }
            else
            {
                BonesLength[i] = (Bones[i + 1].position - nowBone.position).magnitude;
                CompleteLength += BonesLength[i];
            }
            nowBone = nowBone.parent;
        }
    }

    void LateUpdate()
    {
        ResloveIK();
    }

    void ResloveIK()
    {
        if (IKTarget == null)
            return;
        if (BonesLength.Length != ChainLength)
            Init();

        for (int i = 0; i < Bones.Length; i++)
            Positions[i] = Bones[i].position;

        if ((IKTarget.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            Vector3 direction = (IKTarget.position - Positions[0]).normalized;
            for (int i = 1; i < Positions.Length; i++)
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
        }
        else
        {
            for (int iteration = 0; iteration < 10; iteration++)
            {
                //backward
                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    if (i == Positions.Length - 1)
                        Positions[i] = IKTarget.position;
                    else
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                }

                //forward
                for (int i = 1; i < Positions.Length; i++)
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
            }
        }

        for (int i = 0; i < Bones.Length; i++)
            Bones[i].position = Positions[i];
    }

}
