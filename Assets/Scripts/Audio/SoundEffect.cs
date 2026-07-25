using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Audio
{
    [CreateAssetMenu()]
    public class SoundEffect : ScriptableObject, ISoundEffect
    {
        public AudioClip[] sfx;

        [HideInInspector] public float minVolume = 1;
        [HideInInspector] public float maxVolume = 1;
    
        [HideInInspector] public float minPitch = 1;
        [HideInInspector] public float maxPitch = 1;

        public async UniTask Play(AudioSource source, CancellationToken cancellationToken)
        {
            if (sfx.Length == 0) return;

            source.clip = sfx.PickRandom();
            source.volume = Random.Range(minVolume, maxVolume);
            source.pitch = Random.Range(minPitch, maxPitch);
            // source.outputAudioMixerGroup = audioOutput;
            source.time = 0;
            source.Play();
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(source.clip.length), true, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                source.Stop();
            }
        }
    }
}
