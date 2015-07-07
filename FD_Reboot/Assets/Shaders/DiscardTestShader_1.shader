Shader "Custom/DiscardTestShader_1" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Cutoff", float) = 0.0
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
		#pragma surface surf Standard vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
	
		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
			half3 barryCenterCoord;			
		};
		
		// example from http://forum.unity3d.com/threads/how-can-i-get-tangent-on-a-surface-shader.184441/
		void vert(inout appdata_tan i, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);		
			half3 tangent_normalized = normalize(i.tangent.xyz);
			o.barryCenterCoord = tangent_normalized;
		}
		
		
		//half _Glossiness;
		//half _Metallic;
		fixed4 _Color;
		float _Cutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5 * _SinTime);
			
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color; //tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//c.rgb = float3(IN.uv_MainTex.xy,0);
			
			//c.rgb = float3( IN.barryCenterCoord.xyz);
			
			//clip(-IN.barryCenterCoord.z + 0.5);
			
			//float avr = ( IN.barryCenterCoord.x + IN.barryCenterCoord.y + IN.barryCenterCoord.z )/3.0;
			//clip(IN.barryCenterCoord.x - 0.2);
			float cut = _Cutoff; //0.09;
			
			if(IN.barryCenterCoord.x < cut || IN.barryCenterCoord.y < cut || IN.barryCenterCoord.z < cut)
			{
				//clip(-1);
			}
			else
			{
				clip(-1);
			}
		
			
			
			
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
