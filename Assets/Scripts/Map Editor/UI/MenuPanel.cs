using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace MapEditor.UI
{
    [UxmlElement]
    public partial class MenuPanel : Panel
    {
        public event UnityAction<string> MapLoad;
        public event UnityAction<string> MapSave;

        public event UnityAction Quit;

        [SerializeField]
        private string _newActionText,
                       _exitActionText;

        protected override void OnInitialize()
        {
            var actionPanel = this.Q<ActionPanel>();
            this.Q<AudioButton>("button_new").clicked += () =>
            {
                actionPanel.SetEnabled(true);
                //actionPanel.Initialize(_newActionText);
            };

            var loadPanel = this.Q<ActionPanel>();
            this.Q<AudioButton>("button_load").clicked += () =>
            {
                loadPanel.SetEnabled(true);
            };

            var savePanel = this.Q<ActionPanel>();
            this.Q<AudioButton>("button_save").clicked += () =>
            {
                savePanel.SetEnabled(true);
            };

            this.Q<AudioButton>("button_exit").clicked += () =>
            {
                actionPanel.SetEnabled(true);
                //actionPanel.Initialize(_exitActionText);
                //actionPanel.Action += Quit.Invoke;
            };

            //_loadFilePanel.File += MapLoad;
            //_saveFilePanel.File += MapSave;
        }
    }
}

