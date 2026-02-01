using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    public string path = "";
    private void Start()
    {
        MusicMgr.Instance.PlayerBKMusic(path);
        UIMgr.Instance.ShowPanel<BlankPanel>(callback:OnStage1Start);
        PlayerManager.Instance.player1.CanMove = false;
        PlayerManager.Instance.player2.CanMove = false;
    }
    private void OnStage1Start(BlankPanel arg0)
    {
        DialogManager.Inst.Load("tbstg1dialog");
        DialogManager.Inst.Play(onComplete:() =>
        {
            UIMgr.Instance.HidePanel<BlankPanel>();
            PlayerManager.Instance.player1.CanMove = true;
            PlayerManager.Instance.player2.CanMove = true;
            CameraManager.Instance.Clear();
            CameraManager.Instance.AddNewSubCamera();
            DialogManager.Inst.Play(3);
        });
    }
    private void OnStage1FinalDialog()
    {
        DialogManager.Inst.Play(5,onComplete:(() =>
        {
            PlayerManager.Instance.player1.CanMove = true;
            PlayerManager.Instance.player2.CanMove = true;
        }));
    }
}
