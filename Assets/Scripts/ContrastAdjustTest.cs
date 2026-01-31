// using UnityEngine;

// /// <summary>
// /// 对比度调整着色器测试脚本
// /// </summary>
// public class ContrastAdjustTest : MonoBehaviour
// {
//     [Header("测试对象")]
//     public GameObject testObject;
//     public ContrastAdjust contrastAdjust;
    
//     [Header("测试设置")]
//     public bool enableKeyboardControl = true;
//     public bool showDebugInfo = true;
    
//     [Header("动画设置")]
//     public bool enableAnimation = false;
//     public float animationSpeed = 1f;
    
//     // 动画变量
//     private float animationTime = 0f;
//     private int currentPreset = 0;
    
//     // 预设数组
//     private string[] presetNames = { "正常", "高对比度", "怀旧", "黑白" };
    
//     void Start()
//     {
//         // 初始化测试对象
//         InitializeTestObject();
        
//         if (showDebugInfo)
//         {
//             Debug.Log("=== 对比度调整着色器测试开始 ===");
//             Debug.Log("按键控制说明:");
//             Debug.Log("1-4: 切换预设");
//             Debug.Log("C: 调整对比度");
//             Debug.Log("B: 调整亮度"); 
//             Debug.Log("S: 调整饱和度");
//             Debug.Log("H: 切换色相偏移");
//             Debug.Log("G: 切换Gamma校正");
//             Debug.Log("A: 动画开/关");
//             Debug.Log("R: 重置到默认");
//         }
//     }
    
//     void InitializeTestObject()
//     {
//         if (testObject == null)
//         {
//             // 创建一个测试立方体
//             testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
//             testObject.name = "ContrastTestCube";
//             testObject.transform.position = Vector3.zero;
//             testObject.transform.localScale = Vector3.one * 2;
//         }
        
//         if (contrastAdjust == null)
//         {
//             contrastAdjust = testObject.GetComponent<ContrastAdjust>();
//             if (contrastAdjust == null)
//             {
//                 contrastAdjust = testObject.AddComponent<ContrastAdjust>();
//             }
//         }
        
//         // 创建测试纹理
//         CreateTestTexture();
//     }
    
//     void CreateTestTexture()
//     {
//         // 创建一个测试纹理
//         Texture2D testTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        
//         for (int y = 0; y < 256; y++)
//         {
//             for (int x = 0; x < 256; x++)
//             {
//                 // 创建彩虹渐变
//                 float u = (float)x / 256f;
//                 float v = (float)y / 256f;
                
//                 Color color = Color.HSVToRGB(u, 1f, v);
//                 testTexture.SetPixel(x, y, color);
//             }
//         }
        
//         testTexture.Apply();
//         contrastAdjust.SetMainTexture(testTexture);
        
//         if (showDebugInfo)
//         {
//             Debug.Log("✅ 测试纹理创建完成");
//         }
//     }
    
//     void Update()
//     {
//         if (enableKeyboardControl)
//         {
//             HandleKeyboardInput();
//         }
        
//         if (enableAnimation && contrastAdjust != null)
//         {
//             HandleAnimation();
//         }
//     }
    
//     void HandleKeyboardInput()
//     {
//         // 预设切换
//         if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyPreset(0);
//         if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyPreset(1);
//         if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyPreset(2);
//         if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyPreset(3);
        
//         // 基础调整
//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             float current = contrastAdjust.Contrast;
//             current += Input.GetKey(KeyCode.LeftShift) ? -0.1f : 0.1f;
//             contrastAdjust.SetContrast(current);
//             if (showDebugInfo)
//                 Debug.Log($"对比度调整为: {current:F2}");
//         }
        
//         if (Input.GetKeyDown(KeyCode.B))
//         {
//             float current = contrastAdjust.Brightness;
//             current += Input.GetKey(KeyCode.LeftShift) ? -0.1f : 0.1f;
//             contrastAdjust.SetBrightness(current);
//             if (showDebugInfo)
//                 Debug.Log($"亮度调整为: {current:F2}");
//         }
        
//         if (Input.GetKeyDown(KeyCode.S))
//         {
//             float current = contrastAdjust.Saturation;
//             current += Input.GetKey(KeyCode.LeftShift) ? -0.1f : 0.1f;
//             contrastAdjust.SetSaturation(current);
//             if (showDebugInfo)
//                 Debug.Log($"饱和度调整为: {current:F2}");
//         }
        
//         // 高级设置
//         if (Input.GetKeyDown(KeyCode.H))
//         {
//             contrastAdjust.SetUseHueShift(!contrastAdjust.UseHueShift);
//             if (showDebugInfo)
//                 Debug.Log($"色相偏移: {(contrastAdjust.UseHueShift ? "开启" : "关闭")}");
//         }
        
//         if (Input.GetKeyDown(KeyCode.G))
//         {
//             contrastAdjust.SetUseGamma(!contrastAdjust.UseGamma);
//             if (showDebugInfo)
//                 Debug.Log($"Gamma校正: {(contrastAdjust.UseGamma ? "开启" : "关闭")}");
//         }
        
