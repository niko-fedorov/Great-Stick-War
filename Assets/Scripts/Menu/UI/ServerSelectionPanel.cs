using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UIElements;

namespace Menu.UI
{
    [UxmlElement]
    public partial class ServerSelectionPanel : Panel
    {
        protected override void OnInitialize()
        {
            //var adressInputField = this.Q<TextField>();
            //this.Q<AudioButton>("button_connect").clicked += () =>
            //{
            //    NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = adressInputField.text;
            //    NetworkManager.Singleton.StartClient();
            //};
        }
    }
}
