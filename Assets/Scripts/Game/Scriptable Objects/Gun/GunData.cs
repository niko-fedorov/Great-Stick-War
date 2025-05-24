using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public enum FireMode
    {
        Auto,
        Single
    }

    [CreateAssetMenu]
    public class GunData : ItemData
    {
        [SerializeField]
        private FireMode _fireMode;
        public FireMode FireMode => _fireMode;

        [SerializeField]
        private int _damage,
                    _fireRate;
        public int Damage => _damage;
        public int FireRate => _fireRate;

        [SerializeField]
        private float _distance,
                      _reloadTime,
                      _velocity,
                      _bulletDrop;
        public float Distance => _distance;
        public float ReloadTime => _reloadTime;
        public float Velocity => _velocity;
        public float BulletDrop => _bulletDrop;

        [SerializeField]
        private int _clipAmmo,
                    _ammo;
        public int ClipAmmo => _clipAmmo;
        public int Ammo => _ammo;

        [Header("Audio Settings")]
        [SerializeField]
        private AudioClip _shotSound,
                          _reloadSound;
        public AudioClip ShotSound => _shotSound;
        public AudioClip ReloadSound => _reloadSound;

        [SerializeField]
        private Bullet _bulletPrefab;
        public Bullet BulletPrefab => _bulletPrefab;
        [SerializeField]
        private BulletHit _bulletHitPrefab;
        public BulletHit BulletHitPrefab => _bulletHitPrefab;
    }
}
