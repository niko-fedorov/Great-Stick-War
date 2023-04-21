using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreboard : MonoBehaviour
{
    [SerializeField]
    private Text _playerTextElementPrefab;

    [SerializeField]
    private Transform _layout; 

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            var element = Instantiate(_playerTextElementPrefab, _layout);

            var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).GetComponent<Player>();
            player.Died += (p) =>
            {
               // element.text = $"{player.Name}  {}  {}";
                
            };
        };
    }
}
