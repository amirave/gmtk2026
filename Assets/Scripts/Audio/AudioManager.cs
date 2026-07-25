using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Scripts.Audio;

namespace Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [SerializeField] private AudioClip[] _musicTracks;
        [SerializeField] private AudioMixer _mixer;

        private PlayerState _playerState;
  
        private Dictionary<string, ISoundEffect> _sfxClips;
        private List<AudioSource> _sfxSources;
        private List<AudioMusicTrack> _bgmTracks;
    
        private AudioMixerGroup _mixerSFX;
        private AudioMixerGroup _mixerBGM;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            _playerState = new PlayerState()
            {
                highScore = 100,
                musicVolume = 0.5f,
                sfxVolume = 0.35f
            };
        }

        void Start()
        {
            _mixerSFX = _mixer.FindMatchingGroups("Master/SFX")[0];
            _mixerBGM = _mixer.FindMatchingGroups("Master/Music")[0];
        
            _sfxClips = new Dictionary<string, ISoundEffect>();

            _sfxSources = new List<AudioSource>();

            _bgmTracks = new List<AudioMusicTrack>();

            foreach (var clip in _musicTracks)
                CreateMusicTrack(clip);
        
            LoadSounds();

            ToggleChannelInternal(AudioChannel.Music, _playerState.musicVolume, false);
            ToggleChannelInternal(AudioChannel.Fx, _playerState.sfxVolume, false);
        }
    
        private void LoadSounds()
        {
            var allSfx = Resources.LoadAll<SoundEffect>("");
            foreach (var sfx in allSfx)
            {
                var sfxId = sfx.name;
                _sfxClips.Add(sfxId, sfx);
            }
        }

        private void CreateMusicTrack(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioSystem] trying to create a music track from an empty clip");
                return;
            }

            var obj = new GameObject("BGM_" + clip.name, typeof(AudioSource), typeof(AudioMusicTrack));
            obj.transform.SetParent(transform);

            var src = obj.GetComponent<AudioSource>();
            src.clip = clip;

            var track = obj.GetComponent<AudioMusicTrack>();
            track.Initialize(_mixerBGM);
        
            _bgmTracks.Add(track);
        }
    
        public void PlayMusicTrack(string id)
        {
            var found = false;
            
            foreach (var track in _bgmTracks)
            {
                if (track.id.Equals(id))
                {
                    track.FadeIn();
                    found = true;
                }
                else
                {
                    track.FadeOut();
                }
            }
            
            if ( found == false )
                Debug.LogWarning($"[{typeof(AudioManager)}] Could not play music track because track was not found - {id}");
        }

        public void FadeOutMusic()
        {
            foreach (var track in _bgmTracks)
            {
                track.FadeOut();
            }
        }
    
        public async UniTask PlayEffect(string id, CancellationToken cancellationToken = default)
        {
            if (_sfxClips.ContainsKey(id) == false)
            {
                Debug.LogWarning($"[{typeof(AudioManager)}] Could not play effect because clip was not found - {id}");
                return;
            }

            var sfx = _sfxClips[id];
            var src = GetOrCreateSFXAudioSource();
            
            await sfx.Play(src, cancellationToken);
        }
    
        public async UniTask PlayEffectWithDelay(string id, float delay, CancellationToken cancellationToken)
        {
            var wasCancelled = await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            if (wasCancelled == true)
                return;

            PlayEffect(id, cancellationToken);
        }
        
        private AudioSource GetOrCreateSFXAudioSource()
        {
            // Try reusing existing source
            for (var i = 0; i < _sfxSources.Count; i++)
            {
                var source = _sfxSources[i];
                if (source.isPlaying == false)
                    return source;
            }
            
            // Create new source
            var obj = new GameObject($"Voice{_sfxSources.Count + 1}", typeof(AudioSource));
            obj.transform.SetParent(transform);

            var src = obj.GetComponent<AudioSource>();

            src.playOnAwake = false;
            src.clip = null;
            src.loop = false;
            src.spatialize = false;
            src.outputAudioMixerGroup = _mixerSFX;
            
            _sfxSources.Add(src);
            return src;
        }

        public void SetupAudioSource(AudioSource src)
        {
            src.playOnAwake = false;
            src.clip = null;
            src.loop = false;
            src.spatialize = false;
            src.outputAudioMixerGroup = _mixerSFX;
        }

        public void ToggleChannelInternal(AudioChannel channel, float value, bool writeToProfile)
        {
            var paramName = channel == AudioChannel.Fx
                ? AudioNames.MIXER_PARAM_VOLUME_SFX
                : AudioNames.MIXER_PARAM_VOLUME_BGM;

            var logValue = Mathf.Sqrt(value);
            var paramValue = Mathf.Lerp(MagicNumbers.SOUND_DISABLED_VOLUME_DB, MagicNumbers.SOUND_ENABLED_VOLUME_DB, logValue);

            _mixer.SetFloat(paramName, paramValue);

            if (writeToProfile)
            {
                if (channel == AudioChannel.Music)
                    _playerState.musicVolume = value;
                else
                    _playerState.sfxVolume = value;

            }
        }
        
        public float GetChannelVolume(AudioChannel channel)
        {
            return channel == AudioChannel.Music ? _playerState.musicVolume : _playerState.sfxVolume;
        }
    
        public float GetChannelActive(AudioChannel channel)
        {
            var paramName = channel == AudioChannel.Fx
                ? AudioNames.MIXER_PARAM_VOLUME_SFX
                : AudioNames.MIXER_PARAM_VOLUME_BGM;

            if (_mixer.GetFloat(paramName, out var value) == false)
                return 0;
            
            return value;
        }
    
        public void SetChannelActive(AudioChannel channel)
        {
            ToggleChannelInternal(channel, channel == AudioChannel.Fx ? _playerState.sfxVolume : _playerState.musicVolume, false);
        }
    }
}
