using UnityEngine;

namespace Game
{
    public enum ClassType
    {
        Rifler,
        Engineer,
        Support,
        Sniper
    }

    [CreateAssetMenu]
    public class ClassData : ScriptableObject
    {
        [SerializeField]
        private ClassType _type;
        public ClassType Type => _type;

        [SerializeField]
        private string _name;
        public string Name => _name;

        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField]
        private Item[] _itemPrefabs;
        public Item[] ItemPrefabs => _itemPrefabs;
    }
}
