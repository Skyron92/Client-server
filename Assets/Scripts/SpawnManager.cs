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
        // Instantiate our instance when we start (for both clients and server)
        m_PrefabInstance = Instantiate(PrefabToSpawn, SpawnPoints[Random.Range(0, SpawnPoints.Count)]);

        // Get the NetworkObject component assigned to the Prefab instance
        m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();

        // Set it to be inactive
        m_PrefabInstance.SetActive(false);
    }

    private IEnumerator SpawnTimer() {
        yield return new WaitForSeconds(2);
        SpawnInstance();
        yield break;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) {
        m_PrefabInstance.SetActive(true);
        return m_SpawnedNetworkObject;
    }

    /// <summary>
    /// Client and Server side
    /// INetworkPrefabInstanceHandler.Destroy implementation
    /// </summary>
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
        // We register our network Prefab and this NetworkBehaviour that implements the
        // INetworkPrefabInstanceHandler interface with the Prefab handler
        NetworkManager.PrefabHandler.AddHandler(PrefabToSpawn, this);
        if (!IsServer || !SpawnPrefabAutomatically) return;
        if (SpawnPrefabAutomatically) SpawnInstance();
    }

    public override void OnNetworkDespawn() {
        if (m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned) m_SpawnedNetworkObject.Despawn();
        base.OnNetworkDespawn();
    }
    
    public override void OnDestroy() {
        // This example destroys the
        if (m_PrefabInstance != null) {
            // Always deregister the prefab
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(PrefabToSpawn);
            Destroy(m_PrefabInstance);
        }
        base.OnDestroy();
    }
}
}
