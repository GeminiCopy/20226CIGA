using UnityEngine;

[System.Serializable]
public class SubCameraConfig
{
    [Header("场景设置")]
    public Vector3 sceneBOffset = new Vector3(500f, 0f, 0f);
    
    [Header("视口设置")]
    public Vector2 viewportPosition = new Vector2(0.1f, 0.1f);
    public float viewportSize = 0.3f;
    
    [Header("显示设置")]
    public Color borderColor = Color.yellow;
    public float borderThickness = 3f;
    public bool showBorder = true;
    public bool showLabel = true;
    public string cameraLabel = "副摄像机";
    
    [Header("交互设置")]
    public float scrollSpeed = 0.1f;
    public float minViewportSize = 0.1f;
    public float maxViewportSize = 0.8f;
}