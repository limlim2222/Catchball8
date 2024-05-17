using UnityEngine;
using Photon.Pun;

class DebugBallButton : MonoBehaviour
{
    [SerializeField] DebugBall ball;
    [SerializeField] PhotonView pv;
    [SerializeField] Transform pos;
    internal void Assign(DebugBall ball)
    {
        this.ball = ball;
        pv = ball.GetComponent<PhotonView>();
    }

    public void OnClick()
    {
        if (!pv.IsMine) return;
        ball.Snap(pos);
        ball.ResetPhysics();
    }
}