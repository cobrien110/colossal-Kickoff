Shader "Custom/GrassShader"
{
    Properties
    {
        _GroundTexture("Ground Texture", 2D) = "white" {}
        _BaseColor("Base Gradient Color", Color) = (0.3, 0.8, 0.3, 1)
        _TipColor("Tip Color", Color) = (0.5, 1, 0.5, 1)
        _GradientColor("Gradient Color", Color) = (0.1, 0.5, 0.1, 1)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Cull Off // Render both sides of the faces

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    float3 normal : TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_INSTANCE_ID
                };

                sampler2D _GroundTexture;
                float4 _BaseColor;
                float4 _TipColor;
                float4 _GradientColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.uv = v.uv;
                    o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Sample ground texture color
                    fixed4 groundColor = tex2D(_GroundTexture, i.uv);

                // Compute gradient based on height (Y-axis)
                float height = saturate(i.worldPos.y / 5.0); // Adjust scale as needed
                fixed4 gradientColor = lerp(_BaseColor, _GradientColor, height);

                // Add tip color
                float tipFactor = smoothstep(0.8, 1.0, height); // Blend tip color
                fixed4 tipColor = lerp(gradientColor, _TipColor, tipFactor);

                // Combine ground texture color and gradient
                fixed4 finalColor = groundColor * tipColor;

                return finalColor;
            }
            ENDCG
        }
        }
}