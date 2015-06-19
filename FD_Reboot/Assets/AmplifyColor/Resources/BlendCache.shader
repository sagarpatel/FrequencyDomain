// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Color/BlendCache" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	Subshader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile AC_QUALITY_STANDARD AC_QUALITY_MOBILE
				#include "Common.cginc"

				fixed4 frag( v2f i ) : COLOR
				{
					fixed4 lut1 = tex2D( _RgbTex, i.uv );
					fixed4 lut2 = tex2D( _LerpRgbTex, i.uv );
					return lerp( lut1, lut2, _lerpAmount );
				}
			ENDCG
		}
	}

Fallback off

} // shader
