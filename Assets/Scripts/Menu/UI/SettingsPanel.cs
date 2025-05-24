using UI;
using UnityEngine.UIElements;

namespace Menu.UI
{
    [UxmlElement]
    public partial class SettingsPanel : Panel
    {
        protected override void OnInitialize()
        {
            var languadeDropdown = this.Q<DropdownField>();

            //var 
            //_sideSelectDropdowns = new Dropdown[2];
            //for (int i = 0; i < 2; i++)
            //{
            //    _sideSelectDropdowns[i] = Instantiate(_sideSelectDropdownPrefab, _sideSelectPanel.transform);
            //    foreach (var side in SideDatabase.GetDatas())
            //        _sideSelectDropdowns[i].options.Add(new Dropdown.OptionData(side.Name, side.Icon));
            //}

            //foreach (var mapData in ResourceManager.Instance.Load<MapData>())
            //{
            //    var map = Instantiate(_mapSlotPrefab, _mapsScrollView.content);
            //    map.Initialize(mapData);

            //    map.Clicked += () => _map = mapData;
            //}

            //this.Q<AudioButton>("create").clicked += () =>
            //{
            //    ServerManager.Instance.ServerData.Value = new ServerData
            //        (ResourceManager.Instance.Load<MapData>()[0], new SideType[2] {SideType.AustriaHungary, SideType.GermanEmpire});
            //        //SideDatabase.GetDatas()
            //            //.Where(data => _sideSelectDropdowns
            //            //.Select(side => side.options[side.value].text)
            //            //.Contains(data.Name))
            //            //.Select(data => data.Type)
            //            //.ToArray());

            //    NetworkManager.Singleton.StartHost();
            //};
        }
    }
}
