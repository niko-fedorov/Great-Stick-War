using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ControlSlot : MonoBehaviour
    {
        public event UnityAction KeyChange;

        //[SerializeField]
        //private Text _actionNameText;
        //[SerializeField]
        //private AudioButton _keySelectButton;

        //public void Initialize(ControlData data)
        //{
        //    _actionNameText.text = data.Name;
        //    _keySelectButton.Label.text = data.Key.ToString();

        //    _keySelectButton.Clicked += KeyChange.Invoke;
        //}
    }
}

