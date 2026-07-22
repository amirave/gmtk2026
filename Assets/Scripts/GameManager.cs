using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float _timePerRound = 3;
        [SerializeField] private List<Item> _itemPrefabs;
        [SerializeField] private List<Rule> _rules;

        private CancellationTokenSource _cts;

        void Start()
        {
            _cts = new CancellationTokenSource();
            RunForeverAsync(2f, _cts.Token).Forget();
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public async UniTaskVoid RunForeverAsync(float waitSeconds, CancellationToken ct)
        {
            while (true)
            {
                var item = GenerateItem();

                await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: ct);

                var shouldDestroy = Input.GetKey(KeyCode.Space);
                var match = DoesMatchRule(item);

                Debug.Log($"{shouldDestroy}, {match}");
                // if (userChoice == match)
                // {
                //     Debug.Log("CORRECT!!");
                // }
                // else
                // {
                //     Debug.Log("WRONGG");
                // }

                Destroy(item.gameObject);

                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);

                if (ct.IsCancellationRequested)
                    return;
            }
        }

        Item GenerateItem()
        {
            var itemPrefab = _itemPrefabs.PickRandom();
            return Instantiate(itemPrefab, transform);
        }

        bool DoesMatchRule(Item item)
        {
            foreach (var rule in _rules)
            {
                // Does the item contain the same property of rule with the expected value?
                var match = item.Properties.Any(prop => prop.Match(rule.property));
                if (!match) return false;
            }

            return true;
        }
    }
}