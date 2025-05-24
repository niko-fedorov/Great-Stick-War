using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class GrenadeData : GunData
    {
        [Header("Grenade Settings")]

        [SerializeField]
        private float _explosionTime;
        public float ExplosionTime => _explosionTime;

        [SerializeField]
        private float _damageRadius;
        public float DamageRadius => _damageRadius;
    }
}
