Shader "Custom/FD_Custom_ParallaxDiffuse" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 150
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd

	
		fixed4 _Color;
		float _Parallax = 0.02;

		struct Input 
		{
			float3 viewDir;

		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			//half h = 0; //tex2D (_ParallaxMap, IN.uv_BumpMap).w;
			//float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
			//IN.uv_MainTex += offset;
			//IN.uv_BumpMap += offset;
			
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			//o.Alpha = c.a;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
