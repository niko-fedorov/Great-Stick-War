using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class Panel : VisualElement, IInitializeable
    {
        public static event UnityAction<Panel> Opened;

        public void Initialize()
        {
            var closeButton = this.Q<AudioButton>("close");
            if (closeButton != null)
                closeButton.clicked += Close;

            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void Open()
        {
            SetEnabled(true);

            Opened.Invoke(this);
        }

        public void Close()
        {
            SetEnabled(false);
        }
    }
}

