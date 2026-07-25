using System;
using UnityEngine;

namespace Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioInjector: MonoBehaviour
    {
        private void Start()
        {
            AudioManager.Instance.SetupAudioSource(GetComponent<AudioSource>());
        }
    }
}