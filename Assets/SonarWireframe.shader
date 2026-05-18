Shader "Custom/SonarWireframe"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1,0,0,1)
        _LineWidth ("Line Width", Float) = 5
        _PulseWidth ("Pulse Width", Float) = 10
        _PulseSpeed ("Pulse Speed", Float) = 20
        _PulseFade ("Pulse Fade", Float) = 1
        _PulseRadius ("Pulse Radius", Float) = 40
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"            
        }

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define MAX_PULSES 10

            int _PulseCount;
            float4 _PulseOrigins[MAX_PULSES];
            float _PulseTimes[MAX_PULSES];
            float _PulseRadii[MAX_PULSES];
            float _PulseFades[MAX_PULSES];
            float _PulseWidths[MAX_PULSES];

            float _PulseSpeed;
            float _PulseWidth;
            float4 _LineColor;
            float _LineWidth;
            

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
                float3 bary : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2g vert(appdata v)
            {
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;

                float3 bary[3] = {
                    float3(1,0,0),
                    float3(0,1,0),
                    float3(0,0,1)
                };

                for(int i = 0; i < 3; i++)
                {
                    o.pos = input[i].pos;
                    o.bary = bary[i];
                    o.worldPos = input[i].worldPos;
                    triStream.Append(o);
                }
            }

            float edgeFactor(float3 bary)
            {
                float3 d = fwidth(bary);
                float3 a3 = smoothstep(float3(0,0,0), d * _LineWidth, bary);
                return min(min(a3.x, a3.y), a3.z);
            }

            float4 frag(g2f i) : SV_Target
{
                float visible = 0;

                for (int p = 0; p < _PulseCount; p++)
                {
                    float dist = distance(i.worldPos, _PulseOrigins[p].xyz);

                    float radius = _PulseRadii[p];
                    float fade   = _PulseFades[p];

                    float band = abs(dist - radius);
                    float ring = smoothstep(_PulseWidths[p], 0, band);

                    visible += ring * fade;
                }

                visible = saturate(visible);

                float edge = 1 - edgeFactor(i.bary);
                return _LineColor * edge * visible;
            }

            ENDCG
        }
    }
}