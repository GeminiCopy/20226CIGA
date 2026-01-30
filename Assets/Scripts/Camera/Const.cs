using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Const : MonoBehaviour
{
    private void Awake() 
    {
        //Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920,1080,FullScreenMode.Windowed);
    }
    public void RefreshScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
