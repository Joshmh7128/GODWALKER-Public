Shader "Custom/NormalColor"
{
	Properties
	{
		_Col1("X Color", Color) = (1,1,1,1)
		_Col2("Y Color", Color) = (0,0,0,1)
		_Col3("Z Color", Color) = (0,0,0,1)
	}
	SubShader
	{
		 Tags { "RenderType" = "Opaque" }
		 CGPROGRAM
		 #pragma surface surf Lambert
		 struct Input 
		 {
			 float4 color : COLOR;
			 float3 worldNormal;
		 };
		 float3 _Col1;
		 float3 _Col2;
		 float3 _Col3;
		 void surf(Input i, inout SurfaceOutput o) 
		 {
			 float3 normal = i.worldNormal;
			 float normalMask = normal.x + normal.y + normal.z;
			 o.Albedo = _Col1 * abs(normal.x) + _Col2 * abs(normal.y) + _Col3 * abs(normal.z);
		 }
		 ENDCG
	}
	Fallback "Diffuse"
}
