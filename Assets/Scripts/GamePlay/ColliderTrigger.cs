using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EventSet
{
    None,
    QuitGame,
    NextStage,
}
[RequireComponent(typeof(BoxCollider2D))]
public class ColliderTrigger : MonoBehaviour
{
    public EventSet eventSet;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("tri");
        if(collision.CompareTag("Player"))
        {
            Debug.Log("suc");
            switch(eventSet)
            {
                case EventSet.QuitGame:
                    Application.Quit();
                    break;
                case EventSet.NextStage:
                    int currentIndex = SceneManager.GetActiveScene().buildIndex;
                    int nextIndex = currentIndex + 1;

                    // 检查是否超出范围
                    if (nextIndex < SceneManager.sceneCountInBuildSettings)
                    {
                        // 通过索引获取场景路径，然后提取场景名
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        Debug.Log($"{sceneName} + path{scenePath}");
                        SceneMgr.Instance.LoadScene(sceneName);
                        GameManager.Instance.ReStart();
                    }
                    break;
                case EventSet.None:
                    break;
            }
        }
    }
}
