using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Run,
    Jump,
    Mounted,
    Vehicled
}
public class PlayerController : NetworkBehaviour
{
    public event System.Action<IInteractable> Interaction = delegate { };
    public event System.Action InteractionEnd = delegate { };
    public event System.Action<Gun> GunChange = delegate { };
    public event System.Action GunRemoved = delegate { };

    private PlayerState _playerState;
    public PlayerState PlayerState => _playerState;

    [SerializeField]
    private float _walkSpeed,
                  _crawlSpeed,
                  _runSpeed,
                  _sensitivity,
                  _jumpHeight,
                  _actionDistance,
                  _maxGunCameraOffset;


    [SerializeField]
    private InventorySlot[] _inventorySlots;
    public InventorySlot[] InventorySlots => _inventorySlots;

    [System.Serializable]
    public struct InventorySlot
    {
        [SerializeField]
        private SlotType _slotType;
        public SlotType SlotType => _slotType;

        [SerializeField]
        private Gun _gunPrefab;
        public Gun GunPrefab => _gunPrefab;
        public Gun Gun;
        public NetworkBehaviourReference NetworkGun;

        //public Gun SpawnGun()
        //    => Gun = Instantiate(_gunPrefab, _pivot);

        [SerializeField]
        private Transform _pivot;
        public Transform Pivot => _pivot;
    }
    [SerializeField]
    private Transform _itemPivot;
    public Transform ItemPivot => _itemPivot;
    private Animator _armsAnimator;

    private Gun _gun;
    public Gun Gun => _gun;

    private int _currentGun;

    

    private float _speed,
                  _scopeCurrentVelocity;
    private Vector3 _targetRotation,
                    _fpCameraPosition,
                    _gunCameraPosition,
                    _movementDirection,
                    _velocity,
                    _rotationDirection,
                    _rotationVelocity,
                    _currentVelocity,
                    _clampRotation,
                    _aimCurrentVelocity;

    private RaycastHit _hit;
    private Transform _hitObject;
    private MountGun _mountGun;

    [SerializeField]
    private Transform _model;
    public Transform Model => _model;

    [SerializeField]
    private Camera _fpCamera,
                   _gunCamera;

    private CharacterController _controller; 

    public NetworkList<NetworkBehaviourReference> NetworkGuns = new NetworkList<NetworkBehaviourReference>();
    private Gun[] _guns;

