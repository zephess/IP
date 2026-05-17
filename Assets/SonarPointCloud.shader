Shader "Custom/SonarPointCloud"
{
    Properties
    {
        _PointSize ("Point Size", Float) = 0.05
        _Color ("Point Color", Color) = (0,1,1,1)
        _PulseWidth ("Pulse Width", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            ZWrite On
            ZTest LEqual
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_PULSES 10

            float _PointSize;
            float4 _Color;
            float _PulseWidth;

            int _PulseCount;
            float4 _PulseOrigins[MAX_PULSES];
            float _PulseTimes[MAX_PULSES];
            float _PulseSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2g
            {
                float4 pos : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2g vert(appdata v)
            {
                v2g o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            
            [maxvertexcount(12)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                float size = _PointSize;

                for(int v = 0; v < 3; v++)
                {
                    float4 clip = input[v].pos;
                    float3 world = input[v].worldPos;

                    float4 offsets[4] =
                    {
                        float4(-size,-size,0,0),
                        float4(size,-size,0,0),
                        float4(size,size,0,0),
                        float4(-size,size,0,0)
                    };

                    float2 uvs[4] =
                    {
                        float2(0,0),
                        float2(1,0),
                        float2(1,1),
                        float2(0,1)
                    };

                    for(int i=0;i<4;i++)
                    {
                        g2f o;

                        o.pos = clip + offsets[i];
                        o.worldPos = world;
                        o.uv = uvs[i];

                        triStream.Append(o);
                    }

                    triStream.RestartStrip();
                }
            }

            float4 frag(g2f i) : SV_Target
            {
                float visible = 0;

                for(int p=0; p<_PulseCount; p++)
                {
                    float dist = distance(i.worldPos, _PulseOrigins[p].xyz);

                    float wave = _PulseSpeed * (_Time.y - _PulseTimes[p]);

                    float band = abs(dist - wave);

                    visible += smoothstep(_PulseWidth, 0, band);
                }

                visible = saturate(visible);

                return _Color * visible;
            }

            ENDCG
        }
    }
}