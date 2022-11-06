Shader "Mobile/Diffuse (Mesh Animation)" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        
        [Header(Mesh Animation)]
        _AnimTex ("Animation", 2D) = "white" {}
        _AnimMul ("Animation Bounds Size", Vector) = (1, 1, 1, 0)
        _AnimAdd ("Animation Bounds Offset", Vector) = (0, 0, 0, 0)
        [PerRendererData] _AnimInfoA ("Animation Info A", Vector) = (0, 20, 1, 0) /* (x: start, y: length, z: speed, w: startTime) */
        [PerRendererData] _AnimInfoB ("Animation Info B", Vector) = (0, 20, 1, 0) /* (x: start, y: length, z: speed, w: startTime) */
        [PerRendererData] _AnimWeight ("A or B Normalized Weight", float) = 0
        [PerRendererData] _AnimLoop ("Animation Loop", Float) = 1
        [PerRendererData] _EmissionColor ("Emission Color", Color) = (0,0,0,1) 
        [PerRendererData] _AnimTime ("Animation Time", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 150
    
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd addshadow vertex:vert
        #pragma multi_compile_instancing 
        #pragma target 2.5
        #pragma require samplelod
        
        sampler2D _MainTex;
        sampler2D _AnimTex;
        float4 _AnimTex_TexelSize;
        
        float4 _AnimMul;
        float4 _AnimAdd;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _AnimInfoA)
            UNITY_DEFINE_INSTANCED_PROP(float4, _AnimInfoB)
            UNITY_DEFINE_INSTANCED_PROP(float, _AnimWeight)
            UNITY_DEFINE_INSTANCED_PROP(float, _AnimLoop)
            UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _AnimTime)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float2 vertcoord: TEXCOORD1;
            float4 color : COLOR0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void vert (inout appdata v) {            
            UNITY_SETUP_INSTANCE_ID(v);
            
            float4 infoA = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimInfoA);
            float4 infoB = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimInfoB);
            float looping = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimLoop);
            float weight = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimWeight);
            
            float t = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimTime);

            /* //Currently assume all anims are loops
            float progressA = (t - infoA.w) * infoA.z;
            float progressB = (t - infoB.w) * infoB.z;
            float progress01A = lerp(saturate(progressA), frac(progressA), looping);
            float progress01B = lerp(saturate(progressB), frac(progressB), looping);
            */
            
            float2 coordsA = float2(0.5 + v.vertcoord.x, 0.5 + infoA.x + t * infoA.y) * _AnimTex_TexelSize.xy;
            float2 coordsB = float2(0.5 + v.vertcoord.x, 0.5 + infoB.x + t * infoB.y) * _AnimTex_TexelSize.xy;
            float4 positionA = tex2Dlod(_AnimTex, float4(coordsA, 0, 0)) * _AnimMul + _AnimAdd;
            float4 positionB = tex2Dlod(_AnimTex, float4(coordsB, 0, 0)) * _AnimMul + _AnimAdd;

            float4 finalPosition = lerp(positionA, positionB, weight);
            
            v.vertex = float4(finalPosition.xyz, 1.0);
        }
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            half3 em = UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionColor);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Emission = em;
        }
        ENDCG
    }
    
    Fallback "Mobile/Diffuse"
}