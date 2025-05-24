using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class VehicleControllerData : ControllerData
    {
        [SerializeField]
        private string _name;
        public string Name => _name;

        [SerializeField]
        private SideType _sideType;
        public SideType SideType => _sideType;

        [SerializeField]
        private int _health;
        public int Health => _health;

        [SerializeField]
        private float _speed,
                      _rotationSpeed;
        public float Speed => _speed;
        public float RotationSpeed => _rotationSpeed;

        [SerializeField]
        private Vector3 _size;
        public Vector3 Size => _size;

        [SerializeField]
        private float _turretRotationSpeed;
    }
}
