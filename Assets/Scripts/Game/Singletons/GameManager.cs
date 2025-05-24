using System.Collections.Generic;
using Unity.Netcode;

namespace Game
{
    public class GameManager : NetworkSingleton<GameManager>
    {
        private Dictionary<ulong, Player> _players;
        public Player GetPlayer()
            => _players[OwnerClientId];
        public Player GetPlayer(ulong id)
            => _players[id];

        public bool IsActive { get; private set; } = true;

        private void Start()
        {
            NetworkManager.OnServerStarted += () =>
            {
                _players = new Dictionary<ulong, Player>();

                NetworkManager.OnClientConnectedCallback += id =>
                {
                    var player = NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<Player>();

                    player.SideSelected += () => player.Spawn();

                    _players.Add(id, player);
                };
                NetworkManager.OnClientDisconnectCallback += id =>
                {
                    _players.Remove(id);
                };
            };
        }
    }
}
