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
    //背景音乐播放组件
    private AudioSource bkMusic = null;
    //背景音乐大小
    private float bkMusicValue = 0.1f;

    //管理正在播放的音效
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效音量大小
    private float soundValue = 0.1f;
    //判断音效播放是否暂停
    private bool soundIsPlay = true;

    private MusicMgr() 
    {
        //将Update函数让MonoMgr监听 使得Update每帧执行
        MonoMgr.Instance.AddUpateListener(Update);
    }
    private void Update()
    {
        if (!soundIsPlay) return;

        //不停地遍历容器 检测是否有音效播放完毕 播放完的销毁
        //为了避免边遍历边移除出现问题 我们采用逆向遍历
        for(int i = soundList.Count -1;i>=0;i--)
        {
            if (!soundList[i].isPlaying)
            {
                //音效播放完毕 将切片制空
                soundList[i].clip = null;
                //将其放入缓存池中
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                //将其从List中移除
                soundList.RemoveAt(i);
            }
        }
    }

    //播放背景音乐
    public void PlayerBKMusic(string path)
    {
        //如果没有挂载AudioSource的组件 动态创建
        if(bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //根据传入的背景音乐名字 来播放背景音乐
        ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (AudioClip) =>
        {
            bkMusic.clip = AudioClip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });

    }

    //停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Stop();
    }

    //暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null) return;
        bkMusic.Pause();
    }

    //设置背景音乐大小
    public void ChangeBKMusicValue(float value)
    {
        bkMusicValue = value;
        if (bkMusic == null) return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音效路径</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callback">加载结束后的回调</param>
    public void PlaySound(string path,bool isLoop = false,bool isSync = false,UnityAction<AudioSource> callback = null)
    {
        //从缓存池中取出音效对象得到对应组件
        AudioSource source = PoolMgr.Instance.GetObj("Sounds/Prefabs/soundObj").GetComponent<AudioSource>();

        //根据传入的音效名字 来播放音效
        if (isSync)
        {
            //同步加载音效资源
            AudioClip audioClip = ResourcesMgr.Instance.Load<AudioClip>(path);

            //因为存在缓存池上限 取出来的可能是正在用的组件 所以我们取出来先stop
            source.Stop();

            source.clip = audioClip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //存入容器记录 方便之后判断是否停止
            //由于从缓存池中取出对象 有可能取出正在使用的对象（超上限了）
            //不需要重复去添加即可
            if(!soundList.Contains(source))
                    soundList.Add(source);
        }
        else
        {
            //异步加载音效资源
            ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (AudioClip) =>
            {
                source.clip = AudioClip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                //存入容器记录 方便之后判断是否停止
                soundList.Add(source);
                //传递给外部使用
                callback?.Invoke(source);
            });
        }
    }

    /// <summary>
    /// 停止播放指定音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            //停止播放
            source.Stop();
            //从List中移除
            soundList.Remove(source);

            //音效播放完毕 将切片制空
            source.clip = null;
            //将其放入缓存池中
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoudnValue(float value)
    {
        soundValue = value;
        foreach (AudioSource source in soundList)
        {
            source.volume = value;
        }
    }

    /// <summary>
    /// 播放或者暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否是继续播放 true为播放 false为暂停</param>
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
    /// 清空音效相关记录 过场景时 !!!在清空缓存池之前去调用!!!
    /// </summary>
    public void ClearSound()
    {
        for(int i = 0;i<soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //清空音效列表
        soundList.Clear();
    }
}
