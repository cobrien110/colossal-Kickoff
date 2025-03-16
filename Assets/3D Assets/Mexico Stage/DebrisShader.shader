Shader "Custom/DebrisShader"
{
    Properties
    {
        //Base texture
        _BaseMap("Base Texture", 2D) = "white" {} 
        //Tint color
        _Color("Color Tint", Color) = (1,1,1,1)   
    }

        SubShader
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
            Pass
            {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                    uint id : SV_InstanceID; //Instance ID for GPU Instancing
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float3 normalWS : NORMAL;
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                };

                TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
                CBUFFER_START(UnityPerMaterial)
                    //Texture Tiling & Offset
                    float4 _BaseMap_ST;  
                    //Color Tint
                    float4 _Color;        
                CBUFFER_END

                StructuredBuffer<float4x4> _TransformBuffer; //GPU Instanced Transform Buffer

                v2f vert(appdata v)
                {
                    v2f o;
                    float4x4 modelMatrix = _TransformBuffer[v.id];

                    //Transform vertex position
                    float4 worldPos = mul(modelMatrix, v.vertex);
                    o.vertex = TransformWorldToHClip(worldPos.xyz);
                    o.worldPos = worldPos.xyz;

                    //Transform normals properly (remove translation effects)
                    o.normalWS = normalize(mul((float3x3)modelMatrix, v.normal));
                    o.uv = TRANSFORM_TEX(v.uv, _BaseMap);

                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    //Sample Base Texture
                    float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                    //Apply Color Tint
                    float4 finalColor = texColor * _Color;

                    //Basic lighting
                    Light mainLight = GetMainLight();
                    float3 lightDir = normalize(mainLight.direction);
                    float diff = saturate(dot(i.normalWS, lightDir));
                    float3 lighting = mainLight.color * diff;

                    return float4(finalColor.rgb * lighting, finalColor.a);
                }

                ENDHLSL
            }
        }
}