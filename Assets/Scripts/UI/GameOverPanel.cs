using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : BasePanel
{
    public override void HideMe()
    {
        Debug.Log("�رս������");
        gameObject.SetActive(false);
    }

    public override void ShowMe()
    {
        Debug.Log("�����������");
        gameObject.SetActive(true);
    }

    
}
