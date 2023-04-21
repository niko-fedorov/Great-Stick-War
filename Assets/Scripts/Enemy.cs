using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

public class Enemy : MonoBehaviour, IDamageable
{
    public event System.Action Damaged = delegate { };
    public event System.Action Died = delegate { };

    public float Health { get; } = 100;
    public float CurrentHealth { get; set; } = 100;

    [SerializeField]
    private SideType _sideType;

    [SerializeField]
    private InventorySlot[] _inventorySlots;
    [System.Serializable]
    private struct InventorySlot
    {
        [SerializeField]
        private SlotType _slotType;
        public SlotType SlotType => _slotType;

        [SerializeField]
        private Gun _gunPrefab;
        public Gun Gun { get; private set; }
        public Gun SpawnGun()
            => Gun = Instantiate(_gunPrefab, _pivot);

        [SerializeField]
        private Transform _pivot;
        public Transform Pivot => _pivot;
    }

    [SerializeField]
    private float _speed,
                  _shotDelay,
                  _inaccuracy,                   
                  _damageDuration;

    [SerializeField]
    private MeshRenderer _model;
    [SerializeField]
    private Transform _itemPivot;

    private float _damageTime;

    private bool _isDamaged;
    public bool IsDead { get; private set; }

    private Gun _gun;

    private Slider _healthBar;

    private Player _player;

    private Animator _animator;
    private NavMeshAgent _agent;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _healthBar = GetComponentInChildren<Slider>();

        _model.material.color = GameManager.Instance.GetSideColor(_sideType);

        for (int i = 0; i < _inventorySlots.Length; i++)
            _inventorySlots[i].SpawnGun();

        _gun = _inventorySlots[0].Gun;
    }

    public void Damage(float damage, RaycastHit hit)
    {
        _healthBar.value = CurrentHealth -= damage;

        _isDamaged = true;
        _model.material.color = GameManager.Instance.GetActionColor(ActionType.Damage);

        Damaged.Invoke();

        if (CurrentHealth <= 0)
            Die();
    }
    public void Die()
    {
        IsDead = true;

        _model.material.color = GameManager.Instance.GetActionColor(ActionType.Death);

        _animator.SetTrigger("Death");

        Died.Invoke();

        enabled = false;
    }

    private void Update()
    {
        transform.forward = _agent.destination;
        
        _player = Physics.OverlapSphere(transform.position, 10).Select(x => x.GetComponent<Player>()).First(x => x);

        if (_player && _player.Health > 0 && Vector3.Distance(transform.position, _player.transform.position) <= _gun.Data.Distance)
        {
            _gun.ShootServerRpc(transform.position + Vector3.up,
                _player.transform.position + new Vector3(Random.Range(-.125f, .125f), Random.Range(0, 1.5f), Random.Range(-.125f, .125f))
                    + Random.insideUnitSphere * _inaccuracy
                        - transform.position);
        }
        else
            _agent.SetDestination(_player.transform.position);

        if (_gun.CurrentClipAmmo == 0 && _gun.CurrentAmmo > 0)
            _gun.Reload();

        if(_isDamaged)
        {
            _damageTime += Time.deltaTime;
            if (_damageTime >= _damageDuration)
            {
                _damageTime = 0;
                _model.material.color = GameManager.Instance.GetSideColor(_sideType);
                _isDamaged = false;
            }
        }
    }
}
