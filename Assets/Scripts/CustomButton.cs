using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    private TaskCompletionSource<bool> _tcs;


    public Task<bool> WaitForClick()
    {
        _tcs = new TaskCompletionSource<bool>();

        gameObject.GetComponent<Button>().onClick.AddListener(OnClicked);
        return _tcs.Task;
    }

    private void OnClicked()
    {
        gameObject.GetComponent<Button>().onClick.RemoveListener(OnClicked);
        _tcs.TrySetResult(true);
    }
}