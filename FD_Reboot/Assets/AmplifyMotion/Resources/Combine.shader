// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Motion/Combine" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MotionTex ("Motion (RGB)", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode off }
		CGINCLUDE
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MotionTex;
			sampler2D _BlurredTex;
			float4 _MainTex_TexelSize;

			struct v2f
			{
				float4 position : POSITION;
				float4 uv : TEXCOORD0;
			};

			v2f vert( appdata_img v )
			{
				v2f o;
				o.position = mul( UNITY_MATRIX_MVP, v.vertex );
				o.uv.xy = v.texcoord.xy;
				o.uv.zw = v.texcoord.xy;
			#if defined( UNITY_UV_STARTS_AT_TOP )
				if ( _MainTex_TexelSize.y < 0 )
					o.uv.w = 1 - o.uv.w;
			#endif
				return o;
			}
		ENDCG
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers flash

				half4 frag( v2f i ) : COLOR
				{
					return half4( tex2D( _MainTex, i.uv.xy ).xyz, tex2D( _MotionTex, i.uv.zw ).a + 0.0000001f ); // hack to trick Unity into behaving
				}
			ENDCG
		}
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers flash

				half4 frag( v2f i ) : COLOR
				{
					half4 source = tex2D( _MainTex, i.uv.xy );
					half4 blurred = tex2D( _BlurredTex, i.uv.zw );
					half mag = 2 * tex2D( _MotionTex, i.uv.zw ).z;
					return lerp( source, blurred, saturate( mag * 1.5 ) );
				}
			ENDCG
		}
	}

	Fallback Off
}
