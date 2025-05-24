using UI;
using UnityEngine.UIElements;

namespace Menu.UI
{
    [UxmlElement]
    public partial class MapSelectionButton : AudioButton
    {
        public void Initialize(Game.MapData mapData)
        {
            this.Q<Label>().text = mapData.Name;
            this.Q<Image>().sprite = mapData.Icon;
        }
    }
}