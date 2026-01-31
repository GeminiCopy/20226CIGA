using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [Header("UI Components")]
    public Canvas canvas;          // 用于控制显示/隐藏
    public TMP_Text tmp;    // 文本组件
    
    private bool _isTyping;
    private bool _skipTyping;
    private UniTaskCompletionSource _clickSignal;

    private void Awake()
    {
        if (canvas != null) canvas.enabled = false;
    }

    // 简单的 LookAt 相机逻辑，防止气泡侧对着屏幕看不清
    private void LateUpdate()
    {
        if (canvas.enabled && Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    public async UniTask ShowDialogAsync(Dialog dialog)
    {
        if (canvas != null) canvas.enabled = true;
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
        if (canvas != null) canvas.enabled = false;
    }
}