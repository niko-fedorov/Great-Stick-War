using UnityEngine;

public class Sight : MonoBehaviour
{
    [SerializeField]
    private Transform _position;
    public Transform Position => _position;

    [Range(1, 8)]
    [SerializeField]
    private float _scopeValue;
    public float ScopeValue => _scopeValue;
}
