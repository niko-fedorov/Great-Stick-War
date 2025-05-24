using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UI
{
    [UxmlElement]
    public partial class ActionPanel : Panel
    {
        public event UnityAction Action;

        protected override void OnInitialize()
        {
            this.Q<AudioButton>("positive").clicked += Action.Invoke;
            this.Q<AudioButton>("negative").clicked += () => SetEnabled(false);
        }
    }
}