//         if (Input.GetKeyDown(KeyCode.A))
//         {
//             enableAnimation = !enableAnimation;
//             animationTime = 0f;
//             if (showDebugInfo)
//                 Debug.Log($"动画模式: {(enableAnimation ? "开启" : "关闭")}");
//         }
        
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             contrastAdjust.ResetToDefault();
//             if (showDebugInfo)
//                 Debug.Log("重置到默认值");
//         }
//     }
    
//     void HandleAnimation()
//     {
//         animationTime += Time.deltaTime * animationSpeed;
        
//         // 动态变化对比度
//         float animatedContrast = Mathf.Sin(animationTime) * 1.0f;
//         contrastAdjust.SetContrast(animatedContrast);
        
//         // 动态变化亮度
//         float animatedBrightness = Mathf.Cos(animationTime * 0.5f) * 0.5f;
//         contrastAdjust.SetBrightness(animatedBrightness);
        
//         // 动态变化饱和度
//         float animatedSaturation = Mathf.Sin(animationTime * 0.7f) * 0.8f;
//         contrastAdjust.SetSaturation(animatedSaturation);
//     }
    
//     void ApplyPreset(int presetIndex)
//     {
//         currentPreset = presetIndex;
        
//         switch (presetIndex)
//         {
//             case 0:
//                 contrastAdjust.ApplyNormalPreset();
//                 break;
//             case 1:
//                 contrastAdjust.ApplyHighContrastPreset();
//                 break;
//             case 2:
//                 contrastAdjust.ApplyVintagePreset();
//                 break;
//             case 3:
//                 contrastAdjust.ApplyBlackWhitePreset();
//                 break;
//         }
        
//         if (showDebugInfo)
//         {
//             Debug.Log($"切换到预设: {presetNames[presetIndex]}");
//         }
//     }
    
//     void OnGUI()
//     {
//         if (contrastAdjust == null) return;
        
//         GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        
//         GUILayout.Label("🎨 对比度调整测试", EditorStyles.boldLabel);
//         GUILayout.Space(10);
        
//         // 当前状态
//         GUILayout.Label($"当前预设: {presetNames[currentPreset]}");
//         GUILayout.Label($"对比度: {contrastAdjust.Contrast:F2}");
//         GUILayout.Label($"亮度: {contrastAdjust.Brightness:F2}");
//         GUILayout.Label($"饱和度: {contrastAdjust.Saturation:F2}");
//         GUILayout.Label($"色相偏移: {(contrastAdjust.UseHueShift ? "开启" : "关闭")}");
//         GUILayout.Label($"Gamma校正: {(contrastAdjust.UseGamma ? "开启" : "关闭")}");
        
//         GUILayout.Space(10);
        
//         // 预设按钮
//         GUILayout.Label("预设:", EditorStyles.boldLabel);
//         GUILayout.BeginHorizontal();
//         if (GUILayout.Button("正常", GUILayout.Height(25))) ApplyPreset(0);
//         if (GUILayout.Button("高对比度", GUILayout.Height(25))) ApplyPreset(1);
//         if (GUILayout.Button("怀旧", GUILayout.Height(25))) ApplyPreset(2);
//         if (GUILayout.Button("黑白", GUILayout.Height(25))) ApplyPreset(3);
//         GUILayout.EndHorizontal();
        
//         GUILayout.Space(10);
        
//         // 滑块控制
//         GUILayout.Label("手动调整:", EditorStyles.boldLabel);
        
//         float newContrast = GUILayout.HorizontalSlider(contrastAdjust.Contrast, -1f, 3f);
//         if (Mathf.Abs(newContrast - contrastAdjust.Contrast) > 0.001f)
//             contrastAdjust.SetContrast(newContrast);
        
//         float newBrightness = GUILayout.HorizontalSlider(contrastAdjust.Brightness, -1f, 1f);
//         if (Mathf.Abs(newBrightness - contrastAdjust.Brightness) > 0.001f)
//             contrastAdjust.SetBrightness(newBrightness);
        
//         float newSaturation = GUILayout.HorizontalSlider(contrastAdjust.Saturation, -1f, 2f);
//         if (Mathf.Abs(newSaturation - contrastAdjust.Saturation) > 0.001f)
//             contrastAdjust.SetSaturation(newSaturation);
        
//         GUILayout.Space(10);
        
//         // 开关控制
//         GUILayout.Label("高级设置:", EditorStyles.boldLabel);
        
//         bool newHueShift = GUILayout.Toggle(contrastAdjust.UseHueShift, "使用色相偏移");
//         if (newHueShift != contrastAdjust.UseHueShift)
//             contrastAdjust.SetUseHueShift(newHueShift);
        
//         bool newGamma = GUILayout.Toggle(contrastAdjust.UseGamma, "使用Gamma校正");
//         if (newGamma != contrastAdjust.UseGamma)
//             contrastAdjust.SetUseGamma(newGamma);
        
//         bool newAnimation = GUILayout.Toggle(enableAnimation, "动画模式");
//         if (newAnimation != enableAnimation)
//         {
//             enableAnimation = newAnimation;
//             animationTime = 0f;
//         }
        
//         GUILayout.Space(10);
        
//         // 重置按钮
//         if (GUILayout.Button("重置到默认", GUILayout.Height(30)))
//         {
//             contrastAdjust.ResetToDefault();
//             currentPreset = 0;
//         }
        
//         GUILayout.EndArea();
//     }
// }