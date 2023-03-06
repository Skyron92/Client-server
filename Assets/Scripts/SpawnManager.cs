using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ClientServer
{
    public class SpawnManager : NetworkBehaviour, INetworkPrefabInstanceHandler {
    public GameObject PrefabToSpawn;
    public bool SpawnPrefabAutomatically;
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

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) {
        m_PrefabInstance.SetActive(true);
        return m_SpawnedNetworkObject;
    }
    
    public void Destroy(NetworkObject networkObject) {
        m_PrefabInstance.SetActive(false);
    }

    private void SpawnInstance() {
        if (!IsServer) return;
        if (m_PrefabInstance != null && m_SpawnedNetworkObject != null && !m_SpawnedNetworkObject.IsSpawned) {
            m_PrefabInstance.SetActive(true);
            m_SpawnedNetworkObject.Spawn();
        }
    }

    public override void OnNetworkSpawn() {
        NetworkManager.PrefabHandler.AddHandler(PrefabToSpawn, this);
        if (!IsServer || !SpawnPrefabAutomatically) return;
        if (SpawnPrefabAutomatically) SpawnInstance();
    }

    public override void OnNetworkDespawn() {
        if (m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned) m_SpawnedNetworkObject.Despawn();
        base.OnNetworkDespawn();
    }
    
    public override void OnDestroy() {
        if (m_PrefabInstance != null) {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(PrefabToSpawn);
            Destroy(m_PrefabInstance);
        }
        base.OnDestroy();
    }
}
}
