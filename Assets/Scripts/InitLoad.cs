using System;
using UnityEngine;

public class InitLoad : MonoBehaviour
{
    private void Start()
    {
        SceneMgr.Instance.LoadScene("StartScene");
    }
}