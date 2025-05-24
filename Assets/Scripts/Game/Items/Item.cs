using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(AudioSource), typeof(NetworkTransform), typeof(Rigidbody))]
    public abstract class Item : NetworkBehaviour, IInteractable
    {
        #region Variables

        #region Events

        public delegate void ActivatedEvent(float recoil, float spread);
        public event ActivatedEvent Activated = delegate { };

        public event Action Reload;

        #endregion

        [SerializeField]
        private ItemData _data;
        public ItemData Data => _data;

        public string Name => _data.Name;
        public bool IsInteractable { get; private set; }
        public string ActionText => Localization.GetString("item_action");
        //public Color TextColor => GameManager.Instance.GetActionColor(ActionType.Damage);

        [Serializable]
        public struct SightData
        {
            [SerializeField]
            private Transform _object;
            public Vector3 Point => _object.position;

            [Range(1, 8)]
            [SerializeField]
            private float _scopeValue;
            public float ScopeValue => _scopeValue;
        }
        [SerializeField]
        private SightData _sight;
        public SightData Sight => _sight;

        #region Components

        protected AudioSource _audioSource;
        public Collider Collider { get; private set; }
        public NetworkTransform NetworkTransform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        #endregion

        #endregion

        #region Methods

        public void Initialize()
        {
            _audioSource = GetComponent<AudioSource>();
            Collider = GetComponent<Collider>();
            NetworkTransform = GetComponent<NetworkTransform>();
            Rigidbody = GetComponent<Rigidbody>();
        }


        public abstract void Interact(Player player);

        public enum ActivationType
        {
            Single,
            Continuous
        }
        public abstract void Activate(Vector3 origin, Vector3 direction, ActivationType type);
        
        public void OnActivate()
            => Activated.Invoke(0, 0);

        #endregion
    }
}
