using Unity.Netcode;
using UnityEngine;

namespace ClientServer
{
    public struct DiscoveryBroadcastData : INetworkSerializable
    {
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        }
    }
}