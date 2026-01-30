using UnityEngine;

public class WorldViewportController : MonoBehaviour
{
    [Header("场景A设置（主摄像机）")]
    public Camera sceneAMainCamera;
    public Vector3 sceneAPosition = Vector3.zero;
    public float sceneASize = 10f;
    
    [Header("场景B设置（小视口）")]
    public Camera sceneBCamera;
    public Vector3 sceneBOffset = new Vector3(500f, 0f, 0f);
    public float sceneBSize = 10f;
    
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
    
    private bool isDragging = false;
    private Vector2 lastMousePosition;
    private Vector2 dragStart;
    private Camera camA;
    private Camera camB;
    
    // 摄像机B的四个边界点（左上、右上、左下、右下）
    private Vector3[] sceneBBounds = new Vector3[4];
    private bool wasPlayerInBounds;
    
    // 回调委托
    public System.Action onPlayerInBounds;
    public System.Action onPlayerOutOfBounds;
    
    void Start()
    {
        SetupCameras();
        UpdateSceneBCamera();
    }
    
    void SetupCameras()
    {
        if (sceneAMainCamera == null)
        {
            sceneAMainCamera = Camera.main;
        }
        
        camA = sceneAMainCamera;
        
        camA.transform.position = new Vector3(sceneAPosition.x, sceneAPosition.y, -10f);
        camA.orthographicSize = sceneASize;
        camA.backgroundColor = Color.white;
        
        if (sceneBCamera == null)
        {
            GameObject camBObj = new GameObject("SceneBCamera");
            sceneBCamera = camBObj.AddComponent<Camera>();
            camBObj.transform.SetParent(transform);
        }
        
        camB = sceneBCamera;
        
        camB.transform.position = new Vector3(
            sceneAPosition.x + sceneBOffset.x,
            sceneAPosition.y + sceneBOffset.y,
            -10f
        );
        camB.orthographicSize = sceneBSize;
        camB.depth = camA.depth + 1;
        camB.clearFlags = CameraClearFlags.SolidColor;
        camB.backgroundColor = Color.grey;
    }
    
    void Update()
    {
        HandleInput();
        UpdateSceneBCamera();
        CheckPlayerInBounds();
    }
    
    void HandleInput()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 viewportMousePos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        
        bool isInViewport = viewportMousePos.x >= viewportPosition.x && 
                           viewportMousePos.x <= viewportPosition.x + viewportSize &&
                           viewportMousePos.y >= viewportPosition.y && 
                           viewportMousePos.y <= viewportPosition.y + viewportSize;
        
