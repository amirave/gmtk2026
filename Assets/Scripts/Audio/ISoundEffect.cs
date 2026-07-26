using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.Audio
{
    public interface ISoundEffect
    {
        public UniTask Play(AudioSource source, CancellationToken cancellationToken);
    }
}