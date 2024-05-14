using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public float waitTime = 10.0f;
    public float maxSpeed = 6.0f; // 최대 속도
    public float accelerationRate = 0.2f;

    private Coroutine myCoroutine = null;

    private Rigidbody BallRigidbody = null;
    private Transform BallTransform = null;


    private Vector3 BallOriginPosition = Vector3.zero;
    private Quaternion BallOriginRotation = Quaternion.identity;

    private bool shouldMove = false;

    // Start is called before the first frame update
    void Start()
    {
        BallRigidbody = GetComponent<Rigidbody>();
        BallTransform = GetComponent<Transform>();

        //초기값
        BallOriginPosition = BallTransform.position;
        BallOriginRotation = BallTransform.rotation;

        BallRigidbody.velocity = Vector3.zero;
        BallRigidbody.angularVelocity = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            if (BallRigidbody.velocity.magnitude < maxSpeed)
            {
                BallRigidbody.velocity += BallTransform.forward * accelerationRate * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("Hand")&& collision.gameObject.CompareTag("Interactable"))
        {
            shouldMove = true;
        }

        else if (!collision.gameObject.CompareTag("Interactable") && !collision.gameObject.CompareTag("Box"))
        {
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(RetrunObject());
            }
        }

        else if (collision.gameObject.CompareTag("Interactable") && collision.gameObject.CompareTag("Duck"))
        {
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(RetrunObject());
            }
        }

    }



    IEnumerator RetrunObject()
    {
        yield return new WaitForSeconds(waitTime);

        BallRigidbody.velocity = Vector3.zero;
        BallRigidbody.angularVelocity = Vector3.zero;


        BallTransform.position = BallOriginPosition;
        BallTransform.rotation = BallOriginRotation;

        myCoroutine = null;

        yield return null;

    }

}
