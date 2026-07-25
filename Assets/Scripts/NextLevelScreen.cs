using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class NextLevelScreen : MonoBehaviour
    {
        [SerializeField] private RuleView _ruleView;
        [SerializeField] private CustomButton _okButton;

        public async UniTask Show(Rule rule)
        {
            _ruleView.Populate(0, rule);

            await _ruleView.AnimateIn();

            await _okButton.WaitForClick();
        }
    }
}