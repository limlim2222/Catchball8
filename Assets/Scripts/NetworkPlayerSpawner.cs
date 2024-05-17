using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PlayerToSpawn;
    [SerializeField] GameObject BallPrefab1ToSpawn;
    [SerializeField] GameObject BallPrefab2ToSpawn;
    [SerializeField] GameObject BallPrefab3ToSpawn;

    [SerializeField] Transform ballSpawnPosition1;
    [SerializeField] Transform ballSpawnPosition2;
    [SerializeField] Transform ballSpawnPosition3;

    GameObject spawnedPlayerPrefab;
    GameObject spawnedBallPrefab1;
    GameObject spawnedBallPrefab2;
    GameObject spawnedBallPrefab3;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(PlayerToSpawn.name, Vector3.zero, Quaternion.identity);
        spawnedBallPrefab1 = PhotonNetwork.Instantiate(BallPrefab1ToSpawn.name, ballSpawnPosition1.position, ballSpawnPosition1.rotation);
        spawnedBallPrefab2 = PhotonNetwork.Instantiate(BallPrefab2ToSpawn.name, ballSpawnPosition2.position, ballSpawnPosition2.rotation);
        spawnedBallPrefab3 = PhotonNetwork.Instantiate(BallPrefab3ToSpawn.name, ballSpawnPosition3.position, ballSpawnPosition3.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        PhotonNetwork.Destroy(spawnedBallPrefab1);
        PhotonNetwork.Destroy(spawnedBallPrefab2);
        PhotonNetwork.Destroy(spawnedBallPrefab3);
    }
}
