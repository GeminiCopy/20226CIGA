using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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