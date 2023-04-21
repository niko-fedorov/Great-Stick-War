Shader "Custom/Stick Shader" 
{
Properties 
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    [Enum(Entity,0,Ground,1,Wood,2,Metal,3,Glass,4)] _SurfaceType("Surface Type", Int) = 0
}
SubShader 
{
    Tags { "RenderType"="Opaque" }
    LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;

struct Input 
{
    float2 uv_MainTex;
    float4 color: Color;
};

void surf (Input IN, inout SurfaceOutput o) 
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb * IN.color.rgb;
    o.Alpha = c.a * IN.color.a;
}
ENDCG
}

Fallback "Legacy Shaders/VertexLit"
}