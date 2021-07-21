Shader "Custom/FireGroundShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SubTex ("SubTexture", 2D) = "white" {}
        _HeightTex ("HeightMap", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Threshold ("Cutout threshold", Range(0,1)) = 0.1
        _Softness ("Cutout softness", Range(0,0.5)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Cutout" "Queue" = "Transparent"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SubTex;
        sampler2D _BaseTex;
        sampler2D _HeightTex;
        float4 _Color;
        float _Threshold;
        float _Softness;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SubTex;
            float2 uv_HeightTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //fixed4 height = tex2D (_HeightTex, IN.uv_HeightTex + float2(cos(_Time.y),sin(_Time.y)));
            fixed4 height = tex2D (_HeightTex, IN.uv_HeightTex);
            fixed4 sub = tex2D (_SubTex, IN.uv_SubTex);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            fixed3 MainTexture;
            MainTexture = c.rgb * height.r;
            fixed3 SubTexture;
            SubTexture = sub.rgb - height.r;

            //o.Albedo = c.rgb * height.r * _Color + sub.rgb * (1-(height.r + height.g + height.b)*0.333);
            o.Albedo = MainTexture + SubTexture;
            //o.Alpha = smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (height.r + height.g + height.b));
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
