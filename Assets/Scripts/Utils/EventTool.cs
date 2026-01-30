using System;
using System.Collections.Generic;


/// <summary>
/// 对c#原生的Action进行包装接口，继承了此接口才能进入EventTools
/// </summary>
public interface IEventTool
{
    void Register(Action onEvent);
}

/// <summary>
/// 对c#原生的Action进行包装
/// </summary>
public class EventTool : IEventTool
{
    private Action mOnEvent = () => { };
    public void Register(Action onEvent) => mOnEvent += onEvent;
    public void UnRegister(Action onEvent) => mOnEvent -= onEvent;
    public void Trigger() => mOnEvent?.Invoke();
}

/// <summary>
/// 对c#原生的Action进行包装
/// </summary>
/// <typeparam name="T">任意类型，使用struct对gc有优化</typeparam>
public class EventTool<T> : IEventTool
{
    private Action<T> mOnEvent = t => { };
    public void Register(Action<T> onEvent) => mOnEvent += onEvent;
    public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;
    public void Trigger(T t) => mOnEvent?.Invoke(t);

    public void Register(Action onEvent)
    {
        Register(Action);
        return;

        //定义一个T缺省的Action并注册
        void Action(T _) => onEvent();
    }

    public void UnRegisterAll()
    {
        mOnEvent = t => { };
    }
}

/// <summary>
/// 存放所有事件的类，在这里发送事件
/// </summary>
public class TypeEventSystem : Singleton<TypeEventSystem>
{
    private static Dictionary<Type, IEventTool> mEventDict = new();
    static T Get<T>() where T : IEventTool
    {
        //获取值是否成功，成功返回值，失败返回default
        return mEventDict.TryGetValue(typeof(T), out var e) ? (T)e : default;
    }
    private static T Add<T>() where T : IEventTool, new()
    {
        if (mEventDict.TryGetValue(typeof(T), out var e)) return (T)e;

        var t = new T();
        mEventDict.Add(typeof(T), t);
        return t;
    }
    /// <summary>
    /// 在这里使用类/结构体创建事件
    /// </summary>
    /// <typeparam name="T">任意类型，使用struct对gc有优化</typeparam>
    /// <param name="onEvent">要触发的事件</param>
    public void Register<T>(Action<T> onEvent)
    {
        Add<EventTool<T>>().Register(onEvent);
    }
    public void UnRegister<T>(Action<T> onEvent)
    {
        Get<EventTool<T>>().UnRegister(onEvent);
    }
    public void UnRegisterAll<T>()
    {
        Get<EventTool<T>>()?.UnRegisterAll();
        mEventDict.Remove(typeof(T));
    }
    /// <summary>
    /// 带参触发事件
    /// </summary>
    /// <typeparam name="T">绑定了事件的类型</typeparam>
    /// <param name="t">参数</param>
    public void Invoke<T>(T t)
    {
        Get<EventTool<T>>()?.Trigger(t);
    }
    /// <summary>
    /// 无参触发事件，使用new()
    /// </summary>
    /// <typeparam name="T">绑定了事件的类型</typeparam>
    public void Invoke<T>() where T : new()
    {
        Get<EventTool<T>>()?.Trigger(new T());
    }

}