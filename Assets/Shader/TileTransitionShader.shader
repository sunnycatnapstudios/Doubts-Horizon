Shader "Custom/TileTransitionShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _TransitionColor ("Transition Color", Color) = (0, 0, 0, 1)
        _GradientDirection ("Gradient Direction", Vector) = (0, 1, 0)
        _TilemapSize ("Tilemap Size", Vector) = (10, 10, 0)
        _GradientStart ("Gradient Start", Range(0, 1)) = 0
        _GradientEnd ("Gradient End", Range(0, 1)) = 1
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 tilemapUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TransitionColor;
            float3 _GradientDirection;
            float2 _TilemapSize;
            float _GradientStart;
            float _GradientEnd;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.tilemapUV = v.vertex.xy / _TilemapSize;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gradientAmount = (i.tilemapUV.y - _GradientStart) / (_GradientEnd - _GradientStart);
                gradientAmount = clamp(gradientAmount, 0, 1);
                col.rgb = lerp(col.rgb, _TransitionColor.rgb, gradientAmount);
                return col;
            }
            ENDCG
        }
    }
}
