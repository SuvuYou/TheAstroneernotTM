Shader "Custom/VertexColorWithShading"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 color : COLOR; // Pass vertex color to the fragment shader
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.color = v.color; // Pass vertex color through
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the normal direction
                float3 normal = normalize(i.worldNormal);

                // Get the main directional light color and direction
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;

                // Calculate diffuse lighting
                float NdotL = max(dot(normal, lightDir), 0);
                float3 diffuse = i.color.rgb * lightColor * NdotL;

                // Add ambient lighting
                float3 ambient = i.color.rgb * UNITY_LIGHTMODEL_AMBIENT.xyz;

                // Combine diffuse and ambient for the final color
                return float4(diffuse + ambient, i.color.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}