using Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuManager: MonoBehaviour
    {
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private AudioClip _sampleClip;
        [SerializeField] private AudioSource _audioSource;

        void Start()
        {
            _musicSlider.value = AudioManager.Instance.GetChannelVolume(AudioChannel.Music);
            AudioManager.Instance.SetupAudioSource(_audioSource);
        }

        public void OnDragUp()
        {
            _audioSource.clip = _sampleClip;
            _audioSource.Play();
        }

        void Update()
        {
            AudioManager.Instance.ToggleChannelInternal(AudioChannel.Music, _musicSlider.value, true);
            AudioManager.Instance.ToggleChannelInternal(AudioChannel.Fx, _musicSlider.value, true);
        }
    }
}