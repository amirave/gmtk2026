using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
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
        [SerializeField] private RuleListView _rulelistView;

        [Header("Animation")] [SerializeField] private float _inputTime = 1.5f + 0.25f;
        [SerializeField] private float _inputTimeBefore = 0.25f;
        [SerializeField] private float _inputTimeAfter = 0.25f;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private PlayableDirector _directorChild;
        [SerializeField] private TimelineAsset _animMain;
        [SerializeField] private TimelineAsset _animSmash;
        [SerializeField] private TimelineAsset _animPass;
        [SerializeField] private Transform _itemParent;

        [Header("Audio")] [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _audioSecondary;
        [SerializeField] private AudioClip _audioMain;
        [SerializeField] private AudioClip _audioSmash;
        [SerializeField] private AudioClip _audioPass;

        [Header("Settings")] [SerializeField] private float _timePerRound = 2;
        [SerializeField] private List<LevelConfig> _levelConfigs;

        [Header("UI")] [Header("Win Screen")] [SerializeField]
        private NextLevelScreen _nextLevelScreen;

        [Header("Lose Screen")] [SerializeField]
        private LoseLevelScreen _loseLevelScreen;

        private Stack<List<Rule>> _ruleHistory;
        private Level _level;
        private CancellationTokenSource _cts;

        private LevelConfig Config => _levelConfigs[_level.levelNumber - 1];

        private List<Rule> CurrentRules =>
            _ruleHistory.Count != 0 ? _ruleHistory.Peek() : new List<Rule>(_levelConfigs.First().AddedRules);

        void Start()
        {
            _ruleHistory = new();
            // Time.timeScale = 0.5f;
            // _audioSource.pitch = 0.5f;

            _level = new Level
            {
                maxLevelNumber = _levelConfigs.Count,
                successPerLevel = _levelConfigs[0].RoundsPerLevel,
            };

            _rulelistView.ClearRules();
            foreach (var rule in CurrentRules)
            {
                _rulelistView.AddRuleView(0, rule).Forget();
            }

            _cts = new CancellationTokenSource();
            GameLoop(_cts.Token).Forget();
        }

        private async UniTask PlayLevelPassed(Rule chosenRule)
        {
            _nextLevelScreen.gameObject.SetActive(true);
            await _nextLevelScreen.Show(chosenRule);
            _nextLevelScreen.gameObject.SetActive(false);

            var prevRules = CurrentRules;
            
            var rules = new List<Rule>(CurrentRules) { chosenRule };
            _ruleHistory.Push(rules);

            _rulelistView.ClearRules();
            foreach (var rule in prevRules)
            {
                _rulelistView.AddRuleView(0, rule, false).Forget();
            }

            await _rulelistView.AddRuleView(0, chosenRule, true);

            _level.successPerLevel = Config.RoundsPerLevel;
        }

        private async UniTask PlayLevelFailed()
        {
            _loseLevelScreen.gameObject.SetActive(true);
            await _loseLevelScreen.Show();
            _loseLevelScreen.gameObject.SetActive(false);

            // _ruleHistory.Pop();
            _ruleHistory.Clear();
            
            _rulelistView.ClearRules();
            foreach (var rule in CurrentRules)
            {
                _rulelistView.AddRuleView(0, rule).Forget();
            }
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
                var correctAction = DoesMatchRule(item) ? Decision.Smash : Decision.Pass;

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

                if (correctAction == playerAction)
                {
                    var movedToNextLevel = _level.Success();
                    if (movedToNextLevel)
                    {
                        if (_level.levelNumber < _levelConfigs.Count - 1)
                        {
                            await PlayLevelPassed(Config.AddedRules[0]);
                        }
                        else
                        {
                            await PlayWin();
                        }
                    }
                }
                else
                {
                    var didLevelDecrease =  _level.Fail();
                    if (didLevelDecrease)
                    {
                        await PlayLevelFailed();
                    }
                }

                Destroy(item.gameObject);

                if (ct.IsCancellationRequested) return;
            }
        }

        private async UniTask PlayWin()
        {
        }

        private Item GenerateItem()
        {
            var itemPrefab = Config.ItemPrefabs.PickRandom();
            var item = Instantiate(itemPrefab, _itemParent).Compose();

            return item;
        }

        private bool DoesMatchRule(Item item)
        {
            return CurrentRules.Any(rule => rule.MatchItem(item));
        }
        
        private static async UniTask<(Decision, float ElapsedSeconds)> WaitForInputOrTimeout(float timeoutSeconds = 5f,
            CancellationToken cancellationToken = default)
        {
            float elapsed = 0f;

            while (elapsed < timeoutSeconds)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    return (Decision.Smash, elapsed);
                }

                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow))
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