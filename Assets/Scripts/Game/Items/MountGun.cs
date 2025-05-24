using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class MountGun : Gun
    {
        #region Variables

        [Header("Mount Gun Settings")]
        [SerializeField]
        private Transform _playerPivot;

        public Vector3 PlayerPositionOffset { get; private set; }

        private bool _isOccupied;

        public new MountGunData Data => base.Data as MountGunData;

        #endregion

        #region Methods

        private void Update()
        {
            if (IsOwner)
            {
                var m = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")).normalized;
                if(m != Vector2.zero)
                    RotateServerRpc(m, NetworkManager.LocalTime.TimeAsFloat);
                
                //if (Input.GetKey(KeyCode.Mouse0))
                //    Activate(transform.position, transform.forward, );
                if (Input.GetKey(KeyCode.R))
                    ReloadServerRpc();
            }
        }

        public override void Interact(Player player)
        {
            if (!_isOccupied)
            {
                transform.SetPositionAndRotation(_playerPivot.transform.position, _playerPivot.transform.rotation);

                _isOccupied = true;
            }
        }

        public void Leave()
        {
            _isOccupied = false;
        }

        [ServerRpc]
        private void RotateServerRpc(Vector2 angle, float time = 0)
        {
            angle *= IsServer ? Time.deltaTime : NetworkManager.ServerTime.TimeAsFloat - time;

            RotateClientRpc(
                new Vector3(
                    Mathf.Clamp(transform.eulerAngles.x + angle.y, -Data.ClampAngle.y, Data.ClampAngle.y),
                    Mathf.Clamp(transform.eulerAngles.y + angle.x, -Data.ClampAngle.x, Data.ClampAngle.x)));
        }
        [ClientRpc]
        private void RotateClientRpc(Vector3 rotation)
        {
            transform.eulerAngles = rotation;
        }

        #endregion
    }
}
