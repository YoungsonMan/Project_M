Shader "Unlit/DissolveHLSL"
{
     Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}      // 기본 텍스처
        _NoiseTex ("Noise Texture", 2D) = "white" {}   // 노이즈 텍스처
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.5 // Dissolve 임계값
        _EdgeColor ("Edge Color", Color) = (1, 0.5, 0, 1)     // 가장자리 색상
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정
        ZWrite Off                      // Z 버퍼 기록 끄기 (반투명 객체 처리)

        Pass
        {
            HLSLPROGRAM
            #include "UnityCG.cginc" // Unity 셰이더 라이브러리 포함
            #pragma vertex vert
            #pragma fragment frag

            // 텍스처와 파라미터
            sampler2D _MainTex;      // 기본 텍스처
            sampler2D _NoiseTex;     // 노이즈 텍스처
            float _DissolveAmount;   // Dissolve 임계값
            float4 _EdgeColor;       // 가장자리 색상

            // 정점 셰이더 입력
            struct appdata
            {
                float4 vertex : POSITION;  // 정점 위치
                float2 uv : TEXCOORD0;     // UV 좌표
            };

            // 정점 셰이더 출력
            struct v2f
            {
                float4 pos : SV_POSITION;  // 화면 좌표
                float2 uv_MainTex : TEXCOORD0;  // 기본 텍스처 UV
                float2 uv_NoiseTex : TEXCOORD1; // 노이즈 텍스처 UV
            };

            // 정점 셰이더
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);  // 월드 → 클립 좌표 변환
                o.uv_MainTex = v.uv;                    // UV 전달
                o.uv_NoiseTex = v.uv;                   // 동일 UV 전달 (필요 시 수정 가능)
                return o;
            }

            // 픽셀 셰이더
            float4 frag(v2f i) : SV_Target
            {
                // 기본 텍스처 샘플링
                float4 mainColor = tex2D(_MainTex, i.uv_MainTex);

                // 노이즈 텍스처 샘플링
                float noiseValue = tex2D(_NoiseTex, i.uv_NoiseTex).r;

                // Dissolve 값 계산
                float dissolve = smoothstep(_DissolveAmount - 0.1, _DissolveAmount + 0.1, noiseValue);

                // 가장자리 효과 계산
                float edgeEffect = smoothstep(_DissolveAmount, _DissolveAmount + 0.05, noiseValue);
                float3 edgeColor = edgeEffect * _EdgeColor.rgb;

                // 알파 및 클리핑
                float alpha = dissolve;
                if (alpha  < 0.1) discard; // 클리핑

                // 최종 색상
                float4 finalColor = float4(mainColor.rgb + edgeColor, dissolve);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
