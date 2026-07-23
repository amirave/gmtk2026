using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Game
{
    public enum Decision
    {
        Smash,
        Pass
    }
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float _timePerRound = 3;
        [SerializeField] private float _inputTime = 1.5f + 0.25f;
        [SerializeField] private float _inputTimeBefore = 0.25f;
        [SerializeField] private float _inputTimeAfter = 0.25f;
        
        [SerializeField] private List<Item> _itemPrefabs;
        [SerializeReference] private List<Rule> _rules;

        [SerializeField] private PlayableDirector _director;
        [SerializeField] private TimelineAsset _animIn;
        [SerializeField] private TimelineAsset _animSmash;
        [SerializeField] private TimelineAsset _animPass;
        [SerializeField] private Transform _itemParent;

        private CancellationTokenSource _cts;

        void Start()
        {
            _rules = new List<Rule>() { new Rule(new ColorProperty(ColorType.Red)) };
            _cts = new CancellationTokenSource();
            GameLoop(2f, _cts.Token).Forget();
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public async UniTaskVoid GameLoop(float waitSeconds, CancellationToken ct)
        {
            while (true)
            {
                var item = GenerateItem();

                _director.Play(_animPass);

                // await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);

                await UniTask.Delay(TimeSpan.FromSeconds(_inputTime - _inputTimeBefore), cancellationToken: ct);

                var decisionDuration = _inputTimeBefore + _inputTimeAfter;
                var (playerAction, elapsed) = await WaitForInputOrTimeout(decisionDuration, ct);
                var decisionRemaining = decisionDuration - elapsed;
                var correctAction = DoesMatchRule(item) ? Decision.Smash : Decision.Pass;

                Debug.Log($"{playerAction}, {correctAction}, DIST: {Math.Abs(elapsed - _inputTimeBefore)}");
                if (playerAction == Decision.Smash)
                {
                    _director.Stop();
                    _director.playableAsset = _animSmash;
                    _director.time = _inputTime - _inputTimeBefore + elapsed;
                    _director.Play();
                    _director.Evaluate();
                }
                // if (userChoice == match)
                // {
                //     Debug.Log("CORRECT!!");
                // }
                // else
                // {
                //     Debug.Log("WRONGG");
                // }

                await UniTask.Delay(TimeSpan.FromSeconds(0.25f + decisionRemaining), cancellationToken: ct);

                Destroy(item.gameObject);

                if (ct.IsCancellationRequested)
                    return;
            }
        }

        private Item GenerateItem()
        {
            var itemPrefab = _itemPrefabs.PickRandom();
            var item = Instantiate(itemPrefab, _itemParent).Compose();
            
            return item;
        }

        
        private bool DoesMatchRule(Item item)
        {
            return _rules.Any(rule => item.Match(rule.property));
        }

        public static async UniTask<(Decision, float ElapsedSeconds)> WaitForInputOrTimeout(
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
            return (Decision.Pass, elapsed);
        }
    }
}