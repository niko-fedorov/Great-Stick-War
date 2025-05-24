using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class Gun : Item
    {
        #region Variables

        #region Events

        public delegate void AmmoChangedEvent(int ammo, int clipAmmo);
        public event AmmoChangedEvent AmmoChanged = delegate { };

        public event UnityAction Scoped;

        #endregion

        public int Ammo { get; private set; }
        public int ClipAmmo { get; private set; }

        private Coroutine _shooting,
                          _reloading;

        private ParticleSystem _shotParticleSystem;

        public new GunData Data => base.Data as GunData;

        #endregion

        #region Methods

        private void Start()
        {
            _shotParticleSystem = GetComponentInChildren<ParticleSystem>();

            if (IsServer)
            {
                Ammo = Data.Ammo;
                ClipAmmo = Data.ClipAmmo;
            }
        }

        private void Update()
        {
            if (IsOwner)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    ReloadServerRpc();
            }
        }

        //protected override void OnOwnershipChanged(ulong previous, ulong current)
        //{
        //    if(current == NetworkManager.LocalClientId)
        //        GetAmmoServerRpc();
        //}

        public override void Interact(Player player)
        {
            TakeClientRpc(Ammo, ClipAmmo);
        }
        [ClientRpc]
        private void TakeClientRpc(int ammo, int clipAmmo)
            => AmmoChanged.Invoke(ammo, clipAmmo);

        private void Drop()
        {

        }

        #region Activate

        public override void Activate(Vector3 origin, Vector3 direction, ActivationType type)
        {
            if ((type == ActivationType.Single || Data.FireMode == FireMode.Auto) && ClipAmmo > 0 && _shooting == null && _reloading == null)
            {
                _shooting = this.StartTimer(60f / Data.FireRate, () => _shooting = null);

                var bullet = Instantiate(Data.BulletPrefab, origin + direction, Quaternion.LookRotation(direction, transform.up));
                bullet.NetworkObject.Spawn();
                bullet.Initialize(this);

                //var recoil = new Vector3((Random.value < .5f ? 1 : -1) * Data.Recoil.Value.x, Data.Recoil.Value.y) * (_recoilVelocity == Vector3.zero ? Data.Recoil.FirstShotMultiplier : 1);
                //_recoilVelocity += recoil;
                //_spreadVelocity += Random.onUnitSphere * Data.Spread.IncreasePerBullet * Mathf.Deg2Rad * (_spreadVelocity == Vector3.zero ? Data.Spread.StaticSpread : 1);

                OnActivate();
                ActivateClientRpc(Ammo, --ClipAmmo);
            }
        }
        [Rpc(SendTo.Everyone)]
        private void ActivateClientRpc(int ammo, int clipAmmo)
        {
            _audioSource.PlayOneShot(Data.ShotSound);
            _shotParticleSystem.Play();

            AmmoChanged.Invoke(ammo, clipAmmo);
        }

        #endregion

        #region Reload

        [ServerRpc]
        public void ReloadServerRpc()
        {
            if (ClipAmmo < Data.ClipAmmo && Ammo > 0 && _shooting == null && _reloading == null)
            {
                ReloadClientRpc();

                _reloading = this.StartTimer(Data.ReloadTime, () =>
                {
                    int ammo = Mathf.Min(Data.ClipAmmo - ClipAmmo, Ammo);
                    ClipAmmo += ammo;
                    Ammo -= ammo;

                    _reloading = null;
                });

                ReloadedClientRpc(Ammo, ClipAmmo);
            }
        }
        [Rpc(SendTo.Everyone)]
        private void ReloadClientRpc()
        {
            _audioSource.PlayOneShot(Data.ReloadSound);
        }
        [Rpc(SendTo.Everyone)]
        private void ReloadedClientRpc(int ammo, int clipAmmo)
        {
            AmmoChanged.Invoke(ammo, clipAmmo);
        }

        #endregion

        //public void Scope(bool value)
        //{
        //    _isAiming = value;
        //}

        #endregion
    }
}
