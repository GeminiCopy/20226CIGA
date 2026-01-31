using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    [Header("副摄像机设置")]
    public Camera subCamera;
    public Vector3 sceneBOffset = new Vector3(500f, 0f, 0f);
    
    [Header("视口设置")]
    public Vector2 viewportPosition = new Vector2(0.1f, 0.1f);
    public float viewportSize = 0.3f;
    public Color borderColor = Color.yellow;
    public float borderThickness = 3f;
    
    [Header("交互设置")]
    public float scrollSpeed = 0.1f;
    public float minViewportSize = 0.1f;
    public float maxViewportSize = 0.8f;
    
    [Header("玩家检测")]
    public Collider2D playerCollider;
    public bool debugDrawBounds = true;
    
    [Header("显示设置")]
    public bool showBorder = true;
    public bool showLabel = true;
    public string cameraLabel = "副摄像机";
    
    // 拖拽状态（由CameraManager管理）
    private bool isDragging = false;
    
    // 摄像机边界点（左上、右上、左下、右下）
    private Vector3[] sceneBBounds = new Vector3[4];
    private bool wasPlayerInBounds;
    
    // 独立事件回调
    public System.Action onPlayerInBounds;
    public System.Action onPlayerOutOfBounds;
    
    // 摄像机管理器引用
    private Camera mainCamera;
    
    // 索引标识
    public int cameraIndex = -1;
    
    void Start()
    {
        InitializeCamera();
    }
    
    public void Initialize(Camera mainCam, int index)
    {
        mainCamera = mainCam;
        cameraIndex = index;
        
        InitializeCamera();
    }
    
    void InitializeCamera()
    {
        // 如果预制体中没有Camera组件，则创建一个
        if (subCamera == null)
        {
            // 尝试从预制体结构中查找Camera组件
            Camera[] camerasInPrefab = GetComponentsInChildren<Camera>(true);
            if (camerasInPrefab.Length > 0)
            {
                // 使用预制体中的第一个Camera组件
                subCamera = camerasInPrefab[0];
                Debug.Log($"使用预制体中的Camera组件: {subCamera.gameObject.name}");
            }
            else
            {
                // 如果预制体中没有Camera，创建一个
                GameObject camObj = new GameObject($"SubCamera_{cameraIndex}");
                camObj.transform.SetParent(transform);
                subCamera = camObj.AddComponent<Camera>();
                Debug.Log($"动态创建Camera组件: {subCamera.gameObject.name}");
            }
        }
        else
        {
            // 已经有Camera组件引用
            Debug.Log($"使用已有的Camera组件: {subCamera.gameObject.name}");
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // 设置副摄像机属性
        subCamera.depth = mainCamera.depth + 1;
        subCamera.clearFlags = CameraClearFlags.SolidColor;
        subCamera.backgroundColor = Color.grey;
        subCamera.orthographic = true;
        subCamera.orthographicSize = mainCamera.orthographicSize;
        
        UpdateSubCamera();
    }
    
    void Update()
    {
        UpdateSubCamera();
        CheckPlayerInBounds();
    }
    
    void UpdateSubCamera()
    {
        if (subCamera == null || mainCamera == null) return;
        
        // 设置视口矩形
        subCamera.rect = new Rect(viewportPosition.x, viewportPosition.y, viewportSize, viewportSize);
        
        // 计算主摄像机的视口范围
        float camAPosX = mainCamera.transform.position.x;
        float camAPosY = mainCamera.transform.position.y;
        float camAHeight = mainCamera.orthographicSize;
        float camAWidth = mainCamera.orthographicSize * mainCamera.aspect;
        
        Vector2 viewportCenter = viewportPosition + new Vector2(viewportSize, viewportSize) * 0.5f;
        
        // 根据视口中心位置计算动态偏移（像原始WorldViewportController那样）
        float offsetX = Mathf.Lerp(-camAWidth, camAWidth, viewportCenter.x);
        float offsetY = Mathf.Lerp(-camAHeight, camAHeight, viewportCenter.y);
        
        // 设置副摄像机位置：动态偏移 + sceneB偏移
        Vector3 mainCamPos = mainCamera.transform.position;
        subCamera.transform.position = new Vector3(
            mainCamPos.x + sceneBOffset.x + offsetX,
            mainCamPos.y + sceneBOffset.y + offsetY,
            -10f
        );
        
        // 设置副摄像机视口大小（与主摄像机size保持比例关系）
        subCamera.orthographicSize = mainCamera.orthographicSize * viewportSize;
    }
    
    void CheckPlayerInBounds()
    {
        if (playerCollider == null) return;
        
        CalculateSceneBBounds();
        
        Bounds playerBounds = playerCollider.bounds;
        bool isPlayerInBounds = IsBoundsOverlap(playerBounds);
        
        if (isPlayerInBounds && !wasPlayerInBounds)
        {
            wasPlayerInBounds = true;
            onPlayerInBounds?.Invoke();
        }
        else if (!isPlayerInBounds && wasPlayerInBounds)
        {
            wasPlayerInBounds = false;
            onPlayerOutOfBounds?.Invoke();
        }
    }
    
    void CalculateSceneBBounds()
    {
        if (subCamera == null) return;
        
        float camBHeight = subCamera.orthographicSize;
        float camBWidth = subCamera.orthographicSize * subCamera.aspect;
        Vector3 camBPos = subCamera.transform.position;
        
        sceneBBounds[0] = new Vector3(camBPos.x - camBWidth, camBPos.y + camBHeight, 0); // 左上
        sceneBBounds[1] = new Vector3(camBPos.x + camBWidth, camBPos.y + camBHeight, 0); // 右上
        sceneBBounds[2] = new Vector3(camBPos.x - camBWidth, camBPos.y - camBHeight, 0); // 左下
        sceneBBounds[3] = new Vector3(camBPos.x + camBWidth, camBPos.y - camBHeight, 0); // 右下
    }
    
    bool IsBoundsOverlap(Bounds player)
    {
        Vector3 min = sceneBBounds[0];
        Vector3 max = sceneBBounds[3];
        
        bool overlapX = player.max.x > min.x && player.min.x < max.x;
        bool overlapY = player.max.y > min.y && player.min.y < max.y;
        
        return overlapX && overlapY;
    }
    
    // 公共方法
    
    public void SetViewport(Vector2 position, float size)
    {
        viewportPosition = Vector2.Max(Vector2.zero, Vector2.Min(position, new Vector2(1 - size, 1 - size)));
        viewportSize = Mathf.Clamp(size, minViewportSize, maxViewportSize);
        UpdateSubCamera();
    }
    
    public void SetPlayerCollider(Collider2D collider)
    {
        playerCollider = collider;
    }
    
    public void SetSceneBOffset(Vector3 offset)
    {
        sceneBOffset = offset;
        UpdateSubCamera();
    }
    
    public void SetBorderVisible(bool visible)
    {
        showBorder = visible;
    }
    
    public void SetLabelVisible(bool visible)
    {
        showLabel = visible;
    }
    
    public void SetCameraLabel(string label)
    {
        cameraLabel = label;
    }
    
    /// <summary>
    /// 设置视口位置和大小
    /// </summary>
    public void SetViewportPosition(Vector2 position, float size)
    {
        viewportPosition = Vector2.Max(Vector2.zero, Vector2.Min(position, new Vector2(1 - size, 1 - size)));
        viewportSize = Mathf.Clamp(size, minViewportSize, maxViewportSize);
        UpdateSubCamera();
    }
    
    /// <summary>
    /// 获取当前视口位置
    /// </summary>
    public Vector2 GetViewportPosition()
    {
        return viewportPosition;
    }

    /// <summary>
    /// 只设置视口位置，保持大小不变
    /// </summary>
    public void SetViewportPositionOnly(Vector2 position)
    {
        viewportPosition = Vector2.Max(Vector2.zero, Vector2.Min(position, new Vector2(1 - viewportSize, 1 - viewportSize)));
        UpdateSubCamera();
    }
    
    /// <summary>
    /// 开始拖拽（由CameraManager调用）
    /// </summary>
    public void BeginDrag(Vector2 mouseViewportPos)
    {
        isDragging = true;
        Debug.Log($"开始拖拽: {cameraLabel}");
    }
    
    /// <summary>
    /// 拖拽中（由CameraManager调用）
    /// </summary>
    public void Drag(Vector2 newViewportPos)
    {
        if (isDragging)
        {
            // 应用边界限制
            newViewportPos.x = Mathf.Clamp(newViewportPos.x, 0, 1 - viewportSize);
            newViewportPos.y = Mathf.Clamp(newViewportPos.y, 0, 1 - viewportSize);
            
            // 设置新的视口位置
            SetViewportPosition(newViewportPos, viewportSize);
        }
    }
    
    /// <summary>
    /// 结束拖拽（由CameraManager调用）
    /// </summary>
    public void EndDrag()
    {
        if (isDragging)
        {
            isDragging = false;
            Debug.Log($"结束拖拽: {cameraLabel}");
        }
    }
    
    /// <summary>
    /// 强制结束拖拽状态（兼容旧接口）
    /// </summary>
    public void ForceEndDragging()
    {
        if (isDragging)
        {
            isDragging = false;
            Debug.Log($"强制结束拖拽: {cameraLabel}");
        }
    }
    
    /// <summary>
    /// 获取当前拖拽状态
    /// </summary>
    public bool IsDragging
    {
        get { return isDragging; }
    }
    
    // 获取摄像机边界点
    public Vector3[] GetSceneBBounds()
    {
        CalculateSceneBBounds();
        return sceneBBounds;
    }
    
    // 玩家是否在摄像机视野内
    public bool IsPlayerInBounds()
    {
        return wasPlayerInBounds;
    }
    
    // 获取当前视口信息
    public (Vector2 position, float size) GetViewportInfo()
    {
        return (viewportPosition, viewportSize);
    }
}