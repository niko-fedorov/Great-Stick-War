using Unity.Netcode;
using UnityEngine;

public class StickNetworkManager : NetworkManager
{
    

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {

        };
    }
}
