using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int currentStage;
    private void Awake()
    {
        if (GameObject.Find("StageManager"))
        {
            DestroyImmediate(gameObject);
            return;
        }
        TypeEventSystem.Inst.Register<CompleteCurrentStageEvent>(OnCompleteStage);
    }

    public void OnDestroy()
    {
        TypeEventSystem.Inst.UnRegister<CompleteCurrentStageEvent>(OnCompleteStage);
    }
    
    private void OnCompleteStage(CompleteCurrentStageEvent obj)
    {
        currentStage++;
        string sceneName = $"Stage{currentStage}";
        TypeEventSystem.Inst.Invoke(new LoadSceneEvent()
        {
            sceneName = sceneName,
        });
    }
}