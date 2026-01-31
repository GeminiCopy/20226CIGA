using UnityEngine;
using System.Collections.Generic;

public class CameraManager : SingletonMono<CameraManager>
{
    [Header("主摄像机设置")]
    private Camera mainCamera;
    
    [Header("初始副摄像机数量")]
    public int initialSubCameraCount = 0;
    public bool initializeOnStart = true;
    
    [Header("副摄像机预制体")]
    public SubCameraController subCameraPrefab;
    
    [Header("默认副摄像机配置")]
    public Vector3 defaultSceneBOffset = new Vector3(500f, 0f, 0f);
    public Vector2 defaultViewportPosition = new Vector2(0.1f, 0.1f);
    public float defaultViewportSize = 0.3f;
    
    [Header("布局设置")]
    public LayoutType layoutType = LayoutType.Grid;
    public int gridColumns = 2;
    public int gridRows = 2;
    public float gridSpacing = 0.05f;
    public Vector2 gridStartPosition = new Vector2(0.7f, 0.7f);
    
    [Header("交互设置")]
    public bool enableDragging = true;
    public bool allowOverlap = false;
    public float minDistanceBetweenCameras = 0.1f;
    
    [Header("调试设置")]
    public bool showDebugInfo = false;
    public bool drawViewportBorders = true;
    
    [Header("射线可视化")]
    public bool drawRaycastVisualization = true;  // 是否绘制射线检测可视化
    
    // 副摄像机列表
    private List<SubCameraController> subCameras = new List<SubCameraController>();
    
    // 当前拖拽的副摄像机
    private SubCameraController currentlyDraggingCamera;
    
    // 是否允许拖拽（用于限制同时只能拖拽一个）
    private bool allowOnlyOneDragging = true;
    
    // 被TriggerDetector检测到的摄像机集合
    private HashSet<SubCameraController> detectedByTriggerCameras = new HashSet<SubCameraController>();
    
    // 当前拖拽的摄像机（集中式管理）
    private SubCameraController currentDraggingCamera;
    
    // 拖拽偏移量
    private Vector2 dragOffset;
    
    // 公共属性：是否只允许一个摄像机拖拽
    public bool AllowOnlyOneDragging
    {
        get { return allowOnlyOneDragging; }
        set { allowOnlyOneDragging = value; }
    }
    
    // 事件统计
    private Dictionary<SubCameraController, int> playerEnterCounts = new Dictionary<SubCameraController, int>();
    private Dictionary<SubCameraController, int> playerExitCounts = new Dictionary<SubCameraController, int>();
    
    // 公共属性
    public bool IsDraggingEnabled => enableDragging;
    public int SubCameraCount => subCameras.Count;
    public IReadOnlyList<SubCameraController> SubCameras => subCameras.AsReadOnly();
    
