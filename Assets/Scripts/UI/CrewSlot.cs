using UnityEngine;
using UnityEngine.UI;

public class CrewSlot : MonoBehaviour
{
    [SerializeField]
    private Text _crewMemberNameText,
                 _seatIndexText;
                 
    public Text SeatIndexText => _seatIndexText;
    public Text CrewMemberNameText => _crewMemberNameText;
    [SerializeField]
    private Image _crewMemberIconImage,
                  _gunIconImage;
    public Image CrewMemberIconImage => _crewMemberIconImage;
    public Image GunIconImage => _gunIconImage;
}

