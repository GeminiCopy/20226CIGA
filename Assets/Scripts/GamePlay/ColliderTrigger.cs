using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventSet
{
    None,
    QuitGame,
    StartGame,
    NextStage,
}
[RequireComponent(typeof(BoxCollider2D))]
public class ColliderTrigger : MonoBehaviour
{
    public EventSet eventSet;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            switch(eventSet)
            {
                case EventSet.QuitGame:
                    Application.Quit();
                    break;
                case EventSet.NextStage:
                case EventSet.StartGame:
                    //SceneMgr.Instance.LoadScene("Stage1");
                    //用这个使得关卡正常切换
                    TypeEventSystem.Inst.Invoke<CompleteCurrentStageEvent>();
                    break;
                case EventSet.None:
                    break;
            }
        }
    }
}
