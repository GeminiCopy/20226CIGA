using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeInTime = 2.0f;  // 渐入时间
    public float fadeOutTime = 2.0f; // 渐出时间
    
    [Header("Music Paths")]
    public string[] musicPaths = new string[] { "1", "2", "3" }; // 不需要.mp3扩展名
    
    private int currentMusicIndex = 0;
    private bool isTransitioning = false;
    private MusicMgr musicManager;

    void Start()
    {
        // 获取MusicMgr单例
        musicManager = MusicMgr.Instance;
        
        // 自动播放第一个音乐
        //StartCoroutine(AutoPlayFirstMusic());
    }

    IEnumerator AutoPlayFirstMusic()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (musicPaths.Length > 0)
        {
            currentMusicIndex = 0;
            musicManager.PlayerBKMusic(musicPaths[currentMusicIndex], 0f, fadeInTime); // 第一个音乐不渐出，直接渐入
            Debug.Log($"开始播放: {musicPaths[currentMusicIndex]}");
        }
    }

    void Update()
    {
        // 监听P键切换音乐
        if (Input.GetKeyDown(KeyCode.P) && !isTransitioning)
        {
            PlayNextMusic();
        }
    }

    void PlayNextMusic()
    {
        if (isTransitioning) return;

        // 计算下一个音乐索引（循环）
        currentMusicIndex = (currentMusicIndex + 1) % musicPaths.Length;
        
        Debug.Log($"切换到音乐 [{currentMusicIndex + 1}/{musicPaths.Length}]: {musicPaths[currentMusicIndex]}");

        //string nextMusicPath = musicPaths[currentMusicIndex];
        //MusicMgr.Instance.PlayerBKMusic(nextMusicPath);
        
        // 使用MusicMgr的渐出渐入功能播放下一个音乐
        StartCoroutine(TransitionToNextMusic());
    }

    IEnumerator TransitionToNextMusic()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        string nextMusicPath = musicPaths[currentMusicIndex];
        
        // 使用MusicMgr的PlayerBKMusic方法，自动处理渐出渐入
        musicManager.PlayerBKMusic(nextMusicPath, fadeOutTime, fadeInTime);
        
        isTransitioning = false;
    }
}