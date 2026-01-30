using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private void Awake()
    {
        TypeEventSystem.Inst.Register<LoadSceneEvent>(OnLoadScene);
        DontDestroyOnLoad(gameObject);
    }
    private void OnLoadScene(LoadSceneEvent obj)
    {
        StartCoroutine(LoadScene(obj.sceneName,obj.onCompleted));
    }
    private IEnumerator LoadScene(string sceneName, 
        Action onCompleted = null)
    {
        var s = SceneManager.LoadSceneAsync(sceneName);
        yield return s;
        onCompleted?.Invoke();
    }
}