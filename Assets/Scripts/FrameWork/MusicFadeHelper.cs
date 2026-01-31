using UnityEngine;
using System.Collections;

/// <summary>
/// 音乐渐出渐入辅助类
/// </summary>
public class MusicFadeHelper : MonoBehaviour
{
    private MusicMgr musicManager;
    private AudioSource bkMusic;
    private float originalVolume;
    
    /// <summary>
    /// 启动渐出渐入效果
    /// </summary>
    /// <param name="manager">MusicMgr实例</param>
    /// <param name="path">音乐路径</param>
    /// <param name="fadeOutTime">渐出时间</param>
    /// <param name="fadeInTime">渐入时间</param>
    public static void StartFadeOutInMusic(MusicMgr manager, string path, float fadeOutTime, float fadeInTime)
    {
        // 创建临时GameObject和组件
        GameObject helperObj = new GameObject("MusicFadeHelper");
        MusicFadeHelper helper = helperObj.AddComponent<MusicFadeHelper>();
        
        // 开始渐出渐入协程
        helper.StartFadeProcess(manager, path, fadeOutTime, fadeInTime);
    }
    
    private void StartFadeProcess(MusicMgr manager, string path, float fadeOutTime, float fadeInTime)
    {
        musicManager = manager;
        
        // 获取或创建AudioSource
        if (musicManager.bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            musicManager.bkMusic = obj.AddComponent<AudioSource>();
        }
        
        bkMusic = musicManager.bkMusic;
        originalVolume = musicManager.bkMusicValue;
        
        // 启动协程
        StartCoroutine(FadeOutInMusicCoroutine(path, fadeOutTime, fadeInTime));
    }
    
    private IEnumerator FadeOutInMusicCoroutine(string path, float fadeOutTime, float fadeInTime)
    {
        // 如果当前有音乐正在播放，先渐出
        if (bkMusic != null && bkMusic.isPlaying && fadeOutTime > 0)
        {
            yield return StartCoroutine(FadeVolume(bkMusic, bkMusic.volume, 0f, fadeOutTime));
        }

        // 停止当前音乐
        if (bkMusic != null)
        {
            bkMusic.Stop();
        }

        // 异步加载新音乐
        AudioClip newClip = null;
        bool clipLoaded = false;
        
        ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (AudioClip) =>
        {
            newClip = AudioClip;
            clipLoaded = true;
        });

        // 等待音频加载完成
        while (!clipLoaded)
        {
            yield return null;
        }

        if (newClip != null)
        {
            // 设置新音乐
            bkMusic.clip = newClip;
            bkMusic.loop = true;
            bkMusic.volume = 0f; // 从0开始，用于渐入

            // 开始播放
            bkMusic.Play();

            // 渐入：从0升到原音量
            if (fadeInTime > 0)
            {
                yield return StartCoroutine(FadeVolume(bkMusic, 0f, originalVolume, fadeInTime));
            }
            else
            {
                // 如果没有渐入时间，直接设置到目标音量
                bkMusic.volume = originalVolume;
            }
        }
        else
        {
            Debug.LogWarning($"MusicFadeHelper: 无法加载音频文件: {path}");
            // 加载失败时，恢复原音量
            if (fadeOutTime > 0 && bkMusic != null)
            {
                yield return StartCoroutine(FadeVolume(bkMusic, bkMusic.volume, originalVolume, fadeOutTime * 0.5f));
            }
        }
        
        // 完成后销毁辅助对象
        Destroy(gameObject);
    }
    
    private IEnumerator FadeVolume(AudioSource audioSource, float startVolume, float endVolume, float duration)
    {
        if (audioSource == null || duration <= 0)
        {
            audioSource.volume = endVolume;
            yield break;
        }

        float elapsed = 0f;
        
        // 确保音频源存在且正在播放
        if (!audioSource.isPlaying && startVolume > 0 && endVolume > 0)
        {
            audioSource.Play();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 使用平滑曲线
            float volume = Mathf.Lerp(startVolume, endVolume, SmoothStep(t));
            audioSource.volume = volume;
            
            yield return null;
        }

        // 确保最终音量准确
        audioSource.volume = endVolume;
    }
    
    private float SmoothStep(float t)
    {
        // 使用SmoothStep函数实现平滑过渡
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
    
    void OnDestroy()
    {
        // 确保协程停止
        StopAllCoroutines();
    }
}