using System.Linq;
using UnityEngine;

namespace Game
{
    public abstract class ControllerData : ScriptableObject
    {
        [System.Serializable]
        public struct Sound
        {
            public enum Types
            {
                Idle,
                Move,
                Scope,
                Damage,
                Destroy
            }
            [SerializeField]
            private Types _type;
            public Types Type => _type;

            [SerializeField]
            private AudioClip _audioClip;
            public AudioClip AudioClip => _audioClip;
        }
        [Header("Controller Settings"), SerializeField]
        private Sound[] _sounds;
        public Sound[] Sounds => _sounds;
        public AudioClip GetSound(Sound.Types type)
            => _sounds.First(sound => sound.Type == type).AudioClip;
    }
}
