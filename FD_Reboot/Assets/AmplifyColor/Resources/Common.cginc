// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_COLOR_COMMON_INCLUDED
#define AMPLIFY_COLOR_COMMON_INCLUDED

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform sampler2D _RgbTex;
uniform sampler2D _LerpRgbTex;
uniform sampler2D _RgbBlendCacheTex;
uniform sampler2D _MaskTex;
uniform float4 _MaskTex_TexelSize;
uniform half _lerpAmount;

struct v2f
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
	o.uv = v.texcoord.xy;
	o.uv1 = v.texcoord.xy;
#if defined( UNITY_UV_STARTS_AT_TOP )
	if ( _MainTex_TexelSize.y < 0 )
		o.uv1.y = 1 - o.uv1.y;
#endif
#if defined( UNITY_HALF_TEXEL_OFFSET )
	o.uv1.xy += _MaskTex_TexelSize.xy * float2( -0.5, 0.5 );
#endif
	return o;
}

inline fixed4 to_srgb( fixed4 c )
{
	c.rgb = ( c.rgb < half3( 0.0031308, 0.0031308, 0.0031308 ) ) ? 12.92 * c.rgb : 1.055 * pow( c.rgb, 0.41666 ) - 0.055;
	return c;
}

inline fixed4 to_linear( fixed4 c )
{
	c.rgb = ( c.rgb <= half3( 0.04045, 0.04045, 0.04045 ) ) ? c.rgb * ( 1.0 / 12.92 ) : pow( c.rgb * ( 1.0 / 1.055 ) + ( 0.055 / 1.055 ), 2.4 );
	return c;
}

inline fixed4 apply_lut( fixed4 color, sampler2D lut )
{
	const float4 coord_scale = float4( 0.0302734375, 0.96875, 31.0, 0.0 );
	const float4 coord_offset = float4( 0.00048828125, 0.015625, 0.0, 0.0 );
	const float2 texel_height_X0 = float2( 0.03125, 0.0 );

	float4 coord = color * coord_scale + coord_offset;

#if AC_QUALITY_STANDARD
	fixed4 coord_frac = frac( coord );
	float4 coord_floor = coord - coord_frac;

	float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;
	float2 coord_top = coord_bot + texel_height_X0;

	fixed4 lutcol_bot = tex2D( lut, coord_bot );
	fixed4 lutcol_top = tex2D( lut, coord_top );

	return lerp( lutcol_bot, lutcol_top, coord_frac.z );
#endif

#if AC_QUALITY_MOBILE
	float4 coord_floor = floor( coord + 0.5 );

	float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;

	fixed4 lutcol_bot = tex2D( lut, coord_bot );

	return lutcol_bot;
#endif
}

inline fixed4 apply( fixed4 color )
{
	return apply_lut( color, _RgbTex );
}

inline fixed4 apply_blend( fixed4 color )
{
	return apply_lut( color, _RgbBlendCacheTex );
}

#endif
