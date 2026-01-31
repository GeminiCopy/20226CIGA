using UnityEngine;

/// <summary>
/// MusicMgr StartCoroutine错误修复验证测试
/// </summary>
public class StartCoroutineFixTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== StartCoroutine错误修复验证测试 ===");
        
        try
        {
            // 测试1: 验证MusicMgr单例
            var musicMgr = MusicMgr.Instance;
            if (musicMgr != null)
            {
                Debug.Log("✅ MusicMgr单例获取成功");
                
                // 测试2: 验证基本功能
                musicMgr.ChangeBKMusicValue(0.5f);
                musicMgr.ChangeSoundValue(0.8f);
                Debug.Log("✅ 基本方法调用成功");
                
                // 测试3: 验证渐出渐入方法存在
                var fadeMethod = musicMgr.GetType().GetMethod("PlayerBKMusic", new[] { typeof(string), typeof(float), typeof(float) });
                if (fadeMethod != null)
                {
                    Debug.Log("✅ 渐出渐入PlayerBKMusic方法存在");
                }
                else
                {
                    Debug.LogError("❌ 渐出渐入PlayerBKMusic方法不存在");
                }
                
                // 测试4: 验证新方法调用（不会报错）
                Debug.Log("测试: 调用渐出渐入方法（即使音频文件不存在也不会报错）");
                musicMgr.PlayerBKMusic("Music/nonexistent", 1.0f, 1.5f);
                Debug.Log("✅ 渐出渐入方法调用成功（无报错）");
            }
            else
            {
                Debug.LogError("❌ MusicMgr单例获取失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 测试失败: {e.Message}");
            Debug.LogError($"堆栈跟踪: {e.StackTrace}");
        }
        
        Debug.Log("=== 修复验证完成 ===");
    }
    
    void Update()
    {
        // 按F键测试渐出渐入功能
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (MusicMgr.Instance != null)
            {
                Debug.Log("按键测试: 调用渐出渐入PlayerBKMusic");
                MusicMgr.Instance.PlayerBKMusic("Music/test", 1.0f, 2.0f);
            }
        }
        
        // 按G键测试普通播放
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (MusicMgr.Instance != null)
            {
                Debug.Log("按键测试: 调用普通PlayerBKMusic");
                MusicMgr.Instance.PlayerBKMusic("Music/test");
            }
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        GUILayout.Label("=== StartCoroutine错误修复测试 ===");
        
        GUILayout.Label("按F键测试渐出渐入PlayerBKMusic");
        GUILayout.Label("按G键测试普通PlayerBKMusic");
        
        if (MusicMgr.Instance != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("=== MusicMgr状态 ===");
            GUILayout.Label($"背景音乐音量: {MusicMgr.Instance.GetBKMusicVolume():F2}");
            GUILayout.Label($"音效音量: {MusicMgr.Instance.GetSoundVolume():F2}");
            GUILayout.Label($"背景音乐播放: {MusicMgr.Instance.IsBKMusicPlaying()}");
            GUILayout.Label($"当前音效数: {MusicMgr.Instance.GetCurrentSoundCount()}");
        }
        else
        {
            GUILayout.Label("MusicMgr实例为空");
        }
        
        GUILayout.EndArea();
    }
}