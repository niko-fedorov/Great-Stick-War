using UI;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class ClassSelectionPanel : Panel
    {
        public event UnityAction<SideData.ClassData> Selected;

        protected override void OnInitialize()
        {

        }

        public void Initialize(SideData sideData)
        {
            //var listView = this.Q<ListView>();

            //listView.itemsSource = sideData.ClassDatas;
            //listView.makeItem += () =>
            //{

            //};
            //listView.bindItem += (ve, index) =>
            //{

            //};


            //foreach (var classData in sideData.ClassDatas)
            //{
            //    var classSelectButton = Instantiate(_classSelectButtonPrefab, transform);
            //    classSelectButton.Initialize(classData);
            //    classSelectButton.Clicked += () =>
            //    {
            //        Selected.Invoke(classData);
            //    };
            //}
        }
    }
}

