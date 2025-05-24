using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class PlayerControllerPanel : VisualElement, IInitializeable
    {
        #region Methods

        public void Initialize()
        {
            var player = GameManager.Instance.GetPlayer();
            player.ControllerSpawned += controller =>
            {
                var healthProgressBar = this.Q<ProgressBar>("health");

                var itemNameLabel = this.Q<Label>("name");
                var ammoLabel = this.Q<Label>("ammo");

                var vignette = this.Q("vignette");

                var vehicleControllerPanel = this.Q<VehicleControllerPanel>();

                var actionLabel = this.Q < Label>("action");
                controller.Interacted += interactable =>
                {
                    actionLabel.text = interactable == null ? "" : interactable.ActionText;
                };
                controller.ItemChanged += (previousItem, newItem) =>
                {
                    itemNameLabel.text = newItem.Data.Name;

                    var gun = previousItem as Gun;
                    if (gun)
                        gun.AmmoChanged -= ChangeAmmoLabelText;
                    gun = newItem as Gun;
                    if (gun)
                        gun.AmmoChanged += ChangeAmmoLabelText;

                    void ChangeAmmoLabelText(int ammo, int clipAmmo)
                        => ammoLabel.text = $"{clipAmmo}/{gun.Data.ClipAmmo} | {ammo}";
                };
                controller.VehicleEntered += vehicle =>
                {
                    vehicleControllerPanel.SetEnabled(true);
                    vehicleControllerPanel.Initialize(vehicle);
                };
                controller.VehicleLeft += () =>
                {
                    vehicleControllerPanel.SetEnabled(false);
                };
                controller.Damaged += () =>
                {
                    vignette.style.opacity = vignette.style.opacity.value + 10;

                    healthProgressBar.value = controller.Health;
                };
                controller.Died += origin =>
                {
                    this.Q<VisualElement>("panel_death").SetEnabled(true);
                };

                var blindness = this.Q("blindness");
                //player.Blinded += duration 
                //    => UIManager.Instance.StartTimer(duration, )

                //while (true)
                //{
                //    if (Input.GetKeyDown(KeyCode.Escape))
                //    {
                //        if (_pausePanel.activeSelf)
                //        {
                //            Time.timeScale = 1;

                //            UIManager.Instance.ChangeCursorState(global::UIManager.CursorState.Disabled);

                //            _currentPanel = null;

                //            _pausePanel.SetActive(false);
                //        }
                //        else if (!_currentPanel)
                //        {
                //            Time.timeScale = 0;

                //            Cursor.SetEnabled(true);
                //            Cursor.lockState = CursorLockMode.None;

                //            _currentPanel = _pausePanel;

                //            _pausePanel.SetActive(true);
                //        }
                //    }
                //    if (Input.GetKeyDown(KeyCode.T))
                //        _chatInputField.ActivateInputField();

                //    if (Input.GetKeyDown(KeyCode.M))
                //    {
                //        _mapPanel.gameObject.SetActive(true);
                //    }

                //    if (_vignetteImage.color.a != 0)
                //        _vignetteImage.color = new Color(0, 0, 0, Mathf.SmoothDamp(_vignetteImage.color.a, 0, ref _vignetteCurrentVelocity, _damageTime));
                //}
            };
        }
    }

    #endregion
}