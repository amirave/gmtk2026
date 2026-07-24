using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Game
{
    public enum Decision
    {
        Smash,
        Pass,
        None
    }
    
    public class GameManager : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float _inputTime = 1.5f + 0.25f;
        [SerializeField] private float _inputTimeBefore = 0.25f;
        [SerializeField] private float _inputTimeAfter = 0.25f;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private PlayableDirector _directorChild;
        [SerializeField] private TimelineAsset _animMain;
        [SerializeField] private TimelineAsset _animSmash;
        [SerializeField] private TimelineAsset _animPass;
        [SerializeField] private Transform _itemParent;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _audioSecondary;
        [SerializeField] private AudioClip _audioMain;
        [SerializeField] private AudioClip _audioSmash;
        [SerializeField] private AudioClip _audioPass;
        
        [Header("Settings")]
        [SerializeField] private float _timePerRound = 2;
        [SerializeField] private List<LevelConfig> _levelConfigs;

        private readonly List<Rule> _rules = new();
        private Level _level;
        private CancellationTokenSource _cts;

        private LevelConfig Config => _levelConfigs[_level.levelNumber - 1];
        

        void Start()
        {
            _level = new Level
            {
                maxLevelNumber = _levelConfigs.Count,
                successPerLevel = _levelConfigs[0].RoundsPerLevel,
                OnLevelChange = LevelChanged
            };
            _rules.AddRange(Config.AddedRules);

            _cts = new CancellationTokenSource();
            GameLoop(_cts.Token).Forget();
        }

        private void LevelChanged(int previousLevel, int nextLevel)
        {
            _rules.AddRange(Config.AddedRules);
            _level.successPerLevel = Config.RoundsPerLevel;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid GameLoop(CancellationToken ct)
        {
            while (true)
            {
                var startTime = Time.time;
                var item = GenerateItem();

                _director.Play(_animMain);
                _directorChild.playableAsset = null;
                _audioSource.clip = _audioMain;
                _audioSource.Play();
                _itemParent.gameObject.SetActive(true);

                await UniTask.Delay(TimeSpan.FromSeconds(_inputTime - _inputTimeBefore), cancellationToken: ct);

                var decisionDuration = _inputTimeBefore + _inputTimeAfter;
                var (playerAction, elapsed) = await WaitForInputOrTimeout(decisionDuration, ct);
                var decisionRemaining = decisionDuration - elapsed;
                var correctAction = DoesMatchRule(item) ? Decision.Smash : Decision.Pass;

                if (correctAction == playerAction)
                {
                    _level.Success();
                }
                else
                {
                    _level.Fail();
                }

                Debug.Log($"{playerAction} == {correctAction}, DIST: {Math.Abs(elapsed - _inputTimeBefore)}");
                if (playerAction == Decision.Smash)
                {
                    _directorChild.playableAsset = _animSmash;
                    _directorChild.Play();
                    _audioSecondary.clip = _audioSmash;
                    _audioSecondary.Play();
                }
                else if (playerAction == Decision.Pass)
                {
                    _directorChild.playableAsset = _animPass;
                    _directorChild.Play();
                    _audioSecondary.clip = _audioPass;
                    _audioSecondary.Play();
                }

                var curTime = Time.time;
                await UniTask.Delay(TimeSpan.FromSeconds(startTime + _timePerRound - curTime), cancellationToken: ct);

                Destroy(item.gameObject);

                if (ct.IsCancellationRequested)
                    return;
            }
        }

        private Item GenerateItem()
        {
            var itemPrefab = Config.ItemPrefabs.PickRandom();
            var item = Instantiate(itemPrefab, _itemParent).Compose();
            
            return item;
        }

        
        private bool DoesMatchRule(Item item)
        {
            return _rules.Any(rule => item.Match(rule.property));
        }

        private static async UniTask<(Decision, float ElapsedSeconds)> WaitForInputOrTimeout(
            float timeoutSeconds = 5f,
            CancellationToken cancellationToken = default)
        {
            float elapsed = 0f;

            while (elapsed < timeoutSeconds)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    return (Decision.Smash, elapsed);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    return (Decision.Pass, elapsed);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.deltaTime;
            }

            // Timed out
            return (Decision.None, elapsed);
        }
    }
}