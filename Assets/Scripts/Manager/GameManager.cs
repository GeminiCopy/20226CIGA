using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//游戏管理器
public class GameManager:SingletonAutoMono<GameManager>
{
    
    string resourcePath = "Prefeb/UI/GameOverPanel";//结束面板的路径
    GameObject GameOverPanelPrefab;//结束面板prefeb
    GameObject GameOverPanelPrefabInstance;//实例化的面板
    public Vector3 RespawnPoint=Vector3.zero;//玩家重生时传送的位置,先用vector3占着，你们想修改成obj或者其他的都行

    public float RestartCounter, RestartCount;
    private bool isDead;
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        if (isDead)//自动重生
        {
            RestartCount -= Time.deltaTime;
            if (RestartCount <= 0)
            {
                ReStart();
            }
        }
    }
    private void Init()
    {
        RestartCount = 2f;//重新开始时间
        RestartCounter = RestartCount;
        GameObject GameOverPanelPrefab = ResourcesMgr.Instance.Load<GameObject>(resourcePath);//加载面板
    }
    //角色死亡时调用
    public void OnDie()
    {
        UIMgr.Instance.ShowPanel<GameOverPanel>(E_UILayer.system);
        isDead = true;
    }
    public void ReStart()
    {
        isDead = false;
        RestartCounter = RestartCount;//重置自动死亡时间
        //todo执行重生逻辑
    }
}
