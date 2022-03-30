Shader "Unlit/LockOpenShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _T ("t", range(0, 1)) = 0
        _L ("l", range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _T, _L;

            float scl(float x) {
                return x * (1 + 2 * _L);
            }

            float ease(float x) {
                return 1 - (1 - x) * (1 - x);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x = -_L + scl(i.uv.x - sign(i.uv.y - 0.5) * _L * ease(_T));
                if (i.uv.x > 1 || i.uv.x < 0)
                    discard;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // col.xy = sign(i.uv.xy - 0.5);
                   
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                int part = 1.75;
                float t0 = 1.5;
                float t_ = 4.3 - _T - t0;
                float t = floor(t_ * t_ * part);

                if (_T != 0 && (t % 2) == 0)
                    return 1;

                // col.w *= ease(1 - _T);

                return col;
            }
            ENDCG
        }
    }
}
