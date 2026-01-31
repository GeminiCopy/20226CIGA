using UnityEngine;

public class CameraModificationTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool testNewCameraPosition = true;
    public bool testLayoutOnlyPosition = true;
    public bool testRightVerticalLayout = true;
    
    private CameraManager cameraManager;
    
    void Start()
    {
        cameraManager = CameraManager.Instance;
        
        if (testNewCameraPosition)
        {
            TestNewCameraPosition();
        }
        
        if (testLayoutOnlyPosition)
        {
            TestLayoutOnlyPosition();
        }
        
        if (testRightVerticalLayout)
        {
            TestRightVerticalLayout();
        }
    }
    
    void TestNewCameraPosition()
    {
        Debug.Log("=== 测试新摄像机定位 ===");
        
        // 添加一个新摄像机
        var newCamera = cameraManager.AddNewSubCamera();
        
        if (newCamera != null)
        {
            var (position, size) = newCamera.GetViewportInfo();
            Debug.Log($"新摄像机位置: ({position.x:F3}, {position.y:F3})");
            Debug.Log($"新摄像机大小: {size:F3}");
            
            // 验证位置是否在屏幕上方中心附近
            bool isAtTopCenter = Mathf.Abs(position.x - 0.5f) < 0.1f && position.y > 0.7f;
            Debug.Log($"是否在屏幕上方中心: {isAtTopCenter}");
        }
    }
    
    void TestLayoutOnlyPosition()
    {
        Debug.Log("=== 测试布局只调整位置 ===");
        
        // 添加多个摄像机
        var camera1 = cameraManager.AddNewSubCamera();
        var camera2 = cameraManager.AddNewSubCamera();
        var camera3 = cameraManager.AddNewSubCamera();
        
        if (camera1 != null && camera2 != null && camera3 != null)
        {
            // 记录初始信息
            float originalSize1 = camera1.GetViewportInfo().size;
            float originalSize2 = camera2.GetViewportInfo().size;
            float originalSize3 = camera3.GetViewportInfo().size;
            Vector2 originalPos1 = camera1.GetViewportPosition();
            Vector2 originalPos2 = camera2.GetViewportPosition();
            Vector2 originalPos3 = camera3.GetViewportPosition();
            
            Debug.Log($"摄像机1原始: 位置({originalPos1.x:F3}, {originalPos1.y:F3}), 大小{originalSize1:F3}");
            Debug.Log($"摄像机2原始: 位置({originalPos2.x:F3}, {originalPos2.y:F3}), 大小{originalSize2:F3}");
            Debug.Log($"摄像机3原始: 位置({originalPos3.x:F3}, {originalPos3.y:F3}), 大小{originalSize3:F3}");
            
            // 应用布局
            cameraManager.ArrangeAllSubCameras();
            
            // 检查信息
            float newSize1 = camera1.GetViewportInfo().size;
            float newSize2 = camera2.GetViewportInfo().size;
            float newSize3 = camera3.GetViewportInfo().size;
            Vector2 newPos1 = camera1.GetViewportPosition();
            Vector2 newPos2 = camera2.GetViewportPosition();
            Vector2 newPos3 = camera3.GetViewportPosition();
            
            Debug.Log($"摄像机1新: 位置({newPos1.x:F3}, {newPos1.y:F3}), 大小{newSize1:F3}");
            Debug.Log($"摄像机2新: 位置({newPos2.x:F3}, {newPos2.y:F3}), 大小{newSize2:F3}");
            Debug.Log($"摄像机3新: 位置({newPos3.x:F3}, {newPos3.y:F3}), 大小{newSize3:F3}");
            
            // 检查大小是否保持不变
            bool sizeUnchanged1 = Mathf.Approximately(originalSize1, newSize1);
            bool sizeUnchanged2 = Mathf.Approximately(originalSize2, newSize2);
            bool sizeUnchanged3 = Mathf.Approximately(originalSize3, newSize3);
            
            Debug.Log($"摄像机大小保持不变: {sizeUnchanged1 && sizeUnchanged2 && sizeUnchanged3}");
            
            if (sizeUnchanged1 && sizeUnchanged2 && sizeUnchanged3)
            {
                Debug.Log("✅ 布局测试通过：只调整位置，大小保持不变");
            }
            else
            {
                Debug.LogError("❌ 布局测试失败：大小被意外修改");
            }
        }
    }
    
    void TestRightVerticalLayout()
    {
        Debug.Log("=== 测试RightVertical布局（屏幕最右边排列）===");
        
        // 添加不同大小的摄像机
        var camera1 = cameraManager.AddNewSubCamera();
        var camera2 = cameraManager.AddNewSubCamera();
        var camera3 = cameraManager.AddNewSubCamera();
        
        if (camera1 != null && camera2 != null && camera3 != null)
        {
            // 手动设置不同的viewportSize来测试
            camera1.SetViewport(new Vector2(0.1f, 0.1f), 0.2f); // 小尺寸
            camera2.SetViewport(new Vector2(0.2f, 0.2f), 0.3f); // 中尺寸  
            camera3.SetViewport(new Vector2(0.3f, 0.3f), 0.4f); // 大尺寸
            
            Debug.Log("设置不同大小的摄像机后，应用RightVertical布局...");
            
            // 切换到RightVertical布局
            cameraManager.SetLayoutType(CameraManager.LayoutType.RightVertical);
            
            // 获取布局后的信息
            Vector2 pos1 = camera1.GetViewportPosition();
            Vector2 pos2 = camera2.GetViewportPosition();
            Vector2 pos3 = camera3.GetViewportPosition();
            float size1 = camera1.GetViewportInfo().size;
            float size2 = camera2.GetViewportInfo().size;
            float size3 = camera3.GetViewportInfo().size;
            
            Debug.Log($"RightVertical布局结果:");
            Debug.Log($"摄像机1: 位置({pos1.x:F3}, {pos1.y:F3}), 大小{size1:F3}");
            Debug.Log($"摄像机2: 位置({pos2.x:F3}, {pos2.y:F3}), 大小{size2:F3}");
            Debug.Log($"摄像机3: 位置({pos3.x:F3}, {pos3.y:F3}), 大小{size3:F3}");
            
            // 检查是否真的靠右排列
            float rightMargin = 0.05f;
            bool isAtRight1 = Mathf.Abs(pos1.x + size1 - (1f - rightMargin)) < 0.01f;
            bool isAtRight2 = Mathf.Abs(pos2.x + size2 - (1f - rightMargin)) < 0.01f;
            bool isAtRight3 = Mathf.Abs(pos3.x + size3 - (1f - rightMargin)) < 0.01f;
            
            Debug.Log($"摄像机1靠右排列: {isAtRight1}");
            Debug.Log($"摄像机2靠右排列: {isAtRight2}");
            Debug.Log($"摄像机3靠右排列: {isAtRight3}");
            
            if (isAtRight1 && isAtRight2 && isAtRight3)
            {
                Debug.Log("✅ RightVertical布局测试通过：所有摄像机都排列在屏幕最右边");
            }
            else
            {
                Debug.LogError("❌ RightVertical布局测试失败：摄像机未排列在屏幕最右边");
            }
        }
    }
    
    void Update()
    {
        // 按T键测试新摄像机定位
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestNewCameraPosition();
        }
        
        // 按L键测试布局（根据当前规则重新排列）
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestLayoutOnlyPosition();
        }
        
        // 按R键测试RightVertical布局
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestRightVerticalLayout();
        }
    }
}