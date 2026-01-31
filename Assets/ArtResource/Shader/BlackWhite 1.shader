Shader "Hidden/BlackWhite 1"
{
Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1.0
        _Contrast ("Contrast", Range(0.5, 3)) = 1.5      // 对比度
        _Brightness ("Brightness", Range(-1, 1)) = 0.1  // 亮度
        _Gamma ("Gamma", Range(0.5, 2)) = 1.2           // Gamma校正
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    
    SubShader
    {
        // ... 标签和设置保持不变 ...
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            fixed4 _Color;
            float _GrayscaleAmount;
            float _Contrast;    // 新增
            float _Brightness;  // 新增
            float _Gamma;       // 新增
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }
            
            sampler2D _MainTex;
            
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 计算灰度
                float gray = dot(c.rgb, float3(0.299, 0.587, 0.114));
                
                // 应用灰度化
                c.rgb = lerp(c.rgb, gray, _GrayscaleAmount);
                
                // 只在灰度化时应用增强效果
                if (_GrayscaleAmount > 0)
                {
                    // 1. 对比度
                    c.rgb = (c.rgb - 0.5) * _Contrast + 0.5;
                    
                    // 2. 亮度
                    c.rgb += _Brightness;
                    
                    // 3. Gamma校正（让黑色更深）
                    c.rgb = pow(c.rgb, _Gamma);
                    
                    // 确保在有效范围内
                    c.rgb = saturate(c.rgb);
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
