using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    public string path = "Music/2";
    private void Start()
    {
        MusicMgr.Instance.PlayerBKMusic(path);
    }
}
