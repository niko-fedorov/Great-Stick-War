using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [RequireComponent(typeof(AudioSource), typeof(Collider), typeof(Rigidbody))]
    public class Vehicle : NetworkBehaviour, IInteractable, IDamageable
    {
        #region Variables

        #region Events

        public event UnityAction<Player, Seat> SeatChanged;
        public event UnityAction Damaged;
        public event UnityAction Died;

        #endregion

        [SerializeField]
        private VehicleControllerData _data;
        public VehicleControllerData Data => _data;

        private NetworkVariable<int> _health => new NetworkVariable<int>(_data.Health);
        public int Health => _health.Value;

        [SerializeField]
        protected float _cameraOffset;
        public float CameraOffset => _cameraOffset;

        public string ActionText => "сесть";
        public string Name => _data.Name;
        public bool IsInteractable => throw new NotImplementedException();

        [Serializable]
        public class Seat
        {
            [SerializeField]
            private Transform _transform;
            public Transform Transform => _transform;

            [SerializeField]
            private VehicleGun _gun;
            public VehicleGun Gun => _gun;

            private Player _player;
            public Player Player
            {
                get => _player;
                set
                {
                    if (!_player)
                    {
                        _player = value;
                        _player.Disconnected += () => _player = null;
                    }
                }
            }
        }
        [SerializeField]
        protected Seat[] _seats;
        public Seat GetFreeSeat()
            => _seats.FirstOrDefault(seat => !seat.Player);

        #region Components

        private AudioSource _audioSource;

        #endregion

        #endregion

        #region Methods

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (IsOwner)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    LeaveServerRpc();
            }
        }

        public void Interact(Player player)
        {
            var seat = GetFreeSeat();
            if(seat != null)
                InteractClientRpc(player.OwnerClientId, Array.IndexOf(_seats, seat));
        }
        [ClientRpc]
        private void InteractClientRpc(ulong playerId, int seatIndex)
            => SeatChanged.Invoke(GameManager.Instance.GetPlayer(playerId), _seats[seatIndex]);

        [ServerRpc]
        private void LeaveServerRpc()
        {
            //MoveServerRpc(_vehicle.transform.TransformPoint(_vehicle.Data.Size));
        }

        [ClientRpc]
        private void PlaySoundClientRpc(ControllerData.Sound.Types type)
        {
            _audioSource.PlayOneShot(_data.GetSound(type));
        }

        public void Damage(int damage, RaycastHit hit)
        {
            _health.Value -= damage;
            if (_health.Value <= 0) ;
            //Destroy();

            PlaySoundClientRpc(ControllerData.Sound.Types.Damage);
        }

        #endregion
    }
}
