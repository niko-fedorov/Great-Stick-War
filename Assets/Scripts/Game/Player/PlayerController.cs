using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [RequireComponent(typeof(AudioSource), typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour, IDamageable
    {
        #region Variables

        #region Events

        public event UnityAction<float> Blinded;
        public event UnityAction Damaged;
        public event UnityAction<Player> Died;

        public event UnityAction<IInteractable> Interacted;
        public event UnityAction<Item, Item> ItemChanged;
        public event UnityAction<Vehicle> VehicleEntered;
        public event UnityAction VehicleLeft;

        #endregion

        [SerializeField]
        private PlayerControllerData _data;

        #region Item

        [SerializeField]
        private Transform _itemPivot;
        public Transform ItemPivot => _itemPivot;
        private Animator _armsAnimator;

        [System.Serializable]
        public struct InventorySlot
        {
            [SerializeField]
            private SlotType _slotType;
            public SlotType SlotType => _slotType;

            [SerializeField]
            private Transform _pivot;
            public Transform Pivot => _pivot;
        }
        [SerializeField]
        private InventorySlot[] _inventorySlots;
        public InventorySlot GetInventorySlot(SlotType slotType)
            => _inventorySlots.First(slot => slot.SlotType == slotType);

        #endregion

        #region Network

        private NetworkVariable<int> _health = new NetworkVariable<int>(100);
        public int Health => _health.Value;

        #region Items

        private int _itemIndex;
        private Item _item;
        public Item Item => _item;

        private Item[] _items = new Item[System.Enum.GetValues(typeof(SlotType)).Length];
        private Item[] Items => _items;
        //{
        //    get
        //    {
        //        var items = new Item[_items.Count];
        //        for (int i = 0; i < _items.Count; i++)
        //            if (_items[i].TryGet(out Item item))
        //                items[i] = item;
        //        return items;//.OrderBy(item => item.SlotType).ToArray();
        //    }
        //}

        #endregion

        #endregion

        private bool _isActive;

        private float _speed,
                      _velocity,
                      _angle,
                      _recoil,
                      _spread;

        private Vector3 _targetRotation,
                        _viewCameraPoint,
                        _itemCameraPoint,
                        _moveDirection,
                        _rotationDirection,
                        _rotationVelocity,
                        _currentVelocity,
                        _clampRotation,
                        _aimCurrentVelocity;

        private Player _player;

        private Coroutine _moving;

        private PlayerControllerData.State _state;

        private NetworkBehaviour _interactable;
        private MountGun _mountGun;
        private Vehicle _vehicle;

        #region Components

        [Header("Components")]
        [SerializeField]
        private AudioSource _damageAudioSource,
                            _itemAudioSource,
                            _movementAudioSource;
        [SerializeField]
        private Transform _model;
        public Transform Model => _model;
        [SerializeField]
        private MeshRenderer _renderer;

        [SerializeField]
        private Camera _viewCamera,
                       _itemCamera;

        private AudioListener _audioListener;
        private CharacterController _characterController;

        #endregion

        #endregion

        #region Methods

        private void Start()
        {
            _audioListener = GetComponentInChildren<AudioListener>();
            _characterController = GetComponent<CharacterController>();

            _viewCameraPoint = _viewCamera.transform.localPosition;
            _itemCameraPoint = _itemCamera.transform.localPosition;

            _armsAnimator = _itemPivot.GetComponentInChildren<Animator>();

            //_renderer.material.color = SideDatabase.GetData(ServerManager.Instance.PlayerData.).Color;

            if (IsServer)
            {
                _player = GameManager.Instance.GetPlayer(OwnerClientId);

                var side = _player.SideType;//GameManager.Instance.GetPlayer(OwnerClientId).Team.Value;
                var classData = _player.ClassData;

                foreach (var itemPrefab in classData.ItemPrefabs)
                {
                    var item = Instantiate(itemPrefab);
                    item.NetworkObject.SpawnWithOwnership(OwnerClientId);
                    item.Initialize();
                    ChangeItemState(item, ChangeItemStates.Take);
                }

                _item = _items.Last(item => item);
            }

            InitializeServerRpc();

            if (!IsOwner)
                _audioListener.enabled = _viewCamera.enabled = _itemCamera.enabled = false;
        }

        [ServerRpc]
        private void InitializeServerRpc()
            => InitilizeClientRpc(_item, _items.Select(item => (NetworkBehaviourReference)item).ToArray());
        [ClientRpc]
        private void InitilizeClientRpc(NetworkBehaviourReference item, NetworkBehaviourReference[] items)
        {
            for (int i = 0; i < items.Length; i++)
                ChangeItemStateClientRpc(items[i], ChangeItemStates.Take);
            SelectItemClientRpc(null, item);
        }

        private void Update()
        {
            if (IsServer)
            {
                #region Movement

                if (!_characterController.isGrounded || _velocity > 0)
                {
                    _velocity += Physics.gravity.y * Time.deltaTime;
                    _characterController.Move(new Vector3(0, _velocity * Time.deltaTime, 0));
                }
                else if (_velocity < 0)
                {
                    var damage = (int)_data.VelocityDamageAnimationCurve.Evaluate(_velocity);
                    if (damage > 0)
                        Damage(damage);

                    ChangeStateServerRpc(PlayerControllerData.State.Types.Idle);

                    //_networkAudioSource.Play(_data.GetSound(ControllerData.Sound.Types.Land));

                    _velocity = 0;
                }

                #endregion

                #region Item

                #region Interact

                if (Physics.Raycast(_viewCamera.transform.position, _viewCamera.transform.forward, out var hit, _data.ActionDistance))
                {
                    var interactable = hit.transform.GetComponent<NetworkBehaviour>();
                    if (interactable && interactable.GetComponent<IInteractable>() != null && interactable != _interactable)
                        InteractClientRpc(_interactable = interactable);
                }
                else if (_interactable != null)
                    InteractClientRpc(_interactable = null);

                #endregion

                #region Recoil & Spread

                _recoil = Mathf.Max(_recoil - _item.Data.Recoil.DecreasePerSecond * Time.deltaTime, 0);
                _spread = Mathf.Max(_spread - _item.Data.Spread.DecreasePerSecond * Time.deltaTime, 0);

                #endregion

                #endregion
            }

            if (IsOwner && GameManager.Instance.IsActive)
            {
                #region Transform

                var movement = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1);
                if (movement.magnitude > 0)
                {
                    if (Input.GetKeyUp(KeyCode.LeftControl))
                        ChangeStateServerRpc(PlayerControllerData.State.Types.Crouch);
                    else if (Input.GetKey(KeyCode.LeftShift))
                        ChangeStateServerRpc(PlayerControllerData.State.Types.Run);
                    else
                        ChangeStateServerRpc(PlayerControllerData.State.Types.Walk);

                    MoveServerRpc(movement, NetworkManager.LocalTime.TimeAsFloat);
                }
                else
                    ChangeStateServerRpc(PlayerControllerData.State.Types.Idle);

                RotateServerRpc(new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * _data.Sensitivity, NetworkManager.LocalTime.TimeAsFloat);

                //_viewCamera.transform.localRotation = Quaternion.Euler(_angle.Value, 0, 0);

                if (Input.GetKeyDown(KeyCode.Space))
                    JumpServerRpc();

                #endregion

                #region Item

                if (Input.GetMouseButtonDown(0))
                    ActivateItemServerRpc(Item.ActivationType.Single);
                else if (Input.GetMouseButton(0))
                    ActivateItemServerRpc(Item.ActivationType.Continuous);

                if (Input.GetKeyDown(KeyCode.Mouse1))
                    ScopeServerRpc(true);
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                    ScopeServerRpc(false);

                if (Input.GetKeyDown(KeyCode.E))
                    InteractServerRpc();

                for (int i = 0; i < _inventorySlots.Length; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                        SelectItemServerRpc((SlotType)i);
                }

                int mouseScroll = (int)(Input.GetAxis("Mouse ScrollWheel") * -10);
                if (mouseScroll != 0)
                    SelectItemServerRpc(mouseScroll);

                //if (Input.GetKeyDown(KeyCode.G))
                //    ChangeItemState(item, ChangeItemStates.Drop);

                #endregion
            }

            _itemPivot.localPosition = Vector3.up * Mathf.Sin(Time.time / _data.Period * Mathf.PI * 2) * _data.Offset;
        }

        #region Render

        public enum Layers
        {
            Default,
            Hand
        }

        private void ChangeLayer(GameObject parent, Layers layer)
        {
            parent.layer = LayerMask.NameToLayer(layer.ToString());
            foreach (Transform child in parent.transform)
                ChangeLayer(child.gameObject, layer);
        }

        public void ChangeModelState(Layers layer)
        {
            _viewCamera.enabled = _itemCamera.enabled = _audioListener.enabled = layer switch
            {
                Layers.Hand => true,
                Layers.Default => false
            };

            foreach (var item in Items)
                ChangeLayer(item.gameObject, layer);
        }

        #endregion

        #region Movement

        [ServerRpc(RequireOwnership = false)]
        private void MoveServerRpc(Vector3 movement, float time = 0)
        {
            _characterController.Move(transform.TransformDirection(
                movement
                * _state.Speed
                * (Item ? Item.Data.OwnerSpeedMultiplier : 1))
                //    * (_isScoping ? _data.ScopeSpeedMultiplier : 1) : 1))
                * (IsServer ? Time.deltaTime : NetworkManager.ServerTime.TimeAsFloat - time));

            MoveClientRpc(transform.position);
        }
        [ClientRpc]
        private void MoveClientRpc(Vector3 position)
        {
            transform.position = position;

            if (_moving == null)
            {
                _moving = this.StartTimer(_data.GetSound(ControllerData.Sound.Types.Move).length * _data.GetState(PlayerControllerData.State.Types.Walk).Speed / _state.Speed * _data.MovementAudioDuration, () => _moving = null);
                _movementAudioSource.PlayOneShot(_data.GetSound(ControllerData.Sound.Types.Move));
            }
        }

        [ServerRpc]
        private void RotateServerRpc(Vector2 angle, float time = 0)
        {
            angle *= IsServer ? Time.deltaTime : NetworkManager.ServerTime.TimeAsFloat - time;
            _angle = Mathf.Clamp(_angle + angle.y, -90, 90);
            RotateClientRpc(transform.eulerAngles.y + angle.x, Mathf.Clamp(_angle - _recoil, -90, 90));
        }
        [ClientRpc]
        private void RotateClientRpc(float xAngle, float yAngle)
        {
            transform.eulerAngles = Vector3.up * xAngle;
            _itemPivot.localEulerAngles = _viewCamera.transform.localEulerAngles = Vector3.right * yAngle;
        }

        [ServerRpc]
        private void ChangeStateServerRpc(PlayerControllerData.State.Types state)
        {
            switch (state)
            {
                case PlayerControllerData.State.Types.Crouch:
                    _characterController.center /= 2;
                    _characterController.height /= 2;
                    _model.localScale = new Vector3(.25f, 1, .25f);
                    break;
            }
            //if (state == PlayerControllerData.State.Types.Run && (_state.Type == PlayerControllerData.State.Types.Idle || _state.Type == PlayerControllerData.State.Types.Walk))
            _state = _data.GetState(state);
        }

        [ServerRpc]
        private void CrouchServerRpc()
        {
            _characterController.center /= 2;
            _characterController.height /= 2;
            _model.localScale = new Vector3(.25f, 1, .25f);
        }

        [ServerRpc]
        private void JumpServerRpc()
        {
            if (_characterController.isGrounded)
            {
                _velocity = _data.JumpHeight - Physics.gravity.y / 2;
                JumpClientRpc();
            }
        }
        [ClientRpc]
        private void JumpClientRpc()
        {
            //_audioSource.PlayOneShot(_jumpSo);
        }

        #endregion

        #region Item

        #region Interact

        [ServerRpc]
        private void InteractServerRpc()
        {
            if (_interactable != null)
            {
                (_interactable as IInteractable).Interact(_player);
            }
        }
        [ClientRpc]
        private void InteractClientRpc(NetworkBehaviourReference interactable)
            => Interacted.Invoke(interactable.TryGet(out NetworkBehaviour interactableObject) ? interactableObject.GetComponent<IInteractable>() : null);

        #endregion

        #region Change State

        public enum ChangeItemStates
        {
            Take,
            Drop
        }

        private void ChangeItemState(Item item, ChangeItemStates state)
        {
            void OnItemActivated(float recoil, float spread)
            {
                _recoil += item.Data.Recoil.Value.y;
                _spread += item.Data.Spread.IncreasePerBullet;
            }

            switch (state)
            {
                case ChangeItemStates.Take:

                    item.Activated += OnItemActivated;

                    _items[(int)item.Data.SlotType] = item;

                    item.NetworkObject.ChangeOwnership(OwnerClientId);

                    break;

                case ChangeItemStates.Drop:

                    item.Activated -= OnItemActivated;

                    _items[(int)item.Data.SlotType] = null;

                    item.NetworkObject.RemoveOwnership();

                    item.Rigidbody.AddForce(_viewCamera.transform.forward * _data.DropForce, ForceMode.Impulse);

                    break;
            }
        }

        [ClientRpc]
        private void ChangeItemStateClientRpc(NetworkBehaviourReference item, ChangeItemStates state)
        {
            if (item.TryGet(out Item itemObject))
            {
                itemObject.Collider.enabled = itemObject.NetworkTransform.enabled = state switch
                {
                    ChangeItemStates.Take => false,
                    ChangeItemStates.Drop => true
                };
                itemObject.enabled = state switch
                {
                    ChangeItemStates.Take => true,
                    ChangeItemStates.Drop => false
                };
                itemObject.Rigidbody.isKinematic = state switch
                {
                    ChangeItemStates.Take => true,
                    ChangeItemStates.Drop => false
                };
                itemObject.transform.SetParent(state switch
                {
                    ChangeItemStates.Take => GetInventorySlot(itemObject.Data.SlotType).Pivot,
                    ChangeItemStates.Drop => null
                });
                if (state == ChangeItemStates.Take)
                    itemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        #endregion

        #region Select

        [ServerRpc]
        private void SelectItemServerRpc(SlotType slotType)
        {
            var item = Items.FirstOrDefault(item => item.Data.SlotType == slotType);
            if (item)
                SelectItemClientRpc(_item, _item = item);
        }
        [ServerRpc]
        private void SelectItemServerRpc(int offset)
            => SelectItemServerRpc(_item.Data.SlotType + offset);

        [ClientRpc]
        private void SelectItemClientRpc(NetworkBehaviourReference previousItem, NetworkBehaviourReference newItem)
        {
            if (previousItem.TryGet(out Item previousItemObject))
            {
                if (IsOwner)
                    ChangeLayer(previousItemObject.gameObject, Layers.Default);

                previousItemObject.transform.SetParent(GetInventorySlot(previousItemObject.Data.SlotType).Pivot);
                previousItemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            if (newItem.TryGet(out Item newItemObject))
            {
                if (IsOwner)
                    ChangeLayer(newItemObject.gameObject, Layers.Hand);

                newItemObject.Interact(_player);
                newItemObject.transform.SetParent(_itemPivot);
                newItemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            ItemChanged.Invoke(previousItemObject, newItemObject);
        }

        #endregion

        #region Activate

        [ServerRpc]
        private void ActivateItemServerRpc(Item.ActivationType type)
        {
            if (_item)
            {
                var random = Random.insideUnitCircle;
                _item.Activate(_viewCamera.transform.position, _viewCamera.transform.forward + _viewCamera.transform.TransformDirection(new Vector3(random.x, random.y, 0) * _spread), type);
            }
        }

        #endregion

        #region Scope

        [ServerRpc]
        private void ScopeServerRpc(bool value)
        {
            _viewCamera.fieldOfView = 90 / (value ? _item.Data.ScopeValue : 1);
            _itemCamera.transform.position = value ? _item.Sight.Point : _itemCameraPoint;
            
            ScopeClientRpc();
        }
        [ClientRpc]
        private void ScopeClientRpc()
        {
            _itemAudioSource.PlayOneShot(_data.GetSound(ControllerData.Sound.Types.Scope));
        }

        #endregion

        #endregion

        #region Blind

        [ServerRpc]
        public void BlindServerRpc(float duration)
            => BlindClientRpc(duration);

        [ClientRpc]
        private void BlindClientRpc(float duration)
        {
            if (IsOwner)
                Blinded.Invoke(duration);
        }

        #endregion

        #region Damage

        public void Damage(int value, RaycastHit hit = new RaycastHit())
        {
            _health.Value = Mathf.Clamp(_health.Value - value, 0, 101);
            if (_health.Value == 0)
                DieClientRpc(null);
            else
                DamageClientRpc();
        }

        [ClientRpc]
        public void DamageClientRpc()
        {
            _damageAudioSource.Play();

            Damaged.Invoke();
        }

        #endregion

        #region Die

        public void Die(PlayerController killer)
        {
            foreach (var item in _items)
                ChangeItemState(item, ChangeItemStates.Drop);

            DieClientRpc(killer);
        }

        [ClientRpc]
        public void DieClientRpc(NetworkBehaviourReference killer)
        {
            if (killer.TryGet(out Player killerPlayer))
                Died.Invoke(killerPlayer);
        }

        #endregion
    }

    #endregion
}