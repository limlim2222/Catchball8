using UnityEngine;
using Photon.Pun;

public class DebugBall : MonoBehaviour
{
    Rigidbody rb;
    PhotonRigidbodyView rbView;
    [SerializeField] PhotonView pv;

    private void Start()
    {
        if (!pv.IsMine) return;
        FindObjectOfType<DebugBallButton>().Assign(this);
        rb = GetComponent<Rigidbody>();
        rbView = GetComponent<PhotonRigidbodyView>();
    }

    internal void ResetPhysics()
    {
        if (!pv.IsMine) return;
        rbView.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rbView.enabled = true;
    }
    
    internal void Snap(Transform pos)
    {
        if (!pv.IsMine) return;
        transform.position = pos.position;
        transform.rotation = pos.rotation;
    }
}
