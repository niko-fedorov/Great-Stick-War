using UnityEngine;
using Unity.Netcode;

namespace Game
{
    [RequireComponent(typeof(AudioSource))]
    public class NetworkAudioSource : NetworkBehaviour
    {
        //private AudioSource _audioSource;

        //private void Start()
        //{
        //    _audioSource = GetComponent<AudioSource>();
        //}

        //public void Play(AudioClip audioClip)
        //{
        //    _audioSource.PlayOneShot(audioClip);

        //    PlayClientRpc(audioClip.name);
        //}
        //[ClientRpc]
        //private void PlayClientRpc(string clipName, bool interrupt = true)
        //{
        //    if (interrupt || !_audioSource.isPlaying)
        //        _audioSource.PlayOneShot(GameManager.Instance.AudioClipDatabase.GetAudioClip(clipName));
        //}
    }
}
