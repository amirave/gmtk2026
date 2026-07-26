using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class PopupScreen : MonoBehaviour
    {
        [SerializeField] private CustomButton _okButton;

        public async UniTask Show()
        {
            // Prevent accidental input
            await UniTask.WaitForSeconds(0.25f);
            await UniTask.WhenAny(_okButton.WaitForClick(), UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow), PlayerLoopTiming.Update));
        }

    }
}