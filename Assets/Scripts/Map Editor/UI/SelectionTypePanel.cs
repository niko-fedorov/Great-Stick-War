using UI;
using UnityEngine.UIElements;

namespace MapEditor.UI
{
    [UxmlElement]
    public partial class SelectionTypePanel : VisualElement, IInitializeable
    {
        //public event UnityAction<SelectionType> Selected;

        public void Initialize()
        {
            //this.Query<AudioButton>().ForEach(button => button.clicked += () => Selected.Invoke(button.SelectionType);
        }
    }
}
