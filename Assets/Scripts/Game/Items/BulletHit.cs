using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(AudioSource))]
    public class BulletHit : NetworkBehaviour
    {
        [SerializeField]
        private float _duration;

        [System.Serializable]
        private struct HitSound
        {
            [SerializeField]
            private SurfaceType _surfaceType;
            public SurfaceType SurfaceType => _surfaceType;
            [SerializeField]
            private AudioClip[] _sounds;
            public AudioClip[] Sounds => _sounds;
            public AudioClip Sound => _sounds[Random.Range(0, _sounds.Length)];
        }
        [SerializeField]
        private HitSound[] _hitSounds;

        public void Initialize(Material hitMaterial)
        {
            if (IsServer)
            {
                NetworkObject.Spawn();
                this.StartTimer(_duration, () => NetworkObject.Despawn());
            }

            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = _hitSounds.First(x => (int)x.SurfaceType == hitMaterial.GetFloat("_SurfaceType")).Sound;
            audioSource.Play();
        }
    }
}
