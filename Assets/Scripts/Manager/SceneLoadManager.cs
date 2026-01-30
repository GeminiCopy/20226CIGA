using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public IEnumerator LoadScene(string sceneName, 
        Action onCompleted = null)
    {
        var s = SceneManager.LoadSceneAsync(sceneName);
        yield return s;
        onCompleted?.Invoke();
    }
}