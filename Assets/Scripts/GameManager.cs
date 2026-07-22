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
        [SerializeField] private List<Rule> _rules;

        [SerializeField] private PlayableDirector _director;
        [SerializeField] private TimelineAsset _animIn;
        [SerializeField] private TimelineAsset _animSmash;
        [SerializeField] private TimelineAsset _animPass;
        [SerializeField] private Transform _itemParent;

        private CancellationTokenSource _cts;

        void Start()
        {
            _rules = new List<Rule>() { new Rule() };
            var prop = new ColorProperty();
            prop.possibleValues = new List<ColorType>() { ColorType.RED };
            _rules[0].property = prop;
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

                _director.Play(_anim_in);

                await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: ct);

                var shouldDestroy = Input.GetKey(KeyCode.Space);
                var match = DoesMatchRule(item);

                Debug.Log($"{shouldDestroy}, {match}");
                if (shouldDestroy)
                {
                    _director.Play(_anim_smash);
                }
                else
                {
                    _director.Play(_anim_pass);
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

        Item GenerateItem()
        {
            var itemPrefab = _itemPrefabs.PickRandom();
            return Instantiate(itemPrefab, _itemParent);
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