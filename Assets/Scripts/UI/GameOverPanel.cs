using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : BasePanel
{
    public override void HideMe()
    {
        Debug.Log("关闭结束面板");
        gameObject.SetActive(false);
    }

    public override void ShowMe()
    {
        Debug.Log("开启结束面板");
        gameObject.SetActive(true);
    }

    
}
