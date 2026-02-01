using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2 : MonoBehaviour
{
    private void Start()
    {
        OnStage1Start();
    }
    private void OnStage1Start()
    {
        MusicMgr.Instance.StopBKMusic();
        MusicMgr.Instance.PlayerBKMusic("Music/ggj_1 - Mid");
        CameraManager.Instance.Clear();
        CameraManager.Instance.AddNewSubCamera();
        //GameObject.Find("BlankPanel").GetComponent<BasePanel>().HideMe();
        UIMgr.Instance.HidePanel<BlankPanel>();
    }
}
