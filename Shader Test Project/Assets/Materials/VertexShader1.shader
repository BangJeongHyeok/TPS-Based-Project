Shader "Custom/VertexShader1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _OutLine("OutLine", Range(0,1)) = 0

        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Color3 ("Color3", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        cull front

//1Pass
        CGPROGRAM

        #pragma surface surf Lambert vertex:vert noshadow
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color:COLOR;
        };
        half _OutLine;
        fixed4 _Color;


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        void vert(inout appdata_full v)
        {
            v.vertex.xyz = v.vertex.xyz + v.normal * _OutLine;

            v.vertex.y = v.vertex.y + sin(_Time.y) * 0.1 * v.color.g;
            v.vertex.x = v.vertex.x + cos(_Time.y) * 0.1 * v.color.g;
        }

        float4 Lightingno(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0,0,0,1);   
        }
        ENDCG

//2Pass
        cull back
        CGPROGRAM

        #pragma surface surf Lambert vertex:vert addshadow _BandedLighting
        //#pragma surface surf _Ceil
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color:COLOR;
        };

        fixed4 _Color;

        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        // float4 Lighting_ceil(SurfaceOutput s,float4 lightDir, float atten)
        // {
        //     float ndotl = dot(normalize(s.Normal), normalize(lightDir)) *0.5 + 0.5;
        //     ndotl = ndotl * ndotl * ndotl;

        //     return _Color * ceil(ndotl/3) * 3;
        // }

        float4 Lighting_Ceil(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            //! BandedDiffuse 조명 처리 연산
            float3 fBandedDiffuse;
            float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;    //! Half Lambert 공식
 
            //! 0~1로 이루어진 fNDotL값을 3개의 값으로 고정함 <- Banded Lighting 작업
            float fBandNum = 2.0f;
            fBandedDiffuse = ceil(fNDotL * fBandNum) / fBandNum; 
            
 
            //! 최종 컬러 출력
            float4 fFinalColor;
            if(fNDotL > 0.6)
            fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * _Color1;
            else if(fNDotL > 0.3)
            fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * _Color2;
            else
            fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * _Color3;
            fFinalColor.a = s.Alpha;
 
            return fFinalColor;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = float4(1,1,1,1);
            o.Alpha = c.a;
        }

        void vert(inout appdata_full v)
        {
            v.vertex.y = v.vertex.y + sin(_Time.y) * 0.1 * v.color.g;
            v.vertex.x = v.vertex.x + cos(_Time.y) * 0.1 * v.color.g;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
