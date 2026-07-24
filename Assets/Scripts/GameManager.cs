using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Header("UI")]
        [Header("Win Screen")]
        [SerializeField] private GameObject _nextLevelScreen;
        [SerializeField] private List<RuleButton> _nextLevelButtons;

        [Header("Lose Screen")]
        [SerializeField] private GameObject _loseLevelScreen;
        [SerializeField] private LoseButton _loseRestartButton;
        [SerializeField] private LoseButton _loseQuitButton;

        private Stack<List<Rule>> _ruleHistory;
        private Level _level;
        private CancellationTokenSource _cts;

        private LevelConfig Config => _levelConfigs[_level.levelNumber - 1];
        private List<Rule> CurrentRules => _ruleHistory.Count != 0 ? _ruleHistory.Peek() : new List<Rule> (_levelConfigs.First().AddedRules);


        void Start()
        {
            _ruleHistory = new();

            _level = new Level
            {
                maxLevelNumber = _levelConfigs.Count,
                successPerLevel = _levelConfigs[0].RoundsPerLevel,
                wonLevelScreen = _nextLevelScreen,
                lostLevelScreen = _loseLevelScreen,
                lostRestartButton = _loseRestartButton,
                lostQuitButton = _loseQuitButton,
                winChoices = _nextLevelButtons,
            };

            _rulelistView.ClearRules();
            foreach (var rule in CurrentRules)
            {
                _rulelistView.AddRuleView(0, rule).Forget();
            }

            _cts = new CancellationTokenSource();
            GameLoop(_cts.Token).Forget();
        }

        private void LevelPassed(Rule chosenRule)
        {
            var rules = new List<Rule>(CurrentRules)
            {
                chosenRule
            };

            _ruleHistory.Push(rules);
            
            _rulelistView.ClearRules();
            foreach (var rule in CurrentRules)
            {
                _rulelistView.AddRuleView(0, rule).Forget();
            }
            _level.successPerLevel = Config.RoundsPerLevel;
        }

        private void LevelFailed()
        {
            _ruleHistory.Pop();
            _rulelistView.ClearRules();
            foreach (var rule in CurrentRules)
            {
                _rulelistView.AddRuleView(0, rule).Forget();
            }
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
                    var (movedToNextLevel, chosenRuleButton) = await _level.Success();
                    if (movedToNextLevel)
                    {
                        LevelPassed(chosenRuleButton.Rule);
                        Debug.Log($"Chosen new Rule: {chosenRuleButton.Rule}");
                    }
                }
                else
                {
                    var (didLevelDecrease, chosenLostButton) = await _level.Fail();
                    if (didLevelDecrease)
                    {
                        LevelFailed();
                    }

                    Debug.Log($"Chosen lost: {chosenLostButton.Value}");
                }

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
            Debug.Log($"CURRENT RULES: {CurrentRules.Select(rule => rule.ToString()).Aggregate((rules, rule1) => rules += rule1)}");
            return CurrentRules.Any(rule => item.Match(rule.property));
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