using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.Find("SceneLoadManager"))
        {
            DestroyImmediate(gameObject);
            return;
        }
        TypeEventSystem.Inst.Register<LoadSceneEvent>(OnLoadScene);
    }
    private void OnLoadScene(LoadSceneEvent obj)
    {
        StartCoroutine(LoadScene(obj.sceneName, obj.onCompleted));
    }
    private IEnumerator LoadScene(string sceneName,
        Action onCompleted = null)
    {
        var s = SceneManager.LoadSceneAsync(sceneName);
        yield return s;

        onCompleted?.Invoke();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        Rect _areaRect = new Rect(20, 20, 150, 200);
        // 1. 开始一个自动布局区域
        GUILayout.BeginArea(_areaRect);
        // --- 标题 (可选) ---
        GUILayout.Label("调试菜单");
        // --- 按钮 1 ---
        // GUILayout.Button 返回 true 表示按钮被点击
        if (GUILayout.Button("切换", GUILayout.Height(40)))
        {
            TypeEventSystem.Inst.Invoke<CompleteCurrentStageEvent>();
        }

        // 2. 结束区域
        GUILayout.EndArea();
    }
#endif
}