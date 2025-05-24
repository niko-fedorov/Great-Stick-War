using UI;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class SidePlayerTable : Panel
    {
        protected override void OnInitialize()
        {
            //foreach (var side in ServerManager.Instance.ServerData.Sides)
            //    GetComponentInChildren<SideTableSlot>().Initialize(side);
        }
    }
}
