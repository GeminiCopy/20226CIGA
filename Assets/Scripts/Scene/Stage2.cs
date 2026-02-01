using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2 : MonoBehaviour
{
    private void Start()
    {

        UIMgr.Instance.ShowPanel<BlankPanel>(callback: OnStage1Start);
        PlayerManager.Instance.player1=GameObject.Find("PlayerIn").GetComponent<PlayerController2D>();
        PlayerManager.Instance.player2 = GameObject.Find("PlayerOut").GetComponent<PlayerController2D>();
    }
    private void OnStage1Start(BlankPanel arg0)
    {

        CameraManager.Instance.Clear();
        CameraManager.Instance.AddNewSubCamera();
        CameraManager.Instance.AddNewSubCamera();
        GameObject.Find("BlankPanel").GetComponent<BasePanel>().HideMe();
    }
}
