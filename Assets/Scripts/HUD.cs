using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class HUD : Singleton<HUD>
{
    [SerializeField]
    private GameObject _pausePanel,
                       _victoryPanel,
                       _deathPanel;

    [SerializeField]
    private Button _reenlistButton,
                   _disconnectButton;

    [SerializeField]
    private Image _vignetteImage;
    [SerializeField]
    private float _damageTime;
    private float _vignetteCurrentVelocity;

    [SerializeField]
    private Slider _healthBar;
    [SerializeField]
    private Text _actionText,
                 _gunNameText,
                 _gunAmmoText;

    [SerializeField]
    private InputField _chatInputField;

    private GameObject _currentPanel;

    private void Start()
    {   
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        
        _reenlistButton.onClick.AddListener(() => 
        { 
            //player.transform.position = MapManager.
        });
        _disconnectButton.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
            SceneManager.LoadScene(0);
        });

        player.Damaged += () =>
        {
            _vignetteImage.color += new Color32(0, 0, 0, 128);

            _healthBar.value = player.Health;
        };
        player.Died += (origin) =>
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _deathPanel.SetActive(true);
        };
        player.Controller.Interaction += (subject) =>
        {
            _actionText.text = $"{subject.ActionText} {subject.Name}";
            _actionText.color = subject.TextColor;
        };
        player.Controller.InteractionEnd += () =>
        {
            _actionText.text = string.Empty;
            _actionText.color = GameManager.Instance.GetActionColor(ActionType.Default);
        };
        player.Controller.GunChange += (gun) =>
        {
            void ChangeAmmoText() => _gunAmmoText.text = $"{gun.CurrentClipAmmo}/{gun.Data.ClipAmmo} | {gun.CurrentAmmo}";

            _gunNameText.text = gun.Data.Name;
            ChangeAmmoText();
            gun.AmmoChanged += ChangeAmmoText;

            player.Controller.Gun.AmmoChanged -= ChangeAmmoText;
        };
    }

    public void Unpause()
    {
        Time.timeScale = 1;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _currentPanel = null;

        _pausePanel.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_pausePanel.activeSelf)
            {
                Time.timeScale = 1;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                _currentPanel = null;

                _pausePanel.SetActive(false);
            }
            else if(!_currentPanel)
            {
                Time.timeScale = 0;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                _currentPanel = _pausePanel;

                _pausePanel.SetActive(true);              
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
            _chatInputField.ActivateInputField();

        if (_vignetteImage.color.a != 0)
            _vignetteImage.color = new Color(0, 0, 0, Mathf.SmoothDamp(_vignetteImage.color.a, 0, ref _vignetteCurrentVelocity, _damageTime));
    }
}
