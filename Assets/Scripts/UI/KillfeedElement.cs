using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class KillfeedElement : MonoBehaviour
    {
        [SerializeField]
        private Image _sideImage;
        [SerializeField]
        private Text _text;

        public void Initialize(Player killer, Player victim)
        {
            _text.text = $"{killer.Name}    {Localization.GetString("KLL.")}    {victim.Name}";
        }
    }
}
