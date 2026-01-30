using System;
using System.Collections;

public class StageManager : Singleton<StageManager>
{
    public override void Init()
    {
        base.Init();
        TypeEventSystem.Inst.Register<CompleteStageEvent>(OnCompleteStage);
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        
        TypeEventSystem.Inst.UnRegister<CompleteStageEvent>(OnCompleteStage);
    }
    
    private void OnCompleteStage(CompleteStageEvent obj)
    {
        string sceneName = obj switch
        {
            _ => "1"
        };
        TypeEventSystem.Inst.Invoke(new LoadSceneEvent()
        {
            sceneName = sceneName,
        });
    }
}