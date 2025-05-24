using UnityEngine.UIElements;

namespace UI
{
    [UxmlElement]
    public partial class AudioButton : Button, IInitializeable
    {
        [UxmlAttribute("Selectable")]
        private bool _isSelectable;
        public bool IsSelectable => _isSelectable;

        public void Initialize()
        {
            clicked += () => UIManager.Instance.PlayAudio();

            OnInitialize();
        }

        protected virtual void OnInitialize() { }
    }
}
