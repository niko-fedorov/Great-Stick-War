using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Menu.UI
{
    [UxmlElement]
    public partial class Menu : VisualElement, IInitializeable
    {
        public void Initialize()
        {
            this.Q<AudioButton>("singleplayer").clicked += ()
                =>
            { Debug.Log('+'); };

            var multiplayerPanel = this.Q<MultiplayerPanel>();
            this.Q<AudioButton>("multiplayer").clicked += ()
                => multiplayerPanel.Open();

            this.Q<AudioButton>("editor").clicked += () =>
            {
                this.Q<LoadingPanel>().SetEnabled(true);

                SceneManager.LoadSceneAsync(2);
            };

            this.Q<AudioButton>("exit").clicked += ()
                => UIManager.Instance.OpenActionPanel(Localization.GetString("Are you sure you want to exit?"), () => Application.Quit());
        }
    }
}
