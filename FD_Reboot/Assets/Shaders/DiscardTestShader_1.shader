Shader "Custom/DiscardTestShader_1" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		//sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		//half _Glossiness;
		//half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color; //tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0; //_Metallic;
			o.Smoothness = 0; //_Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
