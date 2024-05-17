using Photon.Pun;
using Photon.Realtime;

public class DebugNetworkManager : MonoBehaviourPunCallbacks
{
    public DebugNetworkSpawner spawner;
    public string roomName = "TestRoom";

    void Start()
    {
        if (!spawner)
            spawner = FindObjectOfType<DebugNetworkSpawner>();

        DebugLogger.Instance.Log("Connecting Server");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        DebugLogger.Instance.Log("Server Connected");
        DebugLogger.Instance.Log("Joining Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        DebugLogger.Instance.Log("Raw Connection Established");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        DebugLogger.Instance.Log("Lobby Joined");
        DebugLogger.Instance.Log("Creating Room or Joining Existing of name " + roomName);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugLogger.Instance.LogError(string.Format("Room creation failed with error code {0} and error message {1}", returnCode, message));
    }

    public override void OnJoinedRoom()
    {
        DebugLogger.Instance.Log("Room Joined");
        DebugLogger.Instance.Log("Summoning");
        spawner.Summon();
    }
}