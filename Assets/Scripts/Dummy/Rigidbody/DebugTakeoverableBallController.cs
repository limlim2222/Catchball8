using UnityEngine;
using Photon.Pun;

public class DebugTakeoverableBallController : MonoBehaviour
{
    [SerializeField] DebugBall ball;
    [SerializeField] PhotonView pv;
    [SerializeField] float moveMultiplier = 1;

    private void Update()
    {
        if (!ball)
        {
            ball = FindObjectOfType<DebugBall>();
            return;
        }
        if (!pv) pv = ball.GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            pv.GetComponent<Rigidbody>().AddForce(
                new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * moveMultiplier
            );
        }
    }
}