        if (Input.GetMouseButtonDown(0) && isInViewport)
        {
            isDragging = true;
            dragStart = viewportPosition;
            lastMousePosition = viewportMousePos;
        }
        
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = viewportMousePos - lastMousePosition;
            viewportPosition = dragStart + delta;
            
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0, 1 - viewportSize);
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0, 1 - viewportSize);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //if (scroll != 0)
        //{
        //    // 计算当前视口中心（归一化坐标）
        //    Vector2 center = viewportPosition + new Vector2(viewportSize, viewportSize) * 0.5f;
            
        //    // 计算新大小
        //    float newSize = viewportSize - scroll * scrollSpeed;
        //    newSize = Mathf.Clamp(newSize, minViewportSize, maxViewportSize);
            
        //    // 调整位置以保持中心点不变（中心缩放）
        //    viewportPosition = center - new Vector2(newSize, newSize) * 0.5f;
            
        //    // 边界限制
        //    viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0, 1 - newSize);
        //    viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0, 1 - newSize);
            
        //    viewportSize = newSize;
        //}
        
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    ResetViewport();
        //}
    }
    
    void UpdateSceneBCamera()
    {
        camB.rect = new Rect(viewportPosition.x, viewportPosition.y, viewportSize, viewportSize);
        
        float camAPosX = camA.transform.position.x;
        float camAPosY = camA.transform.position.y;
        float camAHeight = camA.orthographicSize;
        float camAWidth = camA.orthographicSize * camA.aspect;
        
        Vector2 viewportCenter = viewportPosition + new Vector2(viewportSize, viewportSize) * 0.5f;
        
        float offsetX = Mathf.Lerp(-camAWidth, camAWidth, viewportCenter.x);
        float offsetY = Mathf.Lerp(-camAHeight, camAHeight, viewportCenter.y);
        
        camB.transform.position = new Vector3(
            sceneAPosition.x + sceneBOffset.x + offsetX,
            sceneAPosition.y + sceneBOffset.y + offsetY,
            -10f
        );
        
        camB.orthographicSize = sceneBSize * viewportSize;
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
        float camBHeight = camB.orthographicSize;
        float camBWidth = camB.orthographicSize * camB.aspect;
        Vector3 camBPos = camB.transform.position;
        
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
    
    //void OnDrawGizmos()
    //{
    //    if (!debugDrawBounds || camB == null) return;
        
    //    Gizmos.color = Color.yellow;
    //    CalculateSceneBBounds();
        
    //    Vector3 topLeft = sceneBBounds[0];
    //    Vector3 topRight = sceneBBounds[1];
    //    Vector3 bottomLeft = sceneBBounds[2];
    //    Vector3 bottomRight = sceneBBounds[3];
        
    //    Gizmos.DrawLine(topLeft, topRight);
    //    Gizmos.DrawLine(topRight, bottomRight);
    //    Gizmos.DrawLine(bottomRight, bottomLeft);
    //    Gizmos.DrawLine(bottomLeft, topLeft);
        
    //    if (playerCollider != null)
    //    {
    //        Gizmos.color = wasPlayerInBounds ? Color.green : Color.red;
    //        Gizmos.DrawWireCube(playerCollider.bounds.center, playerCollider.bounds.size);
    //    }
    //}
    
    //void ResetViewport()
    //{
    //    viewportPosition = new Vector2(0.1f, 0.1f);
    //    viewportSize = 0.3f;
    //    UpdateSceneBCamera();
    //}
    
    //void OnGUI()
    //{
    //    DrawViewportBorder();
    //    DrawSceneABoundaries();
    //    DrawInfo();
    //}
    
    //void DrawViewportBorder()
    //{
    //    Rect screenRect = new Rect(
    //        viewportPosition.x * Screen.width,
    //        (1 - viewportPosition.y - viewportSize) * Screen.height,
    //        viewportSize * Screen.width,
    //        viewportSize * Screen.height
    //    );
        
    //    GUI.color = borderColor;
    //    DrawBorder(screenRect, borderThickness);
    //    GUI.color = Color.white;
    //}
    
    //void DrawSceneABoundaries()
    //{
    //    if (camA != null)
    //    {
    //        GUI.color = Color.green;
            
    //        Vector3 center = camA.WorldToScreenPoint(camA.transform.position);
    //        float height = 2f * sceneASize * Screen.height / (2f * camA.orthographicSize);
    //        float width = height * camA.aspect;
            
    //        Rect sceneARect = new Rect(center.x - width / 2, Screen.height - center.y - height / 2, width, height);
    //        DrawBorder(sceneARect, 2f);
            
    //        GUI.Label(new Rect(center.x - width / 2 + 5, Screen.height - center.y - height / 2 + 5, 100, 20), "场景A", GUI.skin.label);
            
    //        GUI.color = Color.white;
    //    }
    //}
    
    //void DrawBorder(Rect rect, float thickness)
    //{
    //    GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
    //    GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), Texture2D.whiteTexture);
    //    GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
    //    GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
    //}
    
    //void DrawInfo()
    //{
    //    string info = $"视口大小: {viewportSize:F2} | 位置: ({viewportPosition.x:F2}, {viewportPosition.y:F2})";
    //    GUI.Label(new Rect(10, 10, 400, 20), info);
        
    //    string cameraInfo = $"B场景摄像机: ({camB.transform.position.x:F1}, {camB.transform.position.y:F1})";
    //    GUI.Label(new Rect(10, 30, 400, 20), cameraInfo);
        
    //    string offsetInfo = $"场景偏移: ({sceneBOffset.x:F0}, {sceneBOffset.y:F0})";
    //    GUI.Label(new Rect(10, 50, 300, 20), offsetInfo);
        
    //    GUI.Label(new Rect(10, 70, 300, 20), "拖拽移动B场景视口 | 滚轮缩放 | R重置视口");
    //}
    
    public void SetSceneBOffset(Vector3 offset)
    {
        sceneBOffset = offset;
        camB.transform.position = new Vector3(
            sceneAPosition.x + sceneBOffset.x,
            sceneAPosition.y + sceneBOffset.y,
            -10f
        );
    }
    
    public void SetViewport(Vector2 position, float size)
    {
        viewportPosition = Vector2.Max(Vector2.zero, Vector2.Min(position, new Vector2(1 - size, 1 - size)));
        viewportSize = Mathf.Clamp(size, minViewportSize, maxViewportSize);
        UpdateSceneBCamera();
    }
    
    public void SetPlayerCollider(Collider2D collider)
    {
        playerCollider = collider;
    }
    
    // 获取摄像机B的边界点
    public Vector3[] GetSceneBBounds()
    {
        CalculateSceneBBounds();
        return sceneBBounds;
    }
    
    // 玩家是否在摄像机B视野内
    public bool IsPlayerInBounds()
    {
        return wasPlayerInBounds;
    }
}
