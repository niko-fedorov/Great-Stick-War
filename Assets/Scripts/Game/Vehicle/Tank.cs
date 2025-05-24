using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Tank : Vehicle
    {
        #region Variables

        [Header("Tank Settings")]
        [SerializeField]
        private Transform _turret;

        private Vector3 _direction,
                        _barrelCurrentRotationVelocity;

        private float _turretTargetAngle;

        private Gun _barrel;

        private Slider _healthBar;

        #endregion

        #region Methods

        private void Update()
        {
            #region Movement

            _direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.Translate(0, 0, _direction.z * Data.Speed, Space.Self);
            transform.Rotate(0, _direction.x * Data.RotationSpeed * Time.deltaTime, 0);

            //if (_direction != Vector3.zero)
            //    _networkAudioSource.PlayOneShot(_data.GetSound(SoundType.Idle));
            //else if (_networkAudioSource.clip == _data.GetSound(SoundType.Idle))
            //    _networkAudioSource.Stop();

            #endregion

            #region Rotation

            if (_turret)
            {
                //_turret.LookAt(,, transform.forward);
                //_turretTargetAngle = Vector3.SmoothDamp(_turretTargetAngle, )
            }

            //_barrel.transform.forward = Vector3.SmoothDamp(_barrel.transform.forward, , ref _barrelCurrentRotationVelocity, _data.RotationSpeed * Time.deltaTime);

            #endregion

            #region Action

            if (Input.GetMouseButtonDown(0))
            {
                //_barrel.ShootServerRpc(_camera.transform.position, _barrelCurrentRotationVelocity);
            }

            #endregion
        }

        private void OnDisable()
        {
            //_camera.enabled = false;
        }

        /*
        public void Damage(float damage, RaycastHit hit)
        {
            CurrentHealth -= damage;

            _healthBar.value = CurrentHealth;

            if (CurrentHealth <= 0)
                Die();
        }

        public void Die()
        {
            Died.Invoke();
        }
        */
    }

    #endregion
}
