using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class VehicleGunData : GunData
    {
        [Header("Vehicle Gun Settings")]
        [SerializeField]
        private float _rotationSpeed;

        [SerializeField]
        private Vector2Int _rotationMinAngle, _rotationMaxAngle;
        public Vector2Int Min => _rotationMinAngle;
        public Vector2Int Max => _rotationMaxAngle;
    }
}
