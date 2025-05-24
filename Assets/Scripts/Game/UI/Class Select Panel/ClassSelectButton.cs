using UI;
using UnityEngine.UIElements;

namespace Game
{
    namespace UI
    {
        [UxmlElement]
        public partial class ClassSelectButton : global::UI.AudioButton
        {
            public void Initialize(SideData.ClassData classData)
            {
                this.Q<TextElement>("text_class").text = classData.Type.ToString();
                //this.Q<Image>("image_class").sprite = classData.Icon;
                this.Q<ListView>("list-view_class-datas");
            }
        }
    }
}
