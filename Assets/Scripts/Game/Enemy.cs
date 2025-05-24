using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;

namespace Game
{
    public class Enemy : MonoBehaviour//, IDamageable
    {
        #region Variables

        public event UnityAction Damaged;
        public event UnityAction Died;

        private NetworkVariable<int> _health = new NetworkVariable<int>(100);
        public int Health => _health.Value;

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

        #endregion

        #region Methods

        //private void Start()
        //{
        //    _animator = GetComponentInChildren<Animator>();
        //    _agent = GetComponent<NavMeshAgent>();

        //    _healthBar = GetComponentInChildren<Slider>();

        //    for (int i = 0; i < _inventorySlots.Length; i++)
        //        _inventorySlots[i].SpawnGun();

        //    _gun = _inventorySlots[0].Gun;
        //}

        //public void Damage(int damage, RaycastHit hit)
        //{
        //    _healthBar.value = _health.Value -= damage;

        //    _isDamaged = true;
        //    _model.material.color = GameManager.Instance.GetActionColor(ActionType.Damage);

        //    Damaged.Invoke();

        //    if (CurrentHealth <= 0)
        //        Die();
        //}
        //public void Die()
        //{
        //    IsDead = true;

        //    _model.material.color = GameManager.Instance.GetActionColor(ActionType.Death);

        //    _animator.SetTrigger("Death");

        //    Died.Invoke();

        //    enabled = false;
        //}

        //private void Update()
        //{
        //    transform.forward = _agent.destination;

        //    _player = Physics.OverlapSphere(transform.position, 10).Select(x => x.GetComponent<Player>()).First(x => x);

        //    if (_player && _player.Health.Value > 0 && Vector3.Distance(transform.position, _player.transform.position) <= _gun.Data.Distance)
        //    {
        //        _gun.ShootServerRpc(transform.position + Vector3.up,
        //            _player.transform.position + new Vector3(Random.Range(-.125f, .125f), Random.Range(0, 1.5f), Random.Range(-.125f, .125f))
        //                + Random.insideUnitSphere * _inaccuracy
        //                    - transform.position);
        //    }
        //    else
        //        _agent.SetDestination(_player.transform.position);

        //    if (_gun.Ammo == 0 && _gun.Ammo > 0)
        //        _gun.Reload();

        //}
    }

    #endregion
}
