using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    public GameObject spawnedBallPrefab1;
    public GameObject spawnedBallPrefab2;
    public GameObject spawnedBallPrefab3;

    


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NetworkPlayer_H1" + "", transform.position, transform.rotation);
        spawnedBallPrefab1 = PhotonNetwork.Instantiate("ball1" + "", spawnedBallPrefab1.transform.position, spawnedBallPrefab1.transform.rotation);
        spawnedBallPrefab2 = PhotonNetwork.Instantiate("ball2" + "", spawnedBallPrefab2.transform.position, spawnedBallPrefab2.transform.rotation);
        spawnedBallPrefab3 = PhotonNetwork.Instantiate("ball3" + "", spawnedBallPrefab3.transform.position, spawnedBallPrefab3.transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
