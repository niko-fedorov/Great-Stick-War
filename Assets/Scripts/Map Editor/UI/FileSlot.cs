using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FileSlot : AudioButton
    {
        [SerializeField]
        private Text _nameText;

        public void Initialize(string name)
        {
            _nameText.text = name;
        }
    }
}

