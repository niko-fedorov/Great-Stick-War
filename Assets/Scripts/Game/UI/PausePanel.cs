using UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class PausePanel : Panel
    {
        protected override void OnInitialize()
        {
            this.Q<AudioButton>("resume").clicked += Close;
            this.Q<AudioButton>("settings").clicked += Close;
            this.Q<AudioButton>("exit").clicked += () =>
            {
                NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
                SceneManager.LoadScene("menu");
            };
        }
    }
}
