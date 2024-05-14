using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceTransform : MonoBehaviour
{
    public GameObject Reference;

    // Update is called once per frame
    void Update()
    {
        transform.position = Reference.transform.position;
        Vector3 forward = new Vector3(Reference.transform.forward.x, 0f, Reference.transform.forward.z);
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
