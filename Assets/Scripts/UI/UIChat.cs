using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : NetworkBehaviour
{
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private InputField _inputField;

    [SerializeField]
    private Text _messagePrefab;

    private void Start()
    {
        //_inputField.onValueChanged.AddListener()
        _inputField.onSubmit.AddListener((text) =>
        {
            SendMessageServerRpc(
                $"<color={NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().TextColor}>" +
                    $"{NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().Name}</color>: " +
                        $"{text}");

            _inputField.text = string.Empty;
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string message)
        => ReceiveMessageClientRpc(message);

    [ClientRpc]
    private void ReceiveMessageClientRpc(string message)
    {
        Instantiate(_messagePrefab, _panel.transform).text = message;
    }

    private void Update()
    {
        
    }
}
