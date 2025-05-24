using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    using UI;

    public class UIManager : global::UIManager
    {
        public static new UIManager Instance => global::UIManager.Instance as UIManager;

        [SerializeField]
        private Colors _colors;
        public Colors Colors => _colors;

        //[SerializeField]
        //private SidePlayerTable _sidePlayerTable;

        //protected override void OnUpdate()
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        _sidePlayerTable.gameObject.SetActive(true);
        //    }
        //    else if (Input.GetKeyUp(KeyCode.Tab))
        //    {
        //        _sidePlayerTable.gameObject.SetActive(false);
        //    }
        //}

        protected override void OnStart()
        {
            var playerControllerPanel = _UIDocument.rootVisualElement.Q<PlayerControllerPanel>();
            var vehicleControllerPanel = _UIDocument.rootVisualElement.Q<VehicleControllerPanel>();

            GameManager.Instance.GetPlayer().ControllerSpawned += controller =>
            {
                void OnLive(bool value)
                {
                    SetCursorActive(!value);

                    _audioListener.enabled = !value;
                    _musicAudioSource.Stop();

                    playerControllerPanel.SetEnabled(value);
                }

                OnLive(true);
                controller.Died += killer => OnLive(false);

                controller.VehicleEntered += vehicleControllerPanel.Initialize;
                controller.VehicleLeft += () =>
                {
                    playerControllerPanel.SetEnabled(true);
                    vehicleControllerPanel.SetEnabled(false);
                };
            };

            //_UIDocument.rootVisualElement.Q<SideSelectionPanel>().
        }

        public void Blind(float duration)
        {

        }
    }
}
