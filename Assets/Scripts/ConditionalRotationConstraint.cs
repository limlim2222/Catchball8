using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteAlways]
public class ConditionalRotationConstraint : MonoBehaviour
{
    public RotationConstraint rc;
    public float degree = 0;
    [Tooltip("Checking this makes the condition into 'greater than'. By default the condition is 'less than'.")]
    public bool inverseCondition;

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.z < 0 != inverseCondition)
            rc.enabled = true;
        else rc.enabled = false;
    }
}
