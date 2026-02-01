using System;
using UnityEngine;

public class Stage3 : MonoBehaviour
{
    private bool moreCamera;
    private void Start()
    {
        MusicMgr.Instance.StopBKMusic();
        MusicMgr.Instance.PlayerBKMusic("ggj_1 - Brighter");
        CameraManager.Instance.Clear();
        CameraManager.Instance.AddNewSubCamera();
        moreCamera = false;
    }

    private void Update()
    {
        if (moreCamera) return;

        var pos = PlayerManager.Instance.player1.transform.position;

        if (pos.y > 3.5 && pos.x > -7)
        {
            CameraManager.Instance.AddNewSubCamera();
            CameraManager.Instance.AddNewSubCamera();
            moreCamera = true;
        }
    }
}