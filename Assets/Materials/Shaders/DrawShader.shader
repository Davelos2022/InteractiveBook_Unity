Shader "Unlit/DrawShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Open("Open Value", Range(0.0, 1.0)) = 0
        _NoiseSize("Noise Size", Float) = 25
        _Threshold("Noise Threshold", Range(0, 1)) = 0.5
        [Toggle] _Invert("Invert", Float) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Open;
            float _NoiseSize;
            float _Threshold;
            float _Invert;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float hash( float n )
            {
                return frac(sin(n)*43758.5453);
            }
            
            float noise( float3 x )
            {
                // The noise function returns a value in the range -1.0f -> 1.0f

                float3 p = floor(x);
                float3 f = frac(x);

                f       = f*f*(3.0-2.0*f);
                float n = p.x + p.y*57.0 + 113.0*p.z;

                return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
                               lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
                           lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                               lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if(_Open < 1)
                {
                    fixed4 val = (noise(float3(i.uv.x * _NoiseSize, i.uv.y * _NoiseSize, 0)) + 1) * 0.5;
                    
                    float uv = 0;

                    if (_Invert)
                    {
                        uv = i.uv.x + _Open;
                    }
                    else
                    {
                        uv = (1 - i.uv.x) + _Open;
                    }

                    float alpha = col.a;
                    
                    alpha *= (uv*uv*uv*uv*uv) * _Open * val;

                    if (alpha > _Threshold)
                    {
                        alpha = 1;
                    }
                    else
                    {
                        alpha = 0;
                    }
                    
                    col.a *= alpha;
                    col.a = clamp(col.a, 0, 1);
                }
                
                return col;
            }


            ENDCG
        }
    }
}
