using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRHand : MonoBehaviour
{
    public SteamVR_Action_Boolean GripGrap;

    private SteamVR_Behaviour_Pose myHand = null;

    private Transform myTransform = null;
    private Rigidbody myRigidbody = null;

    private Rigidbody currentRigidbody = null;

    private List<Rigidbody> contactRigidbodies = new List<Rigidbody>();

    // Start is called before the first frame update
    void Start()
    {
        myHand = GetComponent<SteamVR_Behaviour_Pose>();
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GripGrap.GetStateDown(myHand.inputSource))
        {
            Pickup();
        }

        if(GripGrap.GetStateUp(myHand.inputSource))
        {
            Drop();
        }
    }

    public void Pickup()
    {
        currentRigidbody = GetNearestRigidBody();

        if (currentRigidbody == null)
            return;

        currentRigidbody.useGravity = false;
        currentRigidbody.isKinematic = true;

        currentRigidbody.transform.position = myTransform.position;
        currentRigidbody.transform.parent = myTransform;
    }

    public void Drop()
    {
        if (currentRigidbody == null)
            return;

        currentRigidbody.useGravity = true;
        currentRigidbody.isKinematic = false;

        currentRigidbody.transform.parent = null;

        currentRigidbody.velocity = myHand.GetVelocity();
        currentRigidbody.angularVelocity = myHand.GetAngularVelocity();

        currentRigidbody = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            contactRigidbodies.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            contactRigidbodies.Remove(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private Rigidbody GetNearestRigidBody()
    {
        Rigidbody nearestRigidBody = null;

        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Rigidbody rigidbody in contactRigidbodies)
        {
            distance = (rigidbody.transform.position - myTransform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearestRigidBody = rigidbody;
            }
        }

        return nearestRigidBody;
    }
}
