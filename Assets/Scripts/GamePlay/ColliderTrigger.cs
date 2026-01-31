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
        if(collision.tag=="Player")
        {
            //´¥·¢Åö×²ÊÂ¼þ
            switch(eventSet)
            {
                case EventSet.QuitGame:
                    Application.Quit();
                    break;
                case EventSet.StartGame:
                    SceneMgr.Instance.LoadScene("Stage1");
                    break;
                case EventSet.None:
                    break;
                    

                    
            }
        }
    }
}
