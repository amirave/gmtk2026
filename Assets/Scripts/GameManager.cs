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
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float _timePerRound = 3;
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

                _director.Play(_animIn);

                await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: ct);

                var shouldDestroy = Input.GetKey(KeyCode.Space);
                var match = DoesMatchRule(item);

                Debug.Log($"PRESSED SPACE: {shouldDestroy}, MATCH: {match}");
                if (shouldDestroy)
                {
                    _director.Play(_animSmash);
                }
                else
                {
                    _director.Play(_animPass);
                }
                // if (userChoice == match)
                // {
                //     Debug.Log("CORRECT!!");
                // }
                // else
                // {
                //     Debug.Log("WRONGG");
                // }

                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);
                
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
    }
}