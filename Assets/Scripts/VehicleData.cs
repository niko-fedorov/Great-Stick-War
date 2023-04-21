using UnityEngine;

[CreateAssetMenu(fileName = "NewVehicleData", menuName = "Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [SerializeField]
    private float _speed,
                  _rotationSpeed;

    public float Speed => _speed;
    public float RotationSpeed => _rotationSpeed;
}
