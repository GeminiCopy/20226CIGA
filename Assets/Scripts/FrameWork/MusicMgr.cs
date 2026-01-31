using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音乐音效管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    // 背景音乐组件
    internal AudioSource bkMusic = null;
    // 背景音乐音量
    public float bkMusicValue = 0.1f;

    // 音效组件存储集合
    private List<AudioSource> soundList = new List<AudioSource>();
    // 音效音量大小
    private float soundValue = 0.1f;
    // 判断音效是否暂停
    private bool soundIsPlay = true;

    // 音效池化预制体名称
    private const string SOUND_POOL_PREFAB = "Sounds/Prefabs/soundObj";

    private MusicMgr() 
    {
        // 注册Update给MonoMgr 使用Update每帧执行
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    
    private void Update()
    {
        if (!soundIsPlay) return;

        // 暂停关闭所有音效，判断是否还有音效在播放，没有的话全部移除
        // 为了避免垃圾回收清理对象，我们不是真的移除
        for(int i = soundList.Count -1;i>=0;i--)
        {
            if (!soundList[i].isPlaying)
            {
                // 音效播放完毕，音频片取空
                soundList[i].clip = null;
                // 音频对象推入池中
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                // 从List中移除
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音乐资源路径</param>
    public void PlayerBKMusic(string path)
    {
        // 如果没有关联AudioSource组件，动态创建
        if(bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        // 异步加载背景音乐资源，开始播放背景音乐
        ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (AudioClip) =>
        {
            bkMusic.clip = AudioClip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 播放背景音乐（带渐出渐入效果）
    /// </summary>
    /// <param name="path">音乐资源路径</param>
    /// <param name="fadeOutTime">渐出时间（秒）</param>
    /// <param name="fadeInTime">渐入时间（秒）</param>
    public void PlayerBKMusic(string path, float fadeOutTime, float fadeInTime)
    {
        // 通过MonoMgr启动渐出渐入协程
        MusicFadeHelper.StartFadeOutInMusic(this, path, fadeOutTime, fadeInTime);
    }

    /// <summary>
    /// 直接设置背景音乐音量（立即变化，无渐变）
    /// </summary>
    /// <param name="volume">目标音量</param>
    public void SetBKMusicVolume(float volume)
    {
        bkMusicValue = Mathf.Clamp01(volume);
        if (bkMusic != null)
        {
            bkMusic.volume = bkMusicValue;
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Pause();
    }

    /// <summary>
    /// 恢复背景音乐播放
    /// </summary>
    public void ResumeBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.UnPause();
    }

    /// <summary>
    /// 改变背景音乐音量
    /// </summary>
    /// <param name="value">音量值（0-1）</param>
    public void ChangeBKMusicValue(float value)
    {
        bkMusicValue = Mathf.Clamp01(value);
        if (bkMusic == null) return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音效路径</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callback">返回结果后的回调</param>
    public void PlaySound(string path, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callback = null)
    {
        // 从对象池获取音效组件对应的对象
        AudioSource source = PoolMgr.Instance.GetObj(SOUND_POOL_PREFAB).GetComponent<AudioSource>();

        // 根据传入判断音效类型，播放对应音效
        if (isSync)
        {
            // 同步加载音效资源
            AudioClip audioClip = ResourcesMgr.Instance.Load<AudioClip>(path);

            // 为防止或播放音效，取自当前正在播放的那条，播放新的获取完成后停止
            source.Stop();

            source.clip = audioClip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            // 记录到集合中，之后用于判断是否停止
            // 可能从对象池取出的，可能已经被其他使用了（被占用了），
            // 所以要重复去检测加入集合
            if(!soundList.Contains(source))
                    soundList.Add(source);
        }
        else
        {
            // 异步加载音效资源
            ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (AudioClip) =>
            {
                source.clip = AudioClip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                // 记录到集合中，之后用于判断是否停止
                soundList.Add(source);
                // 根据需要给外部使用
                callback?.Invoke(source);
            });
        }
    }

    /// <summary>
    /// 停止播放指定音效
    /// </summary>
    /// <param name="source">音效播放组件</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            // 停止播放
            source.Stop();
            // 从List中移除
            soundList.Remove(source);

            // 音效播放完毕，音频片取空
            source.clip = null;
            // 推入对象池
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效音量
    /// </summary>
    /// <param name="value">音量值（0-1）</param>
    public void ChangeSoundValue(float value)
    {
        soundValue = Mathf.Clamp01(value);
        foreach (AudioSource source in soundList)
        {
            source.volume = value;
        }
    }

    /// <summary>
    /// 控制音效暂停或播放音效
    /// </summary>
    /// <param name="isPlay">是否是继续播放，true为播放，false为暂停</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if(isPlay)
        {
            foreach (AudioSource source in soundList)
            {
                source.Stop();
            }
        }
        else
        {
            foreach (AudioSource source in soundList)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// 清除音效记录，之后时 !!!不要在栈底返回之前去调用!!!
    /// </summary>
    public void ClearSound()
    {
        for(int i = 0;i<soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        // 清除音效列表
        soundList.Clear();
    }

    /// <summary>
    /// 获取背景音乐当前音量
    /// </summary>
    /// <returns>当前音量值</returns>
    public float GetBKMusicVolume()
    {
        return bkMusicValue;
    }

    /// <summary>
    /// 获取音效当前音量
    /// </summary>
    /// <returns>当前音量值</returns>
    public float GetSoundVolume()
    {
        return soundValue;
    }

    /// <summary>
    /// 检查背景音乐是否在播放
    /// </summary>
    /// <returns>是否在播放</returns>
    public bool IsBKMusicPlaying()
    {
        return bkMusic != null && bkMusic.isPlaying;
    }

    /// <summary>
    /// 获取当前播放的音效数量
    /// </summary>
    /// <returns>音效数量</returns>
    public int GetCurrentSoundCount()
    {
        return soundList.Count;
    }
}