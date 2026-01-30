using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动挂载式的继承Mono的单例模式基类(推荐)
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //动态创建 动态挂载
                //在场景上创建空物体
                GameObject obj = new GameObject();
                //得到T脚本的类名 为对象改名
                obj.name = typeof(T).ToString();
                //动态挂载对应的单例模式脚本
                instance = obj.AddComponent<T>();
                //保证该对象一直存在
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
