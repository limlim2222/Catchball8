using Photon.Pun;
using UnityEngine;

public class DebugTakeoverableBallButton : MonoBehaviour
{
    [SerializeField] DebugBall ball;
    [SerializeField] PhotonView pv;
    [SerializeField] DebugBallButton ballBtn;

    private DebugBallController dbc;
    private void Update()
    {
        if (!ball)
        {
            ball = FindObjectOfType<DebugBall>();
            return;
        }
        if (!dbc) dbc = ball.GetComponent<DebugBallController>();
        if(dbc.enabled) dbc.enabled = false;
        if (!pv) pv = ball.GetComponent<PhotonView>();
    }

    public void OnClick()
    {
        pv.RequestOwnership();
        ballBtn.Assign(ball);
    }
}
