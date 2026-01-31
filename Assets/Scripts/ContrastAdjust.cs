using UnityEngine;

/// <summary>
/// 对比度调整着色器组件
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class ContrastAdjust : MonoBehaviour
{
    [Header("基础调整")]
    [Range(-1f, 3f)]
    [SerializeField] private float contrast = 0f;
    
    [Range(-1f, 1f)]
    [SerializeField] private float brightness = 0f;
    
    [Range(-1f, 2f)]
    [SerializeField] private float saturation = 0f;
    
    [Range(0f, 1f)]
    [SerializeField] private float alpha = 1f;
    
    [SerializeField] private Color tintColor = Color.white;
    
    [Header("高级设置")]
    [SerializeField] private bool useHueShift = false;
    
    [Range(-180f, 180f)]
    [SerializeField] private float hueShift = 0f;
    
    [SerializeField] private bool useGamma = false;
    
    [Range(0.1f, 3f)]
    [SerializeField] private float gamma = 1f;
    
    [Header("纹理")]
    [SerializeField] private Texture2D mainTexture;
    
    private Material material;
    private MeshRenderer meshRenderer;
    
    // 公共属性
    public float Contrast 
    { 
        get => contrast; 
        set => SetContrast(value); 
    }
    
    public float Brightness 
    { 
        get => brightness; 
        set => SetBrightness(value); 
    }
    
    public float Saturation 
    { 
        get => saturation; 
        set => SetSaturation(value); 
    }
    
    public float Alpha 
    { 
        get => alpha; 
        set => SetAlpha(value); 
    }
    
    public Color TintColor 
    { 
        get => tintColor; 
        set => SetTintColor(value); 
    }
    
    public bool UseHueShift 
    { 
        get => useHueShift; 
        set => SetUseHueShift(value); 
    }
    
    public float HueShift 
    { 
        get => hueShift; 
        set => SetHueShift(value); 
    }
    
    public bool UseGamma 
    { 
        get => useGamma; 
        set => SetUseGamma(value); 
    }
    
    public float Gamma 
    { 
        get => gamma; 
        set => SetGamma(value); 
    }
    
    public Texture2D MainTexture 
    { 
        get => mainTexture; 
        set => SetMainTexture(value); 
    }
    
    void Awake()
    {
        InitializeComponent();
    }
    
    void OnEnable()
    {
        InitializeComponent();
        UpdateMaterial();
    }
    
    void OnValidate()
    {
        InitializeComponent();
        UpdateMaterial();
    }
    
    void InitializeComponent()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        if (material == null && meshRenderer != null)
        {
            // 获取或创建材质
            var materials = meshRenderer.sharedMaterials;
            if (materials != null && materials.Length > 0)
            {
                material = materials[0];
            }
        }
    }
    
    void UpdateMaterial()
    {
        if (material == null) return;
        
        // 更新材质属性
        material.SetFloat("_Contrast", contrast);
        material.SetFloat("_Brightness", brightness);
        material.SetFloat("_Saturation", saturation);
        material.SetFloat("_Alpha", alpha);
        material.SetColor("_TintColor", tintColor);
        material.SetFloat("_UseHueShift", useHueShift ? 1f : 0f);
        material.SetFloat("_HueShift", hueShift);
        material.SetFloat("_UseGamma", useGamma ? 1f : 0f);
        material.SetFloat("_Gamma", gamma);
        
        if (mainTexture != null)
        {
            material.SetTexture("_MainTex", mainTexture);
        }
    }
    
    // 公共设置方法
    public void SetContrast(float value)
    {
        contrast = Mathf.Clamp(value, -1f, 3f);
        UpdateMaterial();
    }
    
    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp(value, -1f, 1f);
        UpdateMaterial();
    }
    
    public void SetSaturation(float value)
    {
        saturation = Mathf.Clamp(value, -1f, 2f);
        UpdateMaterial();
    }
    
    public void SetAlpha(float value)
    {
        alpha = Mathf.Clamp01(value);
        UpdateMaterial();
    }
    
    public void SetTintColor(Color value)
    {
        tintColor = value;
        UpdateMaterial();
    }
    
    public void SetUseHueShift(bool value)
    {
        useHueShift = value;
        UpdateMaterial();
    }
    
    public void SetHueShift(float value)
    {
        hueShift = Mathf.Clamp(value, -180f, 180f);
        UpdateMaterial();
    }
    
    public void SetUseGamma(bool value)
    {
        useGamma = value;
        UpdateMaterial();
    }
    
    public void SetGamma(float value)
    {
        gamma = Mathf.Clamp(value, 0.1f, 3f);
        UpdateMaterial();
    }
    
    public void SetMainTexture(Texture2D value)
    {
        mainTexture = value;
        UpdateMaterial();
    }
    
    // 预设方法
    public void ApplyNormalPreset()
    {
        contrast = 0f;
        brightness = 0f;
        saturation = 0f;
        UpdateMaterial();
    }
    
    public void ApplyHighContrastPreset()
    {
        contrast = 1.0f;
        brightness = 0.1f;
        saturation = 0.3f;
        UpdateMaterial();
    }
    
    public void ApplyVintagePreset()
    {
        contrast = -0.2f;
        brightness = 0.15f;
        saturation = -0.3f;
        hueShift = 15f;
        useHueShift = true;
        UpdateMaterial();
    }
    
    public void ApplyBlackWhitePreset()
    {
        contrast = 0.5f;
        brightness = 0f;
        saturation = -1f;
        UpdateMaterial();
    }
    
    // 获取材质
    public Material GetMaterial()
    {
        if (material == null && meshRenderer != null)
        {
            var materials = meshRenderer.materials;
            if (materials != null && materials.Length > 0)
            {
                material = materials[0];
            }
        }
        return material;
    }
    
    // 重置到默认值
    public void ResetToDefault()
    {
        contrast = 0f;
        brightness = 0f;
        saturation = 0f;
        alpha = 1f;
        tintColor = Color.white;
        useHueShift = false;
        hueShift = 0f;
        useGamma = false;
        gamma = 1f;
        UpdateMaterial();
    }
    
#if UNITY_EDITOR
    // 编辑器专用方法
    void OnDrawGizmosSelected()
    {
        // 在场景视图中显示参数信息
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + Vector3.up * 2;
        
        string info = $"对比度: {contrast:F2}\n" +
                     $"亮度: {brightness:F2}\n" +
                     $"饱和度: {saturation:F2}";
        
        UnityEditor.Handles.Label(position, info);
    }
#endif
}