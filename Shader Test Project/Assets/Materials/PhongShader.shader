Shader "Custom/PhongShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf PhongLike

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Alpha = c.a;
        }

        float4 LightingPhongLike(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            //Phong
            float3 ReV = reflect(normalize(-lightDir), normalize(s.Normal));
            float rdotv = saturate(dot(ReV, viewDir) * 0.5 + 0.5);
            //float rdotv = saturate(dot(ReV, viewDir));
            rdotv = pow(rdotv,20);


            //Specular
            float3 h = normalize(viewDir + lightDir);
            float spec = saturate(dot(s.Normal, h));
            spec = pow(spec, 10);
            float3 specCol = spec * _LightColor0.rgb;

            float4 FinalColor;
            FinalColor.rgb = rdotv + specCol;
            FinalColor.a = s.Alpha;

            return FinalColor;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
