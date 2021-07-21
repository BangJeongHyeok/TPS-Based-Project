Shader "Custom/CelShader"
{
    Properties
    {
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Color3 ("Color3", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Value("Value", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf _BandedLighting

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        int _Value;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        float4 Lighting_BandedLighting(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            //! BandedDiffuse 조명 처리 연산
            float3 fBandedDiffuse;
            float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;    //! Half Lambert 공식
 
            fBandedDiffuse = ceil(fNDotL * _Value) / _Value; 
            
            float4 ToneColor = fNDotL > 0.7 ? _Color1 :(fNDotL > 0.4 ? _Color2 : _Color3);

            //! 최종 컬러 출력
            float4 fFinalColor;
            fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * _LightColor0.rgb * atten * ToneColor;
            fFinalColor.a = s.Alpha;
 
            return fFinalColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
