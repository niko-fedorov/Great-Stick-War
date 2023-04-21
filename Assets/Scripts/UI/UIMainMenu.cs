using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField]
    private string _gameMapName;
    [SerializeField]
    private InputField _nicknameInputField;
    [SerializeField]
    private Button _singleplayerButton,
                   _multiplayerButton;

    [Header("Multiplayer Panel Settings")]
    [SerializeField]
    private GameObject _multiplayerPanel;
    [SerializeField]
    private Button _hostButton,
                   _joinButton;

    private void Start()
    {
        _singleplayerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("test_mp", LoadSceneMode.Single);
        });
        _multiplayerButton.onClick.AddListener(() =>
        {
            _multiplayerPanel.SetActive(true);
            _singleplayerButton.gameObject.SetActive(false);
            _multiplayerButton.gameObject.SetActive(false);
        });

        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("test_mp", LoadSceneMode.Single);
        });
        _joinButton.onClick.AddListener(() =>
            NetworkManager.Singleton.StartClient());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _multiplayerPanel.gameObject.activeSelf)
        {
            _multiplayerPanel.SetActive(false);
            _singleplayerButton.gameObject.SetActive(true);
            _multiplayerButton.gameObject.SetActive(true);
        }
    }
}