    [ServerRpc]
    private void SpawnGunsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Gun gun;
        foreach (var slot in _inventorySlots)
        {
            gun = Instantiate(slot.GunPrefab);
            gun.NetworkObject.SpawnWithOwnership(OwnerClientId);
            gun.NetworkObject.TrySetParent(transform);
            NetworkGuns.Add(gun);
        }
    }
    
    public void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        _speed = _walkSpeed;

        _fpCameraPosition = _fpCamera.transform.localPosition;
        _gunCameraPosition = _gunCamera.transform.localPosition;

        _armsAnimator = _itemPivot.GetComponentInChildren<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        NetworkGuns.OnListChanged += (_) =>
        {
            Debug.Log("+");
            _guns = new Gun[NetworkGuns.Count];
            for (int i = 0; i < NetworkGuns.Count; i++)
                if (NetworkGuns[0].TryGet(out Gun gun))
                    _guns[i] = gun;

            _gun = _guns[0];
            EquipGun();
        };

        if (!IsOwner)
        {
            _model.gameObject.layer = LayerMask.NameToLayer("Default");
            _fpCamera.gameObject.SetActive(false);
            _gunCamera.gameObject.SetActive(false);
        }
        else
        {
            SpawnGunsServerRpc();
        }
    }

    private void EquipGun()
    {
        void ChangeLayer(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("First Person");
                ChangeLayer(parent.GetChild(i));
            }
        }

        ChangeLayer(_gun.transform);
        _gun.transform.parent = _itemPivot;

        _gun.transform.localPosition = Vector3.zero;
        _gun.transform.localRotation = Quaternion.identity;
        _gun.enabled = true;

        GunChange.Invoke(_gun);
    }
    private void RemoveGun()
    {
        for (int i = 0; i < _gun.transform.childCount; i++)
            _gun.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Third Person");
        _gun.transform.parent = _inventorySlots.First(x => x.SlotType == _gun.Data.SlotType).Pivot;
        _gun.transform.localPosition = Vector3.zero;
        _gun.transform.localRotation = Quaternion.identity;
        _gun.enabled = false;

        //GunRemoved.Invoke();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
            print($"Client({IsOwner}): {NetworkGuns.Count}");

        if (!IsOwner)
            return;

        #region Action

        if (Physics.Raycast(new Ray(_fpCamera.transform.position, _fpCamera.transform.forward), out _hit, _actionDistance))
        {
            if (_hit.transform.GetComponent<IInteractable>() != null && (!_hitObject || _hitObject != _hit.transform))
            {
                _hitObject = _hit.transform;

                Interaction.Invoke(_hitObject.GetComponent<IInteractable>());
            }
            /*
            if (_hitObject.GetComponent<MountGun>())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Mount");
                    _mountGun = _hitObject.GetComponent<MountGun>();
                    transform.position = _mountGun.transform.position + _mountGun.PlayerPositionOffset;
                    _targetRotation = _mountGun.transform.eulerAngles;
                    _clampRotation = _mountGun.ClampRotation;

                    _playerState = PlayerState.Mounted;

                    RemoveGun();
                    _mountGun.enabled = true;
                    GunChange.Invoke(_mountGun);
                }
            }
            else if (_hitObject.GetComponent<Gun>())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    _hit.collider.gameObject.layer = LayerMask.NameToLayer("First Person");
                    _hitObject.SetParent(_itemPivot);
                    _hitObject.localPosition = Vector3.zero;
                    _hitObject.localRotation = Quaternion.identity;

                    _hitObject.GetComponent<Gun>().enabled = true;
                }
            }
            else if (_hit.transform.GetComponent<Tank>())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    _hitObject.GetComponent<Tank>().enabled = true;

                    enabled = false;
                }
            }
            else if (_hitObject)
            {
                _hitObject = null;

                UIManager.Instance.ActionText.text = "";
            }
            */
        }
        else if (_hitObject)
        {
            InteractionEnd.Invoke();
            _hitObject = null;
        }

        #endregion

        if (Input.GetAxis("Fire1") != 0)
            _gun.ShootServerRpc(_fpCamera.transform.position, _fpCamera.transform.forward);
        if (Input.GetAxis("Fire2") != 0)
            _gun.Scope(true);
        else
            _gun.Scope(false);

        if (Input.GetKeyDown(KeyCode.R))
            if (_gun.Reload())
                _armsAnimator.SetBool("Reloading", true);

        #region Movement

        if (Input.GetKeyDown(KeyCode.LeftShift) && _movementDirection != Vector3.zero)
        {
            _speed = _runSpeed;
            _playerState = PlayerState.Run;
            _armsAnimator.SetBool("Run", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _walkSpeed;
            _armsAnimator.SetBool("Run", false);
        }

        _movementDirection = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _controller.height = _controller.radius;

        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {

        }

        if (!_controller.isGrounded)
        {
            _velocity.y += 2 * Physics.gravity.y * Time.deltaTime;

            _playerState = PlayerState.Jump;
            _armsAnimator.speed = 0;
        }
        else if (_velocity.y < _controller.skinWidth)
        {
            _velocity.y = -_controller.skinWidth;

            _playerState = PlayerState.Idle;
            _armsAnimator.speed = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            _velocity.y = _jumpHeight - Physics.gravity.y / 2;
        }

        _controller.Move(transform.TransformDirection((_movementDirection * _speed + _velocity) * Time.deltaTime));

        #endregion

        #region Rotation

        _rotationDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _rotationVelocity = _rotationDirection * _sensitivity * Time.deltaTime;

        _targetRotation += _rotationVelocity;

        if (_playerState == PlayerState.Mounted)
        {
            _targetRotation = new Vector2(Mathf.Clamp(_targetRotation.x, -_clampRotation.x, _clampRotation.x), Mathf.Clamp(_targetRotation.y, -_clampRotation.y, _clampRotation.y));

            _mountGun.transform.rotation = Quaternion.Euler(_targetRotation.y, _targetRotation.x, 0);
            //_fpCamera.transform.localPosition = _mountGun + _fpCameraPosition;
            _fpCamera.transform.forward = _gun.transform.forward;

        }

        _targetRotation.y = Mathf.Clamp(_targetRotation.y, -90, 90);

        transform.rotation = Quaternion.Euler(0, _targetRotation.x, 0);
        _fpCamera.transform.localRotation = _itemPivot.localRotation
            = Quaternion.Euler(Mathf.Clamp(-_targetRotation.y - _gun._recoilVelocity.y, -90, 90), _gun._recoilVelocity.x, 0); //_rotationVelocity.y

        #endregion

        if (_gun.IsAiming)
        {
            if (_gunCamera.transform.parent != _armsAnimator.transform)
                _gunCamera.transform.parent = _armsAnimator.transform;

            if (_fpCamera.fieldOfView != 60 / _gun.Sight.ScopeValue)
            {
                _fpCamera.fieldOfView = _gunCamera.fieldOfView = Mathf.SmoothDamp(_fpCamera.fieldOfView, 60 / _gun.Sight.ScopeValue, ref _scopeCurrentVelocity, 60 / _gun.Sight.ScopeValue / _gun.Data.ScopeSpeed * Time.deltaTime);
                _gunCamera.transform.localPosition = Vector3.Lerp(_gunCameraPosition, _gun.Sight.Position.localPosition, Mathf.InverseLerp(60, 60 / _gun.Sight.ScopeValue, _fpCamera.fieldOfView));
            }
            else
                _gunCamera.transform.localPosition = _gun.Sight.Position.localPosition - new Vector3(0, Mathf.Sin(_gun._recoilVelocity.y), 0) * .25f;
            //_armsAnimator.speed /= 2;
        }
        else
        {
            if (_gunCamera.transform.parent != _itemPivot)
                _gunCamera.transform.parent = _itemPivot;

            if (_fpCamera.fieldOfView != 60)
            {
                _fpCamera.fieldOfView = _gunCamera.fieldOfView = Mathf.SmoothDamp(_fpCamera.fieldOfView, 60, ref _scopeCurrentVelocity, _gun.Data.ScopeSpeed * Time.deltaTime);
                _gunCamera.transform.localPosition = Vector3.Lerp(_gun.Sight.Position.localPosition, _gunCameraPosition, Mathf.InverseLerp(60 / _gun.Sight.ScopeValue, 60, _fpCamera.fieldOfView));
            }

            //_armsAnimator.speed *= 2;

            _gunCamera.transform.localRotation = Quaternion.Euler(_gun._recoilVelocity.y, -_gun._recoilVelocity.x, 0);
            _gunCamera.transform.localPosition = _gunCameraPosition + Vector3.ClampMagnitude(_movementDirection - Vector3.up * _velocity.y / Physics.gravity.y, 1) * _maxGunCameraOffset;
        }

        if (Input.mouseScrollDelta.y != 0 && _gun)
        {
            if (_playerState == PlayerState.Idle)
            {
                RemoveGun();

                _currentGun += _inventorySlots.Length - (int)Input.mouseScrollDelta.y;
                _currentGun %= _inventorySlots.Length;
                if (_inventorySlots[_currentGun].Gun)
                    _gun = _inventorySlots[_currentGun].Gun;
                EquipGun();
            }
        }
    }
}
