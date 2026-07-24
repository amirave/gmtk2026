using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class RuleListView : MonoBehaviour
    {
        [SerializeField] private RuleView _ruleViewPrefab;
        [SerializeField] private Transform _container;

        private void Start()
        {
            // AddRuleView(0, new Rule(new ShapeProperty(ShapeType.Circle)));
            // AddRuleView(1, new Rule(new ColorProperty(ColorType.Red)));
        }
        

        public async UniTask AddRuleView(int id, Rule rule, bool animate = true)
        {
            var ruleView = Instantiate(_ruleViewPrefab, _container);
            ruleView.Populate(id, rule);
            if (animate)
                await ruleView.AnimateIn();
        }

        public void RemoveRuleView(int id)
        {
            foreach (Transform child in _container)
            {
                var view = child.GetComponent<RuleView>();
                if (view.Id == id)
                {
                    Destroy(view.gameObject);
                }
            }
        }
    }
}