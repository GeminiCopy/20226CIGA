using UnityEngine;

/// <summary>
/// 子摄像机预制体示例
/// 创建一个包含完整功能的子摄像机预制体
/// </summary>
public class SubCameraPrefab : MonoBehaviour
{
    void Start()
    {
        // 如果这个预制体被正确实例化，应该已经包含SubCameraController组件
        SubCameraController controller = GetComponent<SubCameraController>();
        if (controller == null)
        {
            Debug.LogWarning("SubCameraPrefab: 未找到SubCameraController组件，正在添加...");
            controller = gameObject.AddComponent<SubCameraController>();
        }
        
        Debug.Log("SubCameraPrefab: 预制体已初始化");
    }
}