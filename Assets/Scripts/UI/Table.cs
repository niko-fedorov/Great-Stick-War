using System.Collections;
using UnityEngine.UIElements;

namespace UI
{
    [UxmlElement]
    public partial class Table : ListView
    {
        public void Initialize(IList itemsSource)
        {
            //var listView = this.Q<ListView>();
            //listView.itemsSource = itemsSource;
            //listView.makeItem += () =>
            //{
            //    var sideSelectionButton = listView.itemTemplate.Instantiate();
            //    this.Add(sideSelectionButton);

            //    return sideSelectionButton;
            //};
            //listView.bindItem += (ve, index) =>
            //{
            //    var side = (SideType)listView.itemsSource[index];
            //    ve.Q<Button>().clicked += () =>
            //    {
            //        Player.Instance.SelectSideServerRpc(side);
            //    };
            //};
        }
    }
}
