using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class Block : Gun
    {
        #region Methods

        private void Update()
        {
            
        }

        public override void Activate(Vector3 origin, Vector3 direction, ActivationType type)
        {
            if (Physics.Raycast(origin, direction, out var hit))
            {
                WorldGenerator.Instance.SetVoxel(hit.point, Color.green);
                ActivateClientRpc();
            }
        }
        [ClientRpc]
        private void ActivateClientRpc()
        {
            _audioSource.Play();
        }
    }

    #endregion
}
