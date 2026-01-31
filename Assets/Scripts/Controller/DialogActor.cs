using UnityEngine;

public class DialogActor : MonoBehaviour
{
    public string id; // target
    public DialogController myBubble; 
    private void Start()
    {
        // 注册自己
        DialogManager.Inst.Register(id,myBubble);
    }

    private void OnDestroy()
    {
        // 反注册
        if (DialogManager.Inst != null)
        {
            DialogManager.Inst.UnRegister(id);
        }
    }
}