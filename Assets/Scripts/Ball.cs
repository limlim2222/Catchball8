using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour, IMountable
{
    [SerializeField] PhotonView pv;

    public float waitTime = 10.0f;
    public float maxSpeed = 6.0f;
    public float accelerationRate = 0.2f;

    private Coroutine ballReturnCoroutine = null;
    private Rigidbody BallRigidbody = null;

    [SerializeField] private VRHand whoAmIMounted;

    private Vector3 BallOriginPosition = Vector3.zero;
    private Quaternion BallOriginRotation = Quaternion.identity;

    private Vector3 lastPos;
    [SerializeField] private float velocityMultiplier = 1f;

    bool shouldMove = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine) return;
        BallRigidbody = GetComponent<Rigidbody>();
        BallOriginPosition = transform.position;
        BallOriginRotation = transform.rotation;
        BallRigidbody.velocity = Vector3.zero;
        BallRigidbody.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(whoAmIMounted)
        {
            Transform t = whoAmIMounted.MountTransform;
            lastPos = transform.position = t.position;
            transform.rotation = t.rotation;
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
            if (ballReturnCoroutine != null)
                StopCoroutine(ballReturnCoroutine);
            ballReturnCoroutine = StartCoroutine(ReturnObject());
        }

        else if (collision.gameObject.CompareTag("Interactable") && collision.gameObject.CompareTag("Duck"))
        {
            if (ballReturnCoroutine != null)
                StopCoroutine(ballReturnCoroutine);
            ballReturnCoroutine = StartCoroutine(ReturnObject());
        }

    }

    IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(waitTime);

        ToggleKinetics(false);

        BallRigidbody.velocity = Vector3.zero;
        BallRigidbody.angularVelocity = Vector3.zero;


        transform.position = BallOriginPosition;
        transform.rotation = BallOriginRotation;

        ballReturnCoroutine = null;

        yield return null;
    }

    void ToggleKinetics(bool activate)
    {
        BallRigidbody.useGravity = activate && true;
        BallRigidbody.isKinematic = activate && false;
    }

    void IMountable.MountTo(VRHand hand)
    {
        whoAmIMounted = hand;
        ToggleKinetics(false);
    }

    void IMountable.Unmount()
    {
        whoAmIMounted = null;
        ToggleKinetics(true);
        BallRigidbody.AddForce((transform.position - lastPos) * velocityMultiplier);
    }

    public bool AmIYourMount(VRHand vRHand)
        => whoAmIMounted == vRHand;
}
