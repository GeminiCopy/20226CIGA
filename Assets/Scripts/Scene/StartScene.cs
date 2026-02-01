using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    private string path = "Music/2";
    private void Start()
    {
        MusicMgr.Instance.PlayerBKMusic(path);
    }
    public void AddCamera()
    {
        CameraManager.Instance.AddNewSubCamera();
    }
   
}
