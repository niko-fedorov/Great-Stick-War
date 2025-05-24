using UI;
using UnityEngine.UIElements;

namespace MapEditor.UI
{
    [UxmlElement]
    public partial class ToolTypePanel : VisualElement, IInitializeable
    {
        //public event UnityAction<ToolType> Selected;

        public void Initialize()
        {
            //this.Query<AudioButton>().ForEach(button => button.clicked += () => Selected.Invoke(button.ToolType));
        }
    }
}
