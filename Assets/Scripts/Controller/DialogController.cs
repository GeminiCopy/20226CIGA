using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text tmp;    // 文本组件
    
    private bool _isTyping;
    private bool _skipTyping;

    private void Awake()
    {
        tmp.gameObject.SetActive(false);
    }

    public async UniTask ShowDialogAsync(Dialog dialog)
    {
        tmp.gameObject.SetActive(true);
        tmp.text = "";
        
        _isTyping = true;
        _skipTyping = false;

        foreach (char c in dialog.Content)
        {
            if (_skipTyping)
            {
                tmp.text = dialog.Content;
                break;
            }
            tmp.text += c;
            await UniTask.Delay(50);
        }
        _isTyping = false;
        
        await UniTask.WaitForSeconds(dialog.Time);
    }

    public void Close()
    {
        tmp.gameObject.SetActive(false);
    }
}