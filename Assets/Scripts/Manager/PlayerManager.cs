using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : SingletonMono<PlayerManager>
{
    [Header("玩家控制器引用")]
    public PlayerController2D player1;
    public PlayerController2D player2;
    
    [Header("速度设置")]
    public float speedWhenDetected = 5f;    // 检测到玩家时的速度（速度a）
    public float speedWhenNotDetected = 0f; // 未检测到玩家时的速度（速度b）
    
    [Header("跳跃设置")]
    public int jumpCountWhenDetected = 1;    // 检测到玩家时的跳跃次数
    public int jumpCountWhenNotDetected = 0; // 未检测到玩家时的跳跃次数
    
    [Header("调试设置")]
    public bool showDebugInfo = true;
    
    private HashSet<TriggerDetector> detectedTriggers = new HashSet<TriggerDetector>();
    private int previousDetectorCount = -1;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 验证玩家控制器引用
        if (player1 == null || player2 == null)
        {
            Debug.LogError("PlayerManager: 两个玩家控制器引用都不能为空！");
        }
        
        // 查找场景中所有的TriggerDetector
        FindAllTriggerDetectors();
    }
    
    void Start()
    {
        // 初始化玩家控制状态
        UpdatePlayerControls();
        
        if (showDebugInfo)
        {
            Debug.Log("PlayerManager: 初始化完成");
        }
    }
    
    void Update()
    {
        // 持续检查状态变化（确保动态更新的TriggerDetector也能被正确处理）
        CheckTriggerDetectorStates();
    }
    
    /// <summary>
    /// 查找场景中所有的TriggerDetector
    /// </summary>
    private void FindAllTriggerDetectors()
    {
        TriggerDetector[] allDetectors = FindObjectsOfType<TriggerDetector>();
        
        foreach (var detector in allDetectors)
        {
            RegisterTriggerDetector(detector);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"PlayerManager: 找到 {allDetectors.Length} 个TriggerDetector");
        }
    }
    
    /// <summary>
    /// 注册TriggerDetector到管理器
    /// </summary>
    private void RegisterTriggerDetector(TriggerDetector detector)
    {
        if (detector == null) return;
        
        // 这里我们通过反射或事件来监听状态变化
        // 由于TriggerDetector的SetPlayerDetected是私有方法，我们需要其他方式
        // 我们可以定期检查TriggerDetector的状态
        
        if (showDebugInfo)
        {
            Debug.Log($"PlayerManager: 注册TriggerDetector: {detector.gameObject.name}");
        }
    }
    
    /// <summary>
    /// 检查所有TriggerDetector的状态
    /// </summary>
    private void CheckTriggerDetectorStates()
    {
        TriggerDetector[] allDetectors = FindObjectsOfType<TriggerDetector>();
        HashSet<TriggerDetector> currentDetected = new HashSet<TriggerDetector>();
        
        // 收集当前被触发的检测器
        foreach (var detector in allDetectors)
        {
            if (detector != null && detector.IsPlayerDetected)
            {
                currentDetected.Add(detector);
            }
        }
        
        // 检查是否有状态变化
        if (!SetsEqual(detectedTriggers, currentDetected))
        {
            detectedTriggers = currentDetected;
            UpdatePlayerControls();
            
            if (showDebugInfo)
            {
                Debug.Log($"PlayerManager: 检测器状态变化，当前数量: {detectedTriggers.Count}");
            }
        }
    }
    
    /// <summary>
    /// 比较两个HashSet是否相等
    /// </summary>
    private bool SetsEqual<T>(HashSet<T> set1, HashSet<T> set2)
    {
        if (set1.Count != set2.Count) return false;
        
        foreach (var item in set1)
        {
            if (!set2.Contains(item)) return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 更新玩家控制器状态
    /// </summary>
    private void UpdatePlayerControls()
    {
        int currentCount = detectedTriggers.Count;
        
        // 只有当状态真正发生变化时才更新
        if (currentCount == previousDetectorCount) return;
        
        float targetSpeed = currentCount > 0 ? speedWhenDetected : speedWhenNotDetected;
        int targetJumpCount = currentCount > 0 ? jumpCountWhenDetected : jumpCountWhenNotDetected;
        
        // 更新玩家1
        if (player1 != null)
        {
            player1.SetMoveSpeed(targetSpeed);
            player1.SetJumpCount(targetJumpCount);
            
            if (showDebugInfo)
            {
                Debug.Log($"PlayerManager: 玩家1 - 速度: {targetSpeed}, 跳跃次数: {targetJumpCount}");
            }
        }
        
        // 更新玩家2
        if (player2 != null)
        {
            player2.SetMoveSpeed(targetSpeed);
            player2.SetJumpCount(targetJumpCount);
            
            if (showDebugInfo)
            {
                Debug.Log($"PlayerManager: 玩家2 - 速度: {targetSpeed}, 跳跃次数: {targetJumpCount}");
            }
        }
        
        // 记录当前状态
        previousDetectorCount = currentCount;
        
        // 状态变化日志
        if (currentCount > 0)
        {
            Debug.Log($"PlayerManager: 检测到 {currentCount} 个TriggerDetector，启用玩家控制");
        }
        else
        {
            Debug.Log("PlayerManager: 没有检测到任何TriggerDetector，禁用玩家控制");
        }
    }
    
    /// <summary>
    /// 获取当前检测到的TriggerDetector数量
    /// </summary>
    public int GetDetectedTriggerCount()
    {
        return detectedTriggers.Count;
    }
    
    /// <summary>
    /// 获取当前检测到的所有TriggerDetector
    /// </summary>
    public HashSet<TriggerDetector> GetDetectedTriggers()
    {
        return new HashSet<TriggerDetector>(detectedTriggers);
    }
    
    /// <summary>
    /// 手动添加TriggerDetector到检测列表（如果需要）
    /// </summary>
    public void AddTriggerDetector(TriggerDetector detector)
    {
        if (detector != null && !detectedTriggers.Contains(detector))
        {
            detectedTriggers.Add(detector);
            UpdatePlayerControls();
        }
    }
    
    /// <summary>
    /// 手动从检测列表移除TriggerDetector（如果需要）
    /// </summary>
    public void RemoveTriggerDetector(TriggerDetector detector)
    {
        if (detector != null && detectedTriggers.Contains(detector))
        {
            detectedTriggers.Remove(detector);
            UpdatePlayerControls();
        }
    }
}