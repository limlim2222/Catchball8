using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DebugNetworkSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> ToSpawnPrefabsAtOriginGlobal = new List<GameObject>();
    [SerializeField] List<GameObject> ToSpawnPrefabsPositionSpecificGlobal = new List<GameObject>();
    [SerializeField] List<Transform> PositionSpecifiersGlobal = new List<Transform>();

    [SerializeField] List<GameObject> ToSpawnPrefabsAtOriginLocal = new List<GameObject>();
    [SerializeField] List<GameObject> ToSpawnPrefabsPositionSpecificLocal = new List<GameObject>();
    [SerializeField] List<Transform> PositionSpecifiersLocal = new List<Transform>();

    List<GameObject> SpawnedGameObjects = new List<GameObject>();

    internal void Summon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Spawn(ToSpawnPrefabsAtOriginGlobal, ToSpawnPrefabsPositionSpecificGlobal, PositionSpecifiersGlobal);
            Spawn(ToSpawnPrefabsAtOriginLocal, ToSpawnPrefabsPositionSpecificLocal, PositionSpecifiersLocal);
        }
        else Spawn(ToSpawnPrefabsAtOriginLocal, ToSpawnPrefabsPositionSpecificLocal, PositionSpecifiersLocal);
    }

    void Spawn(List<GameObject> origin, List<GameObject> pspec, List<Transform> p)
    {
        foreach (GameObject prefab in origin)
            SpawnedGameObjects.Add(PhotonNetwork.Instantiate(
                prefab.name, Vector3.zero, Quaternion.identity
            ));
        for (int i = 0; i < pspec.Count; ++i)
            SpawnedGameObjects.Add(PhotonNetwork.Instantiate(
                pspec[i].name, p[i].position, p[i].rotation
            ));
    }

    public void OnDestroy()
    {
        for (int i = SpawnedGameObjects.Count - 1; i >= 0; --i)
            PhotonNetwork.Destroy(SpawnedGameObjects[i]);
    }
}
