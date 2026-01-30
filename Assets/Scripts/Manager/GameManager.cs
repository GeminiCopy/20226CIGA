using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//游戏管理器
public class GameManager:MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");//没有就创建
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance; 
        }
    }
    string resourcePath = "Prefeb/UI/GameOverPanel";//结束面板的路径
    GameObject GameOverPanelPrefab;//结束面板prefeb
    GameObject GameOverPanelPrefabInstance;//实例化的面板
    public Vector3 RespawnPoint=Vector3.zero;//玩家重生时传送的位置,先用vector3占着，你们想修改成obj或者其他的都行

    public float RestartCounter, RestartCount;
    private bool isDead;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
        GameObject GameOverPanelPrefab = Resources.Load<GameObject>(resourcePath);//加载面板
    }
    //角色死亡时调用
    public void OnDie()
    {
        if(GameOverPanelPrefabInstance==null)
        {
            GameOverPanelPrefabInstance=Instantiate(GameOverPanelPrefab);//生成结束面板
        }

        GameOverPanelPrefabInstance.SetActive(true);
        isDead = true;
    }
    public void ReStart()
    {
        isDead = false;
        RestartCounter = RestartCount;//重置自动死亡时间
        //todo执行重生逻辑
    }
}
