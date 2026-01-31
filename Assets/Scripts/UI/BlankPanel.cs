using System;
using UnityEngine;

public class BlankPanel : BasePanel
{
    public DialogController Dialog;
    public const string DialogName = "Blank";
    protected override void Awake()
    {
        DialogManager.Inst.Register(DialogName, Dialog);
        base.Awake();
    }
    private void OnDestroy()
    {
        DialogManager.Inst.UnRegister(DialogName);
    }
    public override void ShowMe()
    {
        
    }
    public override void HideMe()
    {
        
    }
}