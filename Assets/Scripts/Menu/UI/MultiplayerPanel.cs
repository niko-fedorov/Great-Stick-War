using Game;
using UI;
using Unity.Netcode;
using UnityEngine.UIElements;

namespace Menu.UI
{
    [UxmlElement]
    public partial class MultiplayerPanel : Panel
    {
        protected override void OnInitialize()
        {
            var creationPanel = this.Q<ServerCreationPanel>();
            this.Q<AudioButton>("create").clicked += () =>
            {
                ServerManager.Instance.ServerData = new ServerData(ResourceManager.Instance.Load<MapData>()[0], new SideType[2] { SideType.AustriaHungary, SideType.GermanEmpire });
                //SideDatabase.GetDatas()
                //.Where(data => _sideSelectDropdowns
                //.Select(side => side.options[side.value].text)
                //.Contains(data.Name))
                //.Select(data => data.Type)
                //.ToArray());

                NetworkManager.Singleton.StartHost();
            }; //creationPanel.SetEnabled(true);

            var selectionPanel = this.Q<ServerSelectionPanel>();
            //this.Q<AudioButton>("select").clicked += () => selectionPanel.SetEnabled(true);
        }
    }
}


