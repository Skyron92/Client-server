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

    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;

    private void Start() {
        m_PrefabInstance = Instantiate(PrefabToSpawn, transform);
        m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
        m_PrefabInstance.SetActive(false);
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer() {
        yield return new WaitForSeconds(2);
        SpawnInstance();
        StartCoroutine(SpawnTimer());
        yield break;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) {
        m_PrefabInstance.SetActive(true);
        m_PrefabInstance.transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Count)].position;
        return m_SpawnedNetworkObject;
    }
    public void Destroy(NetworkObject networkObject) {
        m_PrefabInstance.SetActive(false);
    }

    public void SpawnInstance() {
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
