using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//游戏管理器
public class GameManager : SingletonMono<GameManager>
{

    string EndResourcePath = "UI/Prefabs/GameOverPanel";//结束面板的路径
    string DeathMusicPath = "Music/player_dead";
    public AudioClip deathSound;
    GameObject GameOverPanelPrefab;//结束面板prefeb
    GameObject GameOverPanelPrefabInstance;//实例化的面板
    GameObject PlayerOut;
    public GameObject RespawnPoint = null;//玩家重生时传送的位置,先用vector3占着，你们想修改成obj或者其他的都行

    public float RestartCounter, RestartCount;
    private bool isDead;
    protected override void Awake()
    {
        base.Awake();
        Init();

    }
    private void Update()
    {
        if (isDead)//自动重生
        {
            RestartCounter -= Time.deltaTime;
            if (RestartCounter <= 0)
            {
                ReStart();
            }
        }
    }
    private void Init()
    {
        RestartCount = 2f;//重新开始时间
        RestartCounter = RestartCount;


        PlayerOut = transform.Find("PlayerOut").gameObject;
        //SetRespawnPoint();


    }
    //角色死亡时调用
    public void OnDie()
    {
        UIMgr.Instance.ShowPanel<GameOverPanel>();
        isDead = true;
        MusicMgr.Instance.PlaySound(DeathMusicPath, false, true);
        Debug.Log(" 角色死亡");
    }

    public void SetRespawnPoint()
    {
        RespawnPoint = transform.Find("StartPoint").gameObject;
    }
    public void ReStart()
    {
        isDead = false;
        RestartCounter = RestartCount;//重置自动死亡时间
        //todo执行重生逻辑
        if (RespawnPoint == null)
        {
            Debug.Log("重生错误");
        }

        UIMgr.Instance.HidePanel<GameOverPanel>();
        
    }
    
}
