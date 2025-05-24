using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Game
{
    public enum SlotType
    {
        Primary,
        Sidearm
    }

    [CreateAssetMenu]
    public class ItemData : ScriptableObject
    {
        [SerializeField]
        private SlotType _slotType;
        public SlotType SlotType => _slotType;

        [SerializeField]
        private LocalizedString _name;
        public string Name => _name.GetLocalizedString();

        [SerializeField]
        private float _scopeValue,
                      _scopeSpeed;
        public float ScopeValue => _scopeValue;
        public float ScopeSpeed => _scopeSpeed;

        [Serializable]
        public struct RecoilData
        {
            [SerializeField]
            private Vector2 _value;
            public Vector2 Value => _value;

            [SerializeField]
            private float _firstShotMultiplier,
                          _decreasePerSecond;
            public float FirstShotMultiplier => _firstShotMultiplier;
            public float DecreasePerSecond => _decreasePerSecond;
        }
        [Header("Recoil Settings")]
        [SerializeField]
        private RecoilData _recoil;
        public RecoilData Recoil => _recoil;

        [Serializable]
        public struct SpreadData
        {
            public enum Types
            {
                Hipfire,
                Zoomed
            }
            [Serializable]
            public struct TypeData
            {
                [SerializeField]
                private Types _type;
                public Types Type => _type;

                [SerializeField]
                private float _static,
                              _moving;
                public float Static => _static;
                public float Moving => _moving;
            }
            [SerializeField]
            private TypeData[] _typeDatas;
            public TypeData GetTypeData(Types type)
                => _typeDatas.First(typeData => typeData.Type == type);

            [SerializeField]
            private float _increasePerBullet,
                          _decreasePerSecond;
            public float IncreasePerBullet => _increasePerBullet;
            public float DecreasePerSecond => _decreasePerSecond;
        }
        [Header("Spread Settings")]
        [SerializeField]
        private SpreadData _spread;
        public SpreadData Spread => _spread;

        [SerializeField]
        private bool _isDropable;
        public bool IsDropable => _isDropable;

        [SerializeField]
        private float _ownerSpeedMultiplier = 1;
        public float OwnerSpeedMultiplier => _ownerSpeedMultiplier;
    }
}
