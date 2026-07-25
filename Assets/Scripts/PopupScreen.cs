using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class PopupScreen : MonoBehaviour
    {
        [SerializeField] private CustomButton _okButton;

        public async UniTask Show()
        {
            await UniTask.WhenAny(_okButton.WaitForClick(), UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow), PlayerLoopTiming.Update));
        }

    }
}