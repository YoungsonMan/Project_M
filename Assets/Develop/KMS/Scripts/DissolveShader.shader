Shader "Custom/DissolveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (0.1, 0.1, 0.1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade // 그림자 옵션 추가

        sampler2D  _MainTex;
        sampler2D  _NoiseTex;
        float _DissolveAmount;
        fixed4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
        };

        void surf (Input IN, inout SurfaceOutput  o)
        {
             // 기본 텍스처 설정
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = mainColor.rgb;

            // 노이즈 텍스처 값을 통해 Dissolve 효과 구현
            float noiseValue = tex2D(_NoiseTex, IN.uv_NoiseTex).r;
            float dissolve = smoothstep(_DissolveAmount - 0.1, _DissolveAmount + 0.1, noiseValue);

            // 가장자리 색상 효과 추가
            float edgeEffect = smoothstep(_DissolveAmount, _DissolveAmount + 0.05, noiseValue);
            o.Emission = edgeEffect * _EdgeColor.rgb;

            // Alpha 값 설정 (소멸 정도에 따라 Alpha 조절)
            o.Alpha = dissolve;
            clip(dissolve - 0.1); // 그림자도 소멸과 함께 클리핑되도록 설정
        }
        ENDCG
    }
    FallBack "Diffuse"
}
