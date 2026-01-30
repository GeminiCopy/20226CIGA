using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Resources 资源加载模块管理器
/// </summary>
public class ResourcesMgr : BaseManager<ResourcesMgr>
{
    private ResourcesMgr() { }

    /// <summary>
    /// 同步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : Object
    {
        //通过协同程序异步加载资源
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path, callBack));
    }

    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callBack) where T : Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        //等待资源加载结束后 才会继续执行yield return后面的代码
        yield return rq;
        //资源加载结束 将委托传到外部的委托函数去进行使用
        callBack(rq.asset as T);
    }

    /// <summary>
    /// 指定卸载一个资源的方法
    /// </summary>
    /// <param name="assetToUnload"></param>
    public void UnLoadAsset(Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }

    /// <summary>
    /// 异步卸载对应没有使用的Resources相关的资源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callback));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callback)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后通知外部
        callback();
    }
}
