using System;


public struct LoadSceneEvent
{
    public string sceneName;
    public Action onCompleted;
}
public struct CompleteCurrentStageEvent
{
    
}