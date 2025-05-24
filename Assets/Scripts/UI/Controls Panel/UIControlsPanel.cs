using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ControlsPanel : MonoBehaviour
    {
        public event UnityAction<ControlActionType, KeyCode> KeyChanged;

        //[SerializeField]
        //private LayoutGroup _controlsLayoutGroup;

        //[SerializeField]
        //private ControlSlot _controlSlotPrefab;

        //[SerializeField]
        //private GameObject _keySelectPanel;

        //private void Start()
        //{
        //    foreach (var control in ControlManager.Instance.ControlsData.Controls)
        //    {
        //        var controlSlot = Instantiate(_controlSlotPrefab, _controlsLayoutGroup.transform);
        //        controlSlot.Initialize(control);

        //        controlSlot.KeyChange += () =>
        //        {
        //            _keySelectPanel.SetActive(true);

        //            ControlManager.Instance.AnyKeyPressed += (key) =>
        //            {
        //                KeyChanged.Invoke(control.Type, key);

        //                _keySelectPanel.SetActive(false);
        //            };
        //        };
        //    }
        //}
    }
}
