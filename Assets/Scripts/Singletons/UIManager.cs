using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Text _actionText,
                 _gunNameText,
                 _gunAmmoText;
    public Text ActionText => _actionText;
    public Text GunNameText => _gunNameText;
    public Text GunAmmoText => _gunAmmoText;
}
