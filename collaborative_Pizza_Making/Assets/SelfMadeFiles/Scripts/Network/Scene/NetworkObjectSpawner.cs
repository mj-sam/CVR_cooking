using Unity.Netcode;
using UnityEngine;

namespace SelfMadeFiles.Scripts.Network.Scene
{
    public class NetworkObjectSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject networkObjectPrefab;
        [SerializeField] private Transform spawnPoint;
        
        public void SpawnNetworkObject()
        {
            SpawnNetworkObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnNetworkObjectServerRpc(ulong clientId)
        {
            Instantiate(networkObjectPrefab, spawnPoint.position, spawnPoint.rotation).SpawnWithOwnership(clientId);
        }
    }
}
