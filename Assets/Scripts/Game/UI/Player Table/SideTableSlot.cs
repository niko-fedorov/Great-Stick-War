using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SideTableSlot : MonoBehaviour
    {
        [SerializeField]
        private Image _iconImage;
        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private VerticalLayoutGroup _playerTable;

        private PlayerTableSlot _playerTableSlotPrefab;

        public void Initialize(SideType type)
        {
            var side = GameData.Instance.GetSideData(type);

            _iconImage.sprite = side.Icon;
            _nameText.text = side.Name;

            _playerTableSlotPrefab = GetComponent<PlayerTableSlot>();
            _playerTableSlotPrefab.gameObject.SetActive(false);

            foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.None).Where(player => player.SideType == type))
                Instantiate(_playerTableSlotPrefab, _playerTable.transform).Initialize(player);
        }
    }
}

