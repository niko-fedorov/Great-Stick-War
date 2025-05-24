using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game
{
    public class ServerManager : NetworkSingleton<ServerManager>
    {
        public event UnityAction PlayerDisconnected;

        public ServerData ServerData;
        public PlayerData PlayerData { get; private set; }

        private UnityTransport _transport;

        private NetworkVariable<SideType> _sideType = new NetworkVariable<SideType>(writePerm: NetworkVariableWritePermission.Owner);

        private void Start()
        {
            _transport = NetworkManager.GetComponent<UnityTransport>();

            NetworkManager.OnClientStarted += () =>
            {
                NetworkManager.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
                {

                };
            };

            NetworkManager.OnServerStarted += () =>
            {
                NetworkManager.SceneManager.LoadScene("game", LoadSceneMode.Single);

                NetworkManager.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
                {

                };
            };
        }
    }
}


