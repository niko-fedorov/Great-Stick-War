using UnityEngine;
using UnityEngine.UIElements;

namespace Menu
{
    [RequireComponent(typeof(AudioSource), typeof(AudioSource), typeof(UIDocument))]
    public class UIManager : global::UIManager
    {
        public static new UIManager Instance => global::UIManager.Instance as UIManager;

        private VisualElement _vignette;
        private float[] _musicAudioData;

        protected override void OnStart()
        {
            _vignette = GetComponent<UIDocument>().rootVisualElement.Q("vignette");
            _musicAudioData = GetMusicAudioData();
        }

        public float[] GetMusicAudioData()
        {
            var data = new float[_musicAudioSource.clip.samples];
            _musicAudioSource.clip.GetData(data, 0);

            return data;
        }
        public int GetMusicAudioTimeSamples()
            => _musicAudioSource.timeSamples;

        private void Update()
        {
            _vignette.style.unityBackgroundImageTintColor = new Color(0, 0, 0, _musicAudioData[UIManager.Instance.GetMusicAudioTimeSamples()]);
        }
    }
}
