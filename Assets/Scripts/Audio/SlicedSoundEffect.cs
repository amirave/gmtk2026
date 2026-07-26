using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Audio
{
    [CreateAssetMenu()]
    public class SlicedSoundEffect : ScriptableObject, ISoundEffect
    {
        public AudioClip sfx;
        
        [SerializeField] private float _startTime = 0;
        [SerializeField] private float _endTime = 1;
        
        [HideInInspector] public float minVolume = 1;
        [HideInInspector] public float maxVolume = 1;
    
        [HideInInspector] public float minPitch = 1;
        [HideInInspector] public float maxPitch = 1;
        
        public async UniTask Play(AudioSource source, CancellationToken cancellationToken)
        {
            if (sfx == null) return;

            source.clip = sfx;
            source.volume = Random.Range(minVolume, maxVolume);
            source.pitch = Random.Range(minPitch, maxPitch);
            // source.outputAudioMixerGroup = audioOutput;
            source.Play();
            source.time = 0;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(source.clip.length), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await UniTask.WaitUntil(() => source.time > _startTime);
                source.time = _endTime;
            }
        }
    }
}