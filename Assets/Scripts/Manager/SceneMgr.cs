using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }

    //同步切换场景方法
    public void LoadScene(string name,UnityAction callback = null)
    {
        //切换场景
        SceneManager.LoadScene(name);
        //调用回调
        callback?.Invoke();
    }

    //异步切换场景方法
    public void LoadSceneAsyn(string name,UnityAction callback = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, callback));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction callback = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //不停地在协同程序中每帧检测是否加载结束 如果加载结束就不会进这个循环每帧进行了
        while(!ao.isDone)
        {
            //在这里利用事件中心 每一帧将进度发送给想要得到的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, ao.progress);
            yield return 0;
        }
        //避免最后一帧直接结束了 没有同步1出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, 1);

        //加载结束
        callback?.Invoke();
    }
}
