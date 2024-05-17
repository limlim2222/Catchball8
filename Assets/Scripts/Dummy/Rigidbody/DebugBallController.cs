using UnityEngine;
using Photon.Pun;

public class DebugBallController : MonoBehaviour
{
    [SerializeField] PhotonView pv;
    Rigidbody rb;
    [SerializeField] float moveForce = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine) return;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();
        rb.AddForce(moveDir * moveForce);
    }
}
