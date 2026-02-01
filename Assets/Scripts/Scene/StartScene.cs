using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    
    private void Start()
    {
        MusicMgr.Instance.StopBKMusic();
        MusicMgr.Instance.PlayerBKMusic("Music/ggj_1 - SadStart");
    }
    public void AddCamera()
    {
        CameraManager.Instance.AddNewSubCamera();
    }
   
}
