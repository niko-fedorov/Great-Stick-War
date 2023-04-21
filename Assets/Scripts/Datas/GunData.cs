using UnityEngine;

public enum SlotType
{
    Primary,
    Sidearm
}
public enum FireMode
{
    Auto,
    Single
}

[CreateAssetMenu(fileName = "NewGunData", menuName = "Gun Datas/Firearm Data")]
public class GunData : ScriptableObject
{
    [SerializeField]
    private SlotType _slotType;
    public SlotType SlotType => _slotType;
    [SerializeField]
    private FireMode _fireMode;
    public FireMode FireMode => _fireMode;
    [SerializeField]
    private string _name;
    public string Name => _name;

    [SerializeField]
    private float _damage,
                  _fireRate,
                  _distance,
                  _scopeSpeed,
                  _reloadTime,
                  _scopeValue,
                  _velocity,
                  _bulletDrop;
    public float Damage => _damage;
    public float Distance => _distance;
    public float FireRate => _fireRate;
    public float ReloadTime => _reloadTime;
    public float ScopeValue => _scopeValue;
    public float ScopeSpeed => _scopeSpeed;
    public float Velocity => _velocity;
    public float BulletDrop => _bulletDrop;

    [System.Serializable]
    public struct RecoilData
    {
        [SerializeField]
        private Vector2 _value;
        [SerializeField]
        private float _firstShotMultiplier,
                      _decreasePerSecond;
        public Vector2 Value => _value;
        public float FirstShotMultiplier => _firstShotMultiplier;
        public float DecreasePerSecond => _decreasePerSecond;
                      
    }
    [SerializeField]
    private RecoilData _recoil;
    public RecoilData Recoil => _recoil;
    [System.Serializable]
    public struct SpreadData
    {
        [SerializeField]
        private float _staticSpread,
                      _hipfireStaticSpread,
                      _hipfireMovingSpread,
                      _zoomedStaticSpread,
                      _increasePerBullet,
                      _decreasePerSecond;
        public float StaticSpread => _staticSpread;
        public float IncreasePerBullet => _increasePerBullet;
        public float DecreasePerSecond => _decreasePerSecond;
    }
    [SerializeField]
    private SpreadData _spread;
    public SpreadData Spread => _spread;

    [SerializeField]
    private int _clipAmmo,
                _ammo;
    public int ClipAmmo => _clipAmmo;
    public int Ammo => _ammo;

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
