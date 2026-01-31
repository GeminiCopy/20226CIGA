using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

[Serializable]
public struct Dialog
{
    public int Id;
    public float Time;
    public string Content;
    public int? NextId;
    public string Target;
}

public class DialogManager : Singleton<DialogManager>
{
    private Dictionary<string, DialogController> _registry = new();
    private Dictionary<int, Dialog> _curDialogs;
    public bool _isPlaying;
    public bool Register(string key, DialogController c) => _registry.TryAdd(key, c);
    public bool UnRegister(string key) => _registry.Remove(key);
    public void Load(string key)
    {
        var textasset = ResourcesMgr.Instance.Load<TextAsset>($"Dialog/{key}");
        var list = textasset.text.FromJson<List<Dialog>>();
        _curDialogs = list.ToDictionary(x => x.Id);
    }
    public void Play(int startFrom = 1)
    {
        if (_isPlaying)
        {
            return;
        }
        
        PlayLoopAsync(startFrom).Forget();
    }
    
    private async UniTaskVoid PlayLoopAsync(int startId)
    {
        _isPlaying = true;
        int? currentId = startId;

        DialogController lastController = null;

        while (currentId != null)
        {
            if (!_curDialogs.TryGetValue(currentId.Value, out var data)) break;

            if (_registry.TryGetValue(data.Target, out DialogController curController))
            {
                if (lastController != null && lastController != curController)
                {
                    lastController.Close();
                }

                await curController.ShowDialogAsync(data);
                
                lastController = curController;
            }
            else
            {
                Debug.LogError($"找不到 Target: {data.Target} 的 DialogActor，请检查场景物体或 ID 配置");
                break; 
            }

            currentId = data.NextId;
        }

        if (lastController != null)
        {
            lastController.Close();
        }

        _isPlaying = false;
        Debug.Log("Dialog Finished");
    }
}

public class DialogController : MonoBehaviour
{
    [Header("UI Components")]
    public Canvas canvas;          // 用于控制显示/隐藏
    public TextMeshProUGUI tmp;    // 文本组件

    // 内部状态
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
            await UniTask.Delay(100);
        }
        _isTyping = false;
        
        await UniTask.WaitForSeconds(dialog.Time);
    }

    public void Close()
    {
        if (canvas != null) canvas.enabled = false;
    }
}