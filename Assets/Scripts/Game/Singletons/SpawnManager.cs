using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        [SerializeField]
        private PlayerController _playerPrefab;

        private void Start()
        {
            
        }

        //public void SpawnPlayers()
        //{
        //    foreach (var player in NetworkManager.Singleton.ConnectedClients)
        //    {
        //        var player_ = Instantiate(_playerPrefab);

        //        player_.NetworkObject.SpawnWithOwnership(player.Key);

        //        player_.transform.position = new Vector3(Random.Range(0, 64), 0, Random.Range(0, 64));
        //    }
        //}
    }
}
