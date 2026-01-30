using System;

/// <summary>
/// 单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T>
{
    private static readonly T instance = Activator.CreateInstance<T>();

    public static T Inst => instance;

    public virtual void Init()
    {
    }

    public virtual void Update(float dt)
    {
    }

    public virtual void OnDestroy()
    {
    }
}