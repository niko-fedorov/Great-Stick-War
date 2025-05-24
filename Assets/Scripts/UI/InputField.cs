using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AudioInputField : MonoBehaviour
    {
        public string Text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        private InputField _inputField;
        private Outline _outline;

        private void Start()
        {
            _inputField = GetComponent<InputField>();
            _outline = GetComponent<Outline>();
        }

        public void SetOutlineColor(Color color)
        {
            _outline.effectColor = color;
        }
    }
}