    // 主摄像机访问器
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
        set => mainCamera = value;
    }
    
    // 初始化方法，需要手动调用
    public void Initialize()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("CameraManager: No main camera found!");
            return;
        }
        
        if (initializeOnStart && initialSubCameraCount > 0)
        {
            InitializeSubCameras(initialSubCameraCount);
        }
    }
    
    void Update()
    {
        UpdateLayoutIfNeeded();
        HandleGlobalInput();
        HandleDraggingInput();
    }

    /// <summary>
    /// 加载新场景后调用
    /// </summary>
    public void Clear()
    {
        subCameras.Clear();
        detectedByTriggerCameras.Clear();
    }
    
    // 拖拽输入处理（新增）
    void HandleDraggingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        
        if (Input.GetMouseButton(0) && currentDraggingCamera != null)
        {
            ContinueDrag();
        }
        
        if (Input.GetMouseButtonUp(0) && currentDraggingCamera != null)
        {
            EndDrag();
        }
    }
    
    // 输入控制
    private bool inputEnabled = true;
    
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
    
    void HandleGlobalInput()
    {
        if (!inputEnabled) return;
        
        // 全局快捷键处理
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddNewSubCamera();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RemoveLastSubCamera();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearAllSubCameras();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 根据当前布局规则重新排列所有子摄像机
            ToggleLayout();
        }
    }
    
    #region 动态管理方法
    
    /// <summary>
    /// 添加新摄像机
    /// </summary>
    /// <param name="customConfig">自定义配置，null则使用默认配置</param>
    /// <returns>创建的副摄像机控制器</returns>
    public SubCameraController AddNewSubCamera(SubCameraConfig customConfig = null)
    {
        SubCameraController subCamera;
        
        // 如果有预制体，使用预制体创建
        if (subCameraPrefab != null)
        {
            // 使用预制体创建实例
            GameObject cameraObj = Instantiate(subCameraPrefab.gameObject);
            cameraObj.name = $"SubCamera_{subCameras.Count}";
            
            // 获取SubCameraController组件
            subCamera = cameraObj.GetComponent<SubCameraController>();
            if (subCamera == null)
            {
                Debug.LogWarning("预制体中没有SubCameraController组件，正在添加...");
                subCamera = cameraObj.AddComponent<SubCameraController>();
            }
            else
            {
                Debug.Log($"使用预制体创建SubCamera: {cameraObj.name}");
                
                // 确保预制体中的Camera组件被正确使用
                Camera[] cameras = cameraObj.GetComponentsInChildren<Camera>(true);
                if (cameras.Length > 0)
                {
                    // 如果预制体中有Camera组件，使用第一个
                    if (subCamera.subCamera == null || subCamera.subCamera != cameras[0])
                    {
                        subCamera.subCamera = cameras[0];
                        Debug.Log($"预制体中设置Camera组件: {cameras[0].gameObject.name}");
                    }
                }
            }
        }
        else
        {
            // 回退到动态创建方式
            GameObject cameraObj = new GameObject($"SubCamera_{subCameras.Count}");
            
            // 添加SubCameraController组件
            subCamera = cameraObj.AddComponent<SubCameraController>();
        }
        
        // 应用配置
        ApplyConfiguration(subCamera, customConfig);
        
        // 设置事件回调
        SetupCameraEvents(subCamera);
        
        // 添加到列表
        subCameras.Add(subCamera);
        
        // 初始化相机
        subCamera.Initialize(MainCamera, subCameras.Count - 1);
        
        // 设置新摄像机初始位置在屏幕上方中心
        SetNewCameraToTopCenter(subCamera);
        
        Debug.Log($"CameraManager: Added sub camera {subCameras.Count - 1} {(subCameraPrefab != null ? "(from prefab)" : "(dynamic)")}");
        return subCamera;
    }

    /// <summary>
    /// 设置新摄像机位置到屏幕上方中心
    /// </summary>
    private void SetNewCameraToTopCenter(SubCameraController camera)
    {
        // 屏幕上方中心的坐标
        float centerX = 0.5f;
        float topY = 0.8f;
        
        // 计算实际位置（考虑摄像机的size）
        float actualX = centerX - (camera.viewportSize * 0.5f);
        float actualY = topY;
        
        // 确保不会超出屏幕边界
        actualX = Mathf.Clamp(actualX, 0f, 1f - camera.viewportSize);
        actualY = Mathf.Clamp(actualY, 0f, 1f - camera.viewportSize);
        
        // 设置位置
        camera.SetViewportPositionOnly(new Vector2(actualX, actualY));
        
        Debug.Log($"新摄像机已定位到屏幕上方中心: ({actualX:F2}, {actualY:F2})");
    }
    
    /// <summary>
    /// 排列所有当前子摄像机
    /// </summary>
    public void ArrangeAllSubCameras()
    {
        if (subCameras.Count == 0)
        {
            Debug.LogWarning("CameraManager: 没有子摄像机可以排列");
            return;
        }
        
        // 重新索引所有相机
        ReindexCameras();
        
        // 应用布局
        ApplyLayout();
        
        Debug.Log($"CameraManager: 已排列 {subCameras.Count} 个子摄像机");
    }
    
    /// <summary>
    /// TriggerDetector状态变化回调
    /// </summary>
    /// <param name="detector">变化的TriggerDetector</param>
    /// <param name="detected">是否检测到玩家</param>
    public void OnTriggerDetectorStateChanged(TriggerDetector detector, bool detected)
    {
        // 找到对应的SubCameraController
        SubCameraController correspondingCamera = null;
        
        foreach (var camera in subCameras)
        {
            TriggerDetector[] detectors = camera.GetComponentsInChildren<TriggerDetector>();
            foreach (var det in detectors)
            {
                if (det == detector)
                {
                    correspondingCamera = camera;
                    break;
                }
            }
            
            if (correspondingCamera != null) break;
        }
        
        if (correspondingCamera != null)
        {
            if (detected)
            {
                detectedByTriggerCameras.Add(correspondingCamera);
                Debug.Log($"CameraManager: 摄像机 {correspondingCamera.cameraLabel} 被TriggerDetector检测到");
                
                // 检查是否有摄像机正在被拖动且对应这个TriggerDetector
                if (currentDraggingCamera == correspondingCamera)
                {
                    // 停止当前拖动
                    EndDrag();
                    Debug.Log($"CameraManager: 摄像机 {correspondingCamera.cameraLabel} 正在被拖动，检测到玩家后停止拖动");
                }
            }
            else
            {
                detectedByTriggerCameras.Remove(correspondingCamera);
                Debug.Log($"CameraManager: 摄像机 {correspondingCamera.cameraLabel} 解除TriggerDetector检测");
            }
        }
    }
    
    /// <summary>
    /// 移除指定索引的副摄像机
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveSubCamera(int index)
    {
        if (index < 0 || index >= subCameras.Count)
        {
            Debug.LogWarning($"CameraManager: Invalid index {index}");
            return false;
        }
        
        SubCameraController cameraToRemove = subCameras[index];
        
        // 清理事件统计
        playerEnterCounts.Remove(cameraToRemove);
        playerExitCounts.Remove(cameraToRemove);
        
        // 如果正在拖拽，停止拖拽
        if (currentlyDraggingCamera == cameraToRemove)
        {
            currentlyDraggingCamera = null;
        }
        
        // 销毁GameObject
        GameObject.Destroy(cameraToRemove.gameObject);
        
        // 从列表中移除
        subCameras.RemoveAt(index);
        
        // 重新索引剩余的相机
        ReindexCameras();
        
        // 重新应用布局
        ApplyLayout();
        
        Debug.Log($"CameraManager: Removed sub camera at index {index}");
        return true;
    }
    
    /// <summary>
    /// 移除最后一个副摄像机
    /// </summary>
    /// <returns>是否成功移除</returns>
    public bool RemoveLastSubCamera()
    {
        if (subCameras.Count == 0)
        {
            Debug.LogWarning("CameraManager: No sub cameras to remove");
            return false;
        }
        
        return RemoveSubCamera(subCameras.Count - 1);
    }
    
    /// <summary>
    /// 清空所有副摄像机
    /// </summary>
    public void ClearAllSubCameras()
    {
        foreach (var camera in subCameras)
        {
            if (camera != null)
            {
                GameObject.Destroy(camera.gameObject);
            }
        }
        
        subCameras.Clear();
        playerEnterCounts.Clear();
        playerExitCounts.Clear();
        currentlyDraggingCamera = null;
        
        Debug.Log("CameraManager: Cleared all sub cameras");
    }
    
    /// <summary>
    /// 初始化指定数量的副摄像机
    /// </summary>
    /// <param name="count">数量</param>
    public void InitializeSubCameras(int count)
    {
        ClearAllSubCameras();
        
        for (int i = 0; i < count; i++)
        {
            AddNewSubCamera();
        }
        
        Debug.Log($"CameraManager: Initialized {count} sub cameras");
    }
    
    #endregion
    
    #region 配置管理
    
    void ApplyConfiguration(SubCameraController camera, SubCameraConfig customConfig)
    {
        SubCameraConfig config = customConfig ?? GetDefaultConfig();
        
        camera.sceneBOffset = config.sceneBOffset;
        camera.viewportPosition = config.viewportPosition;
        camera.viewportSize = config.viewportSize;
        camera.borderColor = config.borderColor;
        camera.borderThickness = config.borderThickness;
        camera.cameraLabel = config.cameraLabel;
        camera.SetBorderVisible(config.showBorder);
        camera.SetLabelVisible(config.showLabel);
    }
    
    SubCameraConfig GetDefaultConfig()
    {
        return new SubCameraConfig
        {
            sceneBOffset = defaultSceneBOffset,
            viewportPosition = defaultViewportPosition,
            viewportSize = defaultViewportSize,
            borderColor = Color.yellow,
            borderThickness = 3f,
            showBorder = true,
            showLabel = true,
            cameraLabel = "副摄像机"
        };
    }
    
    void SetupCameraEvents(SubCameraController camera)
    {
        camera.onPlayerInBounds = () => OnPlayerEnterCamera(camera);
        camera.onPlayerOutOfBounds = () => OnPlayerExitCamera(camera);
    }
    
    void ReindexCameras()
    {
        for (int i = 0; i < subCameras.Count; i++)
        {
            if (subCameras[i] != null)
            {
                subCameras[i].cameraIndex = i;
                subCameras[i].SetCameraLabel($"副摄像机 {i + 1}");
            }
        }
    }
    
    #endregion
    
    #region 布局管理
    
    public enum LayoutType
    {
        Grid,           // 网格布局
        Horizontal,     // 水平布局
        Vertical,       // 垂直布局
        RightVertical,  // 右侧垂直布局
        Free            // 自由布局
    }
    
    void ApplyLayout()
    {
        if (subCameras.Count == 0) return;
        
        switch (layoutType)
        {
            case LayoutType.Grid:
                ApplyGridLayout();
                break;
            case LayoutType.Horizontal:
                ApplyHorizontalLayout();
                break;
            case LayoutType.Vertical:
                ApplyVerticalLayout();
                break;
            case LayoutType.RightVertical:
                ApplyRightVerticalLayout();
                break;
            case LayoutType.Free:
                // 自由布局不自动调整
                break;
        }
    }
    
    void ApplyGridLayout()
    {
        int columns = Mathf.Max(1, gridColumns);
        int rows = Mathf.Max(1, gridRows);
        
        float cellWidth = (1f - gridSpacing * (columns + 1)) / columns;
        float cellHeight = (1f - gridSpacing * (rows + 1)) / rows;
        
        for (int i = 0; i < subCameras.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;
            
            if (row >= rows) break; // 超出网格范围
            
            float x = gridSpacing + col * (cellWidth + gridSpacing);
            float y = gridSpacing + row * (cellHeight + gridSpacing);
            
            // 只设置位置，保持原有大小不变
            subCameras[i].SetViewportPositionOnly(new Vector2(x, y));
        }
    }
    
    void ApplyHorizontalLayout()
    {
        float totalWidth = 1f - gridSpacing * (subCameras.Count + 1);
        float cameraWidth = totalWidth / subCameras.Count;
        
        for (int i = 0; i < subCameras.Count; i++)
        {
            float x = gridSpacing + i * (cameraWidth + gridSpacing);
            float y = gridSpacing;
            
            // 只设置位置，保持原有大小不变
            subCameras[i].SetViewportPositionOnly(new Vector2(x, y));
        }
    }
    
    void ApplyVerticalLayout()
    {
        float totalHeight = 1f - gridSpacing * (subCameras.Count + 1);
        float cameraHeight = totalHeight / subCameras.Count;
        
        for (int i = 0; i < subCameras.Count; i++)
        {
            float x = gridSpacing;
            float y = gridSpacing + i * (cameraHeight + gridSpacing);
            
            // 只设置位置，保持原有大小不变
            subCameras[i].SetViewportPositionOnly(new Vector2(x, y));
        }
    }
    
    void ApplyRightVerticalLayout()
    {
        if (subCameras.Count == 0) return;
        
        // 右侧边距
        float rightMargin = 0.05f; // 右侧边距5%
        float topBottomMargin = 0.05f; // 上下边距
        
        // 计算可用的垂直空间
        float availableHeight = 1f - topBottomMargin * 2f; // 去掉上下边距后的可用高度
        float totalSpacing = gridSpacing * (subCameras.Count + 1); // 总间距
        float averageCameraHeight = (availableHeight - totalSpacing) / subCameras.Count;
        
        for (int i = 0; i < subCameras.Count; i++)
        {
            // 获取当前摄像机的实际大小
            float cameraSize = subCameras[i].viewportSize;
            
            // 计算垂直位置（从上到下排列）
            float y = topBottomMargin + gridSpacing + i * (averageCameraHeight + gridSpacing);
            
            // 计算X位置：让摄像机真正靠到屏幕最右边
            // 使用摄像机的实际size，而不是固定宽度
            float x = 1f - rightMargin - cameraSize;
            
            // 只设置位置，保持原有大小不变
            subCameras[i].SetViewportPositionOnly(new Vector2(x, y));
        }
        
        Debug.Log($"RightVertical布局应用完成：{subCameras.Count}个摄像机已排列到屏幕最右边");
    }
    
    void UpdateLayoutIfNeeded()
    {
        // 如果是自由布局，不需要自动更新
        if (layoutType == LayoutType.Free) return;
        
        // 监控布局参数变化，如果需要重新应用布局
        // 这里可以添加更智能的布局更新逻辑
    }
    
    public void SetLayoutType(LayoutType newLayout)
    {
        layoutType = newLayout;
        ApplyLayout();
    }
    
    public void ToggleLayout()
    {
        // 根据当前布局类型重新排列所有子摄像机
        ArrangeAllSubCameras();
        
        Debug.Log($"CameraManager: 根据当前布局类型 {layoutType} 重新排列了 {subCameras.Count} 个子摄像机");
    }
    
    #endregion
    
    #region 拖拽管理
    
    public void OnSubCameraDragStart(SubCameraController camera)
    {
        if (!enableDragging) return;
        
        // 如果允许同时只能拖拽一个
        if (allowOnlyOneDragging)
        {
            // 如果有其他摄像机正在拖拽，先结束它们
            if (currentlyDraggingCamera != null && currentlyDraggingCamera != camera)
            {
                EndDraggingForCamera(currentlyDraggingCamera);
            }
        }
        
        // 检查当前摄像机是否被TriggerDetector检测
        if (IsCameraDetectedByTrigger(camera))
        {
            Debug.Log($"摄像机 {camera.cameraLabel} 被TriggerDetector检测到，尝试切换到其他可拖动摄像机");
            
            // 尝试找到其他可拖动的摄像机
            SubCameraController alternativeCamera = FindAlternativeDraggableCamera(camera);
            if (alternativeCamera != null)
            {
                Debug.Log($"自动切换到可拖动摄像机: {alternativeCamera.cameraLabel}");
                camera = alternativeCamera;
            }
            else
            {
                Debug.Log("没有找到可替代的摄像机，禁止拖拽");
                return;
            }
        }
        
        currentlyDraggingCamera = camera;
        
        if (!allowOverlap)
        {
            // 暂时隐藏其他相机以避免重叠
            foreach (var cam in subCameras)
            {
                if (cam != camera)
                {
                    cam.SetBorderVisible(false);
                }
            }
        }
    }
    
    /// <summary>
    /// 获取当前拖拽的摄像机
    /// </summary>
    public SubCameraController GetCurrentlyDraggingCamera()
    {
        return currentlyDraggingCamera;
    }
    
    /// <summary>
    /// 检查指定摄像机是否被TriggerDetector检测
    /// </summary>
    public bool IsCameraDetectedByTrigger(SubCameraController camera)
    {
        return detectedByTriggerCameras.Contains(camera);
    }
    
    /// <summary>
    /// 获取所有被TriggerDetector检测的摄像机
    /// </summary>
    public HashSet<SubCameraController> GetDetectedCameras()
    {
        return new HashSet<SubCameraController>(detectedByTriggerCameras);
    }
    
    /// <summary>
    /// 强制结束指定摄像机的拖拽状态
    /// </summary>
    /// <param name="camera">要结束拖拽的摄像机</param>
    private void EndDraggingForCamera(SubCameraController camera)
    {
        if (camera != null)
        {
            // 强制结束拖拽状态
            camera.ForceEndDragging();
            
            if (currentlyDraggingCamera == camera)
            {
                currentlyDraggingCamera = null;
            }
        }
    }
    
    /// <summary>
    /// 查找替代的可拖动摄像机
    /// </summary>
    private SubCameraController FindAlternativeDraggableCamera(SubCameraController excludedCamera)
    {
        // 按索引顺序查找，排除当前摄像机和被检测的摄像机
        for (int i = 0; i < subCameras.Count; i++)
        {
            var camera = subCameras[i];
            if (camera != excludedCamera && !IsCameraDetectedByTrigger(camera))
            {
                return camera;
            }
        }
        
        // 如果没找到，返回null
        return null;
    }
    
    public void OnSubCameraDragEnd(SubCameraController camera)
    {
        if (currentlyDraggingCamera == camera)
        {
            currentlyDraggingCamera = null;
        }
        
        if (!allowOverlap)
        {
            // 恢复其他相机的边框显示
            foreach (var cam in subCameras)
            {
                if (cam != camera)
                {
                    cam.SetBorderVisible(true);
                }
            }
            
            // 检查并调整位置以避免重叠
            AdjustPositionsToAvoidOverlap();
        }
    }
    
    void AdjustPositionsToAvoidOverlap()
    {
        for (int i = 0; i < subCameras.Count; i++)
        {
            for (int j = i + 1; j < subCameras.Count; j++)
            {
                if (IsOverlapping(subCameras[i], subCameras[j]))
                {
                    // 简单的分离逻辑
                    Vector2 pos1 = subCameras[i].GetViewportInfo().position;
                    Vector2 pos2 = subCameras[j].GetViewportInfo().position;
                    Vector2 separation = (pos2 - pos1).normalized * minDistanceBetweenCameras;
                    
                    subCameras[j].SetViewport(pos2 + separation, subCameras[j].GetViewportInfo().size);
                }
            }
        }
    }
    
    bool IsOverlapping(SubCameraController cam1, SubCameraController cam2)
    {
        var (pos1, size1) = cam1.GetViewportInfo();
        var (pos2, size2) = cam2.GetViewportInfo();
        
        Rect rect1 = new Rect(pos1.x, pos1.y, size1, size1);
        Rect rect2 = new Rect(pos2.x, pos2.y, size2, size2);
        
        return rect1.Overlaps(rect2);
    }
    
    #endregion
    
    #region 事件管理
    
    void OnPlayerEnterCamera(SubCameraController camera)
    {
        if (!playerEnterCounts.ContainsKey(camera))
        {
            playerEnterCounts[camera] = 0;
        }
        playerEnterCounts[camera]++;
        
        if (showDebugInfo)
        {
            Debug.Log($"Player entered camera {camera.cameraIndex} (count: {playerEnterCounts[camera]})");
        }
    }
    
    void OnPlayerExitCamera(SubCameraController camera)
    {
        if (!playerExitCounts.ContainsKey(camera))
        {
            playerExitCounts[camera] = 0;
        }
        playerExitCounts[camera]++;
        
        if (showDebugInfo)
        {
            Debug.Log($"Player exited camera {camera.cameraIndex} (count: {playerExitCounts[camera]})");
        }
    }
    
    #endregion
    
    #region 查询方法
    
    public SubCameraController GetSubCamera(int index)
    {
        if (index < 0 || index >= subCameras.Count)
        {
            return null;
        }
        return subCameras[index];
    }
    
    public List<SubCameraController> GetCamerasInBounds(Bounds bounds)
    {
        List<SubCameraController> camerasInBounds = new List<SubCameraController>();
        
        foreach (var camera in subCameras)
        {
            if (camera != null && camera.IsPlayerInBounds())
            {
                camerasInBounds.Add(camera);
            }
        }
        
        return camerasInBounds;
    }
    
    public int GetPlayerEnterCount(SubCameraController camera)
    {
        return playerEnterCounts.ContainsKey(camera) ? playerEnterCounts[camera] : 0;
    }
    
    public int GetPlayerExitCount(SubCameraController camera)
    {
        return playerExitCounts.ContainsKey(camera) ? playerExitCounts[camera] : 0;
    }
    
    #endregion
    
    #region Unity生命周期方法
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("CameraManager: No main camera found!");
            return;
        }
        
        if (initializeOnStart && initialSubCameraCount > 0)
        {
            InitializeSubCameras(initialSubCameraCount);
        }
        
        Debug.Log("CameraManager: 集中式拖拽管理系统已启动");
    }
    
    #endregion
    
    #region 集中式拖拽管理
    
    /// <summary>
    /// 尝试开始拖拽（射线检测）
    /// </summary>
    bool TryStartDrag()
    {
        // 直接从鼠标屏幕位置发射垂直于xy面的射线（平行于z轴）
        if (Camera.main == null) return false;
        
        // 将鼠标屏幕位置转换为世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + defaultSceneBOffset;
        mouseWorldPos.z = 0f; // 确保z坐标为0
        
        // 绘制射线（可视化）
        if (drawRaycastVisualization)
        {
            // 绘制从鼠标位置沿z轴方向的射线
            Vector3 rayStart = mouseWorldPos;
            Vector3 rayEnd = mouseWorldPos + Vector3.forward * 100f;
            Debug.DrawLine(rayStart, rayEnd, Color.cyan, 2f);
        }
        
        // 直接从鼠标位置发射垂直于xy面的射线（平行于z轴）
        Vector2 rayOrigin = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.zero, -Mathf.Infinity);
        
        // 按从前往后顺序检查
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            
            // 检查是否是BoxCollider2D
            if (hit.collider is BoxCollider2D)
            {
                // 查找父级的SubCameraController
                SubCameraController camera = hit.collider.transform.parent.GetComponent<SubCameraController>();
                if (camera != null)
                {
                    Debug.Log(i + "找到了");
                    // 检查TriggerDetector状态
                    if (!IsCameraDetectedByTrigger(camera))
                    {
                        // 找到第一个可拖拽的摄像机
                        Debug.Log($"射线检测: 找到可拖拽摄像机 {camera.cameraLabel}");
                        StartDragging(camera);
                        return true;
                    }
                    else
                    {
                        // 被TriggerDetector检测，跳过
                        Debug.Log($"射线检测: 跳过被检测的摄像机 {camera.cameraLabel}");
                    }
                }
                else
                {
                    Debug.Log(i + "没有找到");
                }
            }
        }
        
        return false; // 没有找到可拖拽的摄像机
    }
    
    /// <summary>
    /// 继续拖拽
    /// </summary>
    void ContinueDrag()
    {
        if (currentDraggingCamera != null)
        {
            Vector2 mouseViewportPos = GetMouseViewportPosition();
            Vector2 newPosition = mouseViewportPos + dragOffset;
            
            // 应用边界限制
            newPosition.x = Mathf.Clamp(newPosition.x, 0, 1 - currentDraggingCamera.viewportSize);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, 1 - currentDraggingCamera.viewportSize);
            
            currentDraggingCamera.Drag(newPosition);
        }
    }
    
    /// <summary>
    /// 开始拖拽指定摄像机
    /// </summary>
    void StartDragging(SubCameraController camera)
    {
        currentDraggingCamera = camera;
        
        // 1. 保持原有的单个拖拽逻辑
        if (allowOnlyOneDragging)
        {
            if (currentlyDraggingCamera != null && currentlyDraggingCamera != camera)
            {
                EndDraggingForCamera(currentlyDraggingCamera);
            }
        }
        
        // 2. 保持原有的重叠处理逻辑
        if (!allowOverlap)
        {
            foreach (var cam in subCameras)
            {
                if (cam != camera)
                {
                    cam.SetBorderVisible(false);
                }
            }
        }
        
        currentlyDraggingCamera = camera;
        
        // 3. 开始拖拽
        Vector2 mouseViewportPos = GetMouseViewportPosition();
        camera.BeginDrag(mouseViewportPos);
        
        // 4. 计算拖拽偏移
        dragOffset = camera.GetViewportPosition() - mouseViewportPos;
        
        Debug.Log($"开始集中式拖拽: {camera.cameraLabel}");
    }
    
    /// <summary>
    /// 结束拖拽
    /// </summary>
    void EndDrag()
    {
        if (currentDraggingCamera != null)
        {
            currentDraggingCamera.EndDrag();
            
            // 恢复其他相机的边框显示
            if (!allowOverlap)
            {
                foreach (var cam in subCameras)
                {
                    if (cam != currentDraggingCamera)
                    {
                        cam.SetBorderVisible(true);
                    }
                }
                
                // 检查并调整位置以避免重叠
                AdjustPositionsToAvoidOverlap();
            }
            
            Debug.Log($"结束集中式拖拽: {currentDraggingCamera.cameraLabel}");
            currentDraggingCamera = null;
        }
    }
    
    /// <summary>
    /// 获取鼠标视口位置
    /// </summary>
    Vector2 GetMouseViewportPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        return new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
    }
    
    #endregion
}