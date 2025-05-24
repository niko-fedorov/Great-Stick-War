using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Bullet : NetworkBehaviour
    {
        public event UnityAction<IDamageable> Damaged = delegate { };

        [SerializeField]
        private BulletHit _bulletHitPrefab;
        [SerializeField]
        private float _lifeTime;

        private Gun _gun;
        private Vector3 _position;

        public void Initialize(Gun gun)
        {
            _gun = gun;

            this.StartTimer(_lifeTime, () => NetworkObject.Despawn());
        }

        private void Update()
        {
            _position = transform.position;
            transform.position += (transform.forward * _gun.Data.Velocity + Vector3.down * _gun.Data.BulletDrop * Time.deltaTime) * Time.deltaTime;

            if (IsServer && Physics.Linecast(_position, transform.position, out var hit))
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.Damage(_gun.Data.Damage, hit);
                Instantiate(_bulletHitPrefab, hit.point, Quaternion.LookRotation(hit.normal)).NetworkObject.Spawn();//.Initialize(damageable.);
                
                NetworkObject.Despawn();
            }
        }
    }
}
