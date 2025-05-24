using UI;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class SideSelectionPanel : Panel
    {
        protected override void OnInitialize()
        {
            var sideSelectionButtons = this.Query<SideSelectionButton>().ToList();
            for (int i = 0; i < sideSelectionButtons.Count; i++)
            {
                sideSelectionButtons[i].Initialize(GameData.Instance.GetSideData(ServerManager.Instance.ServerData.Sides[i]));
                sideSelectionButtons[i].clicked += () => SetEnabled(false);
            }
        }
    }
}
