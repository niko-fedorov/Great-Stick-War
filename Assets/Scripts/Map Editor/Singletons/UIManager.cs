using UnityEngine.Events;

namespace MapEditor
{
    public class UIManager : Singleton<UIManager>
    {
        public event UnityAction MenuOpened;
        public event UnityAction MenuClosed;

        public event UnityAction<string, Game.MapData> MapLoad;
        public event UnityAction<string, string> MapSave;

        //public event UnityAction Quit;

        //[Header("Map Editor UI Manager Settings")]

        //[SerializeField]
        //private InputField _mapNameInputField;

        //[SerializeField]
        //private MenuPanel _menuPanel;

        //protected override void Initialize()
        //{
        //GetComponentInChildren<ColorSelectPanel>().Selected += color => ColorSelected.Invoke(color);
        //GetComponentInChildren<ToolTypePanel>().Selected += tool => ToolTypeSelected.Invoke(tool);
        //GetComponentInChildren<SelectionTypePanel>().Selected += selection => SelectionTypeSelected.Invoke(selection);

        //_menuPanel.MapSave += path => MapSave.Invoke(path, _mapNameInputField.text);

        //_menuPanel.Quit += () =>
        //{
        //    _loadingPanel.SetActive(true);

        //    SceneManager.LoadScene(0);
        //};
        //}
    }
}