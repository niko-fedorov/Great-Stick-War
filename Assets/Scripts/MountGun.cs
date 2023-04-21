using UnityEngine;

public class MountGun : Gun
{
    [Header("Mount Gun Settings")]
    [SerializeField]
    private Vector2 _clampRotation;
    public Vector2 ClampRotation => _clampRotation;
    [SerializeField]
    private Transform _playerPivot;

    public Vector3 PlayerPositionOffset { get; private set; }

    private new void Awake()
    {
        //base.Awake();

        PlayerPositionOffset = _playerPivot.localPosition;
    }
}
