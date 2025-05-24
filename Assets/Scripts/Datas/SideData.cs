using Game;
using UnityEngine;
using UnityEngine.Localization;

public enum SideType
{
    AustriaHungary,
    Britain,
    France,
    GermanEmpire,
    OttomanEmpire,
    RussianEmpire
}

[CreateAssetMenu(fileName = "NewSideData", menuName = "Scriptable Object/Side Data")]
public class SideData : ScriptableObject
{
    [SerializeField]
    private SideType _type;
    public SideType Type => _type;

    [SerializeField]
    private LocalizedString _name;
    public string Name => _name.GetLocalizedString();

    [SerializeField]
    private Sprite _icon;
    public Sprite Icon => _icon;

    [SerializeField]
    private Color _color;
    public Color Color => _color;

    [System.Serializable]
    public struct ClassData
    {
        [SerializeField]
        private ClassType _type;
        public ClassType Type => _type;

        [System.Serializable]
        public struct GunSlot
        {
            [SerializeField]
            private SlotType _slotType;
            public SlotType SlotType => _slotType;

            [SerializeField]
            private GunData _gunData;
            public GunData GunData => _gunData;
        }
        [SerializeField]
        private GunSlot[] _gunSlots;
        public GunSlot[] GunSlots => _gunSlots;
    }
    [SerializeField]
    private ClassData[] _classDatas;
    public ClassData[] ClassDatas => _classDatas;
}
