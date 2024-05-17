using UnityEngine;
using Valve.VR;

public class VRHand : MonoBehaviour
{
    [SerializeField] IMountable mountedBall = null;

    InputManager im;
    [SerializeField] private bool isLeft;
    [SerializeField] Transform mountTransform;
    internal Transform MountTransform => mountTransform;

    // Start is called before the first frame update
    void Start()
    {
        im = InputManager.Instance;
        if (isLeft)
        {
            im.OnPinchDownLeft += Pickup;
            im.OnPinchReleaseLeft += Drop;
        }
        else
        {
            im.OnPinchDownRight += Pickup;
            im.OnPinchReleaseRight += Drop;
        }
    }

    public void Pickup()
    {
        Debug.Log("Pickup called");
        mountedBall = GetAdjacentBall();

        if (mountedBall == null)
            return;
        Debug.Log("Adjacent Ball Found and that was: " + mountedBall);

        mountedBall.MountTo(this);
    }

    public void Drop()
    {
        Debug.Log("Drop called");
        if (mountedBall == null)
            return;
        if (!mountedBall.AmIYourMount(this))
            return;
        Debug.Log("Mounted Ball was: " + mountedBall);
        mountedBall.Unmount();
        mountedBall = null;
    }

    private IMountable GetAdjacentBall()
    {
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
            return hit.transform.gameObject.GetComponent<IMountable>();
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * 300);
    }
}
