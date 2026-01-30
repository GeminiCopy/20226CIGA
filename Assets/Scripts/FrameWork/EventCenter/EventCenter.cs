using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于装载不同传入类型事件信息的父类
/// </summary>
public abstract class EventInfoBase { }
/// <summary>
/// 用于包裹对应观察者 委托函数 的类
/// </summary>
/// <typeparam name="T">传入参数类型</typeparam>
public class EventInfo<T> : EventInfoBase
{
    public UnityAction<T> actions;
}
/// <summary>
/// 用于包裹无参无返回值 委托函数 的类
/// </summary>
public class EventInfo : EventInfoBase
{
    public UnityAction actions;
}

public class EventCenter : BaseManager<EventCenter>
{
    //用于记录对应事件名对应的事件
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    private EventCenter() { }


    /// <summary>
    /// 事件触发方法
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="obj">传入的参数</param>
    public void EventTrigger<T>(E_EventType eventName,T obj)
    {
        (eventDic[eventName] as EventInfo<T>).actions?.Invoke(obj);
    }

    /// <summary>
    /// 事件触发方法 无参
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void EventTrigger(E_EventType eventName)
    {
        (eventDic[eventName] as EventInfo).actions?.Invoke();
    }

    /// <summary>
    /// 事件添加监听者的方法
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">函数</param>
    public void AddEventListener<T>(E_EventType eventName,UnityAction<T> func)
    {
        //若不存在该字典 创建
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, new EventInfo<T>());
        }
        //为事件添加函数
        (eventDic[eventName] as EventInfo<T>).actions += func;
    }

    /// <summary>
    /// 事件添加监听者的方法
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">函数</param>
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        //若不存在该字典 创建
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, new EventInfo());
        }
    //为事件添加函数
    (eventDic[eventName] as EventInfo).actions += func;
    }

    /// <summary>
    /// 事件删除监听者的方法
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">函数</param>
    public void RemoveEventListener<T>(E_EventType eventName,UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions -= func;
        }
    }

    /// <summary>
    /// 事件删除监听者的方法
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="func">函数</param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions -= func;
        }
    }

    /// <summary>
    /// 删除所有事件监听者的所有方法
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 删除指定事件的所有监听
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic.Remove(eventName);
        }
    }
}
