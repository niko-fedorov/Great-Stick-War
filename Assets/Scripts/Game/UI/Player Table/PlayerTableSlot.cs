using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    namespace UI
    {
        public class PlayerTableSlot : MonoBehaviour
        {
            [SerializeField]
            private Text _nameText,
                         _killValueText,
                         _deathValueText;

            [SerializeField]
            private Image _iconImage;

            private Player _player;

            public void Initialize(Player player)
            {
                gameObject.SetActive(true);

                _nameText.text = player.Name;

                _player = player;
            }

            private void Start()
            {
                _nameText.text = _player.Name;
                _killValueText.text = _player.Kills.ToString();
                _deathValueText.text = _player.Deaths.ToString();
            }

            private void OnEnable()
            {

            }
        }
    }
}
