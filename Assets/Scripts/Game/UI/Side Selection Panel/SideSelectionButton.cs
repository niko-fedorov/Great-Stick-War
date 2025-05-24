using UI;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class SideSelectionButton : AudioButton
    {
        public void Initialize(SideData sideData)
        {
            clicked += () => GameManager.Instance.GetPlayer().SelectSideServerRpc(sideData.Type);
            text = sideData.Name;
            //iconImage = sideData.Icon;
        }
    }
}
