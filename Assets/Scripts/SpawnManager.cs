using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ClientServer
{
    public class SpawnManager : NetworkBehaviour {
    public GameObject PrefabToSpawn;
    public List<Transform> SpawnPoints = new List<Transform>();
    private static float _time;
    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;

    private void Update() {
        _time += Time.deltaTime;
        if (_time >= 2f && IsServer && Player.Players.Count > 0) {
            _time = 0; 
            m_PrefabInstance = Instantiate(PrefabToSpawn, SpawnPoints[Random.Range(0, SpawnPoints.Count)]);
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.Spawn();
        }
    }
    }
}
