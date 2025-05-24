using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class MountGunData : GunData
    {
        [Header("Mount Settings")]
        [SerializeField]
        private float _layingSpeed;
        public float LayingSpeed => _layingSpeed;
        [SerializeField]
        private Vector2 _clampAngle;
        public Vector2 ClampAngle => _clampAngle;
    }
}
