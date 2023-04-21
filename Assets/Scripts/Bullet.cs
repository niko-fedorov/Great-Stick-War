using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : NetworkBehaviour
{
    public event UnityAction<IDamageable> Damaged = delegate { }, Destroyed;

    [SerializeField]
    private BulletHit _bulletHitPrefab;
    [SerializeField]
    private float _lifeTime;

    private GameObject[] _items;
    public GameObject[] Items => _items;

    private Vector3 _lastPosition;
    private RaycastHit _hit;

    private Gun _gun;

    public void Initialize(Gun gun)
        => _gun = gun;

    private void OnGUI()
    {
        Debug.DrawLine(_lastPosition, transform.position, Color.green, 100);
    }

    private void Start()
    {
        if (IsOwner)
        {
            _lastPosition = transform.position;

            Invoke("DestroyServerRpc", _lifeTime);
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        transform.position += (transform.forward * _gun.Data.Velocity + Vector3.down * _gun.Data.BulletDrop * Time.deltaTime) * Time.deltaTime;

        if (Physics.Raycast(new Ray(_lastPosition, transform.forward), out _hit, Vector3.Distance(_lastPosition, transform.position)))
        {
            var target = _hit.transform.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.Damage(_gun.Data.Damage, _hit);
                Damaged.Invoke(target);
            }

            if (!_hit.transform.GetComponentInParent<Enemy>())
                Instantiate(_bulletHitPrefab, _hit.point + _hit.normal * .001f, Quaternion.LookRotation(-_hit.normal, transform.right)).Initialize(_hit.transform.GetComponentInChildren<MeshRenderer>().material);

            DestroyServerRpc();
        }

        _lastPosition = transform.position;
    }

    [ServerRpc]
    private void DestroyServerRpc()
        => NetworkObject.Despawn();
}
