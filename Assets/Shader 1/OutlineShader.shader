Shader "Custom/SpriteOutlineShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 10
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offsets[8] = {
                    float2(-1, 0), float2(1, 0),
                    float2(0, -1), float2(0, 1),
                    float2(-1, -1), float2(-1, 1),
                    float2(1, -1), float2(1, 1)
                };

                float alpha = 0.0;
                for (int j = 0; j < 8; j++)
                {
                    float2 offsetUV = i.uv + offsets[j] * _OutlineThickness / _ScreenParams.xy;
                    alpha += tex2D(_MainTex, offsetUV).a;
                }

                if (alpha > 0.0 && tex2D(_MainTex, i.uv).a == 0.0)
                {
                    return _OutlineColor;
                }

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
