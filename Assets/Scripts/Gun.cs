using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Gun : NetworkBehaviour, IInteractable
{
    public event UnityAction Shot = delegate { };
    public event UnityAction TargetDestroyed = delegate { }; 
    public event UnityAction Scoped = delegate { };
    public event UnityAction AmmoChanged = delegate { };
    public event UnityAction Reloaded = delegate { };

    public string Name => _data.Name;
    public bool IsInteractable { get; private set; }

    public string ActionText => "Нажмите F, чтобы поднять";
    public Color TextColor => GameManager.Instance.GetActionColor(ActionType.Damage);

    [SerializeField]
    private GunData _data;
    public GunData Data => _data;

    private int _currentAmmo,
                _currentClipAmmo;
    public int CurrentAmmo => _currentAmmo;
    public int CurrentClipAmmo => _currentClipAmmo;

    private bool _isShooting,
                 _isAiming,
                 _isReloading;
    public bool IsAiming => _isAiming;

    private float _recoilSmoothTime,
                  _spreadSmoothTime;
    public Vector3 _recoilVelocity;
    private Vector3 _spreadVelocity,
                    _recoilCurrentVelocity,
                    _spreadCurrentVelocity;

    [SerializeField]
    private Sight _sight;
    public Sight Sight => _sight;

    private AudioSource _audioSource;
    private ParticleSystem _shotParticleSystem;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _shotParticleSystem = GetComponentInChildren<ParticleSystem>();

        _currentAmmo = _data.Ammo;
        _currentClipAmmo = _data.ClipAmmo;

        
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }

    public void Scope(bool value)
    {
        _isAiming = value;
    }

    [ServerRpc]
    public void ShootServerRpc(Vector3 origin, Vector3 direction)
    {
        if (_currentClipAmmo <= 0 || _isShooting || _isReloading)
            return;

        var recoil = new Vector3((Random.value < .5f ? 1 : -1) * _data.Recoil.Value.x, _data.Recoil.Value.y) * (_recoilVelocity == Vector3.zero ? _data.Recoil.FirstShotMultiplier : 1);
        _recoilVelocity += recoil;
        _spreadVelocity += Random.onUnitSphere * _data.Spread.IncreasePerBullet * Mathf.Deg2Rad * (_spreadVelocity == Vector3.zero ? _data.Spread.StaticSpread : 1);

        var ray = new Ray(origin, direction + Vector3.Cross(_spreadVelocity, direction));

        _currentClipAmmo--;

        _audioSource.PlayOneShot(_data.ShotSound);
        _shotParticleSystem.Play();

        Shot.Invoke();
        AmmoChanged.Invoke();

        var bullet = Instantiate(_data.BulletPrefab, ray.origin + ray.direction, Quaternion.LookRotation(ray.direction, transform.up));
        bullet.Initialize(this);
        //bullet.Damaged += 
        bullet.NetworkObject.Spawn();

        StartCoroutine(Shooting());

        _recoilCurrentVelocity = _spreadCurrentVelocity = Vector3.zero;

        //return Vector3.zero;
    }

    public bool Reload()
    {
        if (CurrentClipAmmo < _data.ClipAmmo && CurrentAmmo > 0 && !_isReloading && !_isShooting)
        {
            StartCoroutine(Reloading());

            return true;
        }

        return false;
    }

    private IEnumerator Shooting()
    {
        _isShooting = true;

        yield return new WaitForSeconds(60 / _data.FireRate);

        _isShooting = false;
    }
    private IEnumerator Reloading()
    {
        _isReloading = true;

        _audioSource.PlayOneShot(_data.ReloadSound);

        yield return new WaitForSeconds(_data.ReloadTime);

        int ammo = Mathf.Min(_data.ClipAmmo - _currentClipAmmo, _currentAmmo);
        _currentClipAmmo += ammo;
        _currentAmmo -= ammo;

        Reloaded.Invoke();

        _isReloading = false;
    }

    private void Update()
    {
        if (_recoilVelocity + _spreadVelocity != Vector3.zero)
        {
            if(_recoilCurrentVelocity.magnitude == 0)
                _recoilSmoothTime = _recoilVelocity.magnitude / _data.Recoil.DecreasePerSecond;
            _recoilVelocity = Vector3.SmoothDamp(_recoilVelocity, Vector3.zero, ref _recoilCurrentVelocity, _recoilSmoothTime);
            if (_spreadCurrentVelocity.magnitude == 0)
                _spreadSmoothTime = _spreadVelocity.magnitude / _data.Spread.DecreasePerSecond;
            _spreadVelocity = Vector3.SmoothDamp(_spreadVelocity, Vector3.zero, ref _spreadCurrentVelocity, _spreadSmoothTime);
        }

        /*
        if (_isShooting)
        {
            _time += Time.deltaTime;
            if (_time >= 60 / _data.FireRate)
            {
                _time = 0;

                _isShooting = false;
            }
        }
        */
        /*
        if (_isReloading)
        {
            _time += Time.deltaTime;
            if (_time >= _data.ReloadTime)
            {
                _time = 0;

                int ammo = Mathf.Min(_data.ClipAmmo - _currentClipAmmo, _currentAmmo);
                _currentClipAmmo += ammo;
                _currentAmmo -= ammo;

                _isReloading = false;

                //AmmoChanged.Invoke();
            }
        }
        */
    }
}
