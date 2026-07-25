using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    private UniTaskCompletionSource<bool> _tcs;

    public UniTask<bool> WaitForClick()
    {
        _tcs = new UniTaskCompletionSource<bool>();
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
        return _tcs.Task;
    }

    private void OnClicked()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClicked);
        _tcs.TrySetResult(true);
    }
}