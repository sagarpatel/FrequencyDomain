// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif
#if !UNITY_4 && !UNITY_5
#define UNITY_3
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using AmplifyColor;

namespace AmplifyColor
{
	public enum Quality
	{
		Mobile,
		Standard
	}
}

[AddComponentMenu( "" )]
public class AmplifyColorBase : MonoBehaviour
{
	public const int LutSize = 32;
	public const int LutWidth = LutSize * LutSize;
	public const int LutHeight = LutSize;

	public AmplifyColor.Quality QualityLevel = AmplifyColor.Quality.Standard;
	public float BlendAmount = 0f;
	public Texture LutTexture = null;
	public Texture LutBlendTexture = null;
	public Texture MaskTexture = null;
	public bool UseVolumes = false;
	public float ExitVolumeBlendTime = 1.0f;
	public Transform TriggerVolumeProxy = null;
	public LayerMask VolumeCollisionMask = ~0;

	private Shader shaderBase = null;
	private Shader shaderBlend = null;
	private Shader shaderBlendCache = null;
	private Shader shaderMask = null;
	private Shader shaderBlendMask = null;
	private RenderTexture blendCacheLut = null;
	private Texture2D defaultLut = null;
	private ColorSpace colorSpace = ColorSpace.Uninitialized;
	private AmplifyColor.Quality qualityLevel = AmplifyColor.Quality.Standard;

	public Texture2D DefaultLut { get { return ( defaultLut == null ) ? CreateDefaultLut() : defaultLut ; } }

	private Material materialBase = null;
	private Material materialBlend = null;
	private Material materialBlendCache = null;
	private Material materialMask = null;
	private Material materialBlendMask = null;

	private bool blending;
	private float blendingTime;
	private float blendingTimeCountdown;
	private System.Action onFinishBlend;

	private bool volumesBlending;
	private float volumesBlendingTime;
	private float volumesBlendingTimeCountdown;
	private Texture volumesLutBlendTexture = null;
	private float volumesBlendAmount = 0f;

	public bool IsBlending { get { return blending; } }

	private Texture worldLUT = null;
	private AmplifyColorVolumeBase currentVolumeLut = null;
	private RenderTexture midBlendLUT = null;
	private bool blendingFromMidBlend = false;

	private VolumeEffect worldVolumeEffects = null;
	private VolumeEffect currentVolumeEffects = null;
	private VolumeEffect blendVolumeEffects = null;
	private float effectVolumesBlendAdjust = 0.0f;
	private float effectVolumesBlendAdjusted { get { return Mathf.Clamp01( effectVolumesBlendAdjust < 0.99f ? ( volumesBlendAmount - effectVolumesBlendAdjust ) / ( 1.0f - effectVolumesBlendAdjust ) : 1.0f ); } }
	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();
	private AmplifyColorTriggerProxy actualTriggerProxy = null;

	[HideInInspector] public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

#if TRIAL
	private Texture2D watermark = null;
#endif

	public bool WillItBlend { get { return LutTexture != null && LutBlendTexture != null && !blending; } }

	void ReportMissingShaders()
	{
		Debug.LogError( "[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color." );
		enabled = false;
	}

	void ReportNotSupported()
	{
		Debug.LogError( "[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses." );
		enabled = false;
	}

	bool CheckShader( Shader s )
	{
		if ( s == null )
		{
			ReportMissingShaders();
			return false;
		}
		if ( !s.isSupported )
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	bool CheckShaders()
	{
		return CheckShader( shaderBase ) && CheckShader( shaderBlend ) && CheckShader( shaderBlendCache ) &&
			CheckShader( shaderMask ) && CheckShader( shaderBlendMask );
	}

	bool CheckSupport()
	{
		// Disable if we don't support image effect or render textures
		if ( !SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures )
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	void OnEnable()
	{
		if ( !CheckSupport() )
			return;

		if ( !CreateMaterials() )
			return;

		Texture2D lutTex2d = LutTexture as Texture2D;
		Texture2D lutBlendTex2d = LutBlendTexture as Texture2D;

		if ( ( lutTex2d != null && lutTex2d.mipmapCount > 1 ) || ( lutBlendTex2d != null && lutBlendTex2d.mipmapCount > 1 ) )
			Debug.LogError( "[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. " +
				"Change Texture Type to \"Advanced\" to access Mip settings." );

	#if TRIAL
		watermark = new Texture2D( 4, 4 ) { hideFlags = HideFlags.HideAndDontSave };
		watermark.LoadImage( AmplifyColor.Watermark.ImageData );
	#endif
	}

	void OnDisable()
	{
		if ( actualTriggerProxy != null )
		{
			DestroyImmediate( actualTriggerProxy.gameObject );
			actualTriggerProxy = null;
		}

		ReleaseMaterials();
		ReleaseTextures();

	#if TRIAL
		if ( watermark != null )
		{
			DestroyImmediate( watermark );
			watermark = null;
		}
	#endif
	}

	private void VolumesBlendTo( Texture blendTargetLUT, float blendTimeInSec )
	{
		volumesLutBlendTexture = blendTargetLUT;
		volumesBlendAmount = 0.0f;
		volumesBlendingTime = blendTimeInSec;
		volumesBlendingTimeCountdown = blendTimeInSec;
		volumesBlending = true;

	}

	public void BlendTo( Texture blendTargetLUT, float blendTimeInSec, System.Action onFinishBlend )
	{
		LutBlendTexture = blendTargetLUT;
		BlendAmount = 0.0f;
		this.onFinishBlend = onFinishBlend;
		blendingTime = blendTimeInSec;
		blendingTimeCountdown = blendTimeInSec;
		blending = true;
	}

	private void Start()
	{
		worldLUT = LutTexture;
		worldVolumeEffects = EffectFlags.GenerateEffectData( this );
		blendVolumeEffects = currentVolumeEffects = worldVolumeEffects;
	}

	void Update()
	{
		if ( volumesBlending )
		{
			volumesBlendAmount = ( volumesBlendingTime - volumesBlendingTimeCountdown ) / volumesBlendingTime;
			volumesBlendingTimeCountdown -= Time.smoothDeltaTime;

			if ( volumesBlendAmount >= 1.0f )
			{
				LutTexture = volumesLutBlendTexture;
				volumesBlendAmount = 0.0f;
				volumesBlending = false;
				volumesLutBlendTexture = null;

				effectVolumesBlendAdjust = 0.0f;
				currentVolumeEffects = blendVolumeEffects;
				currentVolumeEffects.SetValues( this );

				if ( blendingFromMidBlend && midBlendLUT != null )
					midBlendLUT.DiscardContents();

				blendingFromMidBlend = false;
			}
		}
		else
			volumesBlendAmount = Mathf.Clamp01( volumesBlendAmount );

		if ( blending )
		{
			BlendAmount = ( blendingTime - blendingTimeCountdown ) / blendingTime;
			blendingTimeCountdown -= Time.smoothDeltaTime;

			if ( BlendAmount >= 1.0f )
			{
				LutTexture = LutBlendTexture;
				BlendAmount = 0.0f;
				blending = false;
				LutBlendTexture = null;

				if ( onFinishBlend != null )
					onFinishBlend();
			}
		}
		else
			BlendAmount = Mathf.Clamp01( BlendAmount );

		if ( UseVolumes )
		{
			if ( actualTriggerProxy == null )
			{
				GameObject obj = new GameObject( name + "+ACVolumeProxy" ) { hideFlags = HideFlags.HideAndDontSave };
				actualTriggerProxy = obj.AddComponent<AmplifyColorTriggerProxy>();
				actualTriggerProxy.OwnerEffect = this;
			}

			UpdateVolumes();
		}
		else if ( actualTriggerProxy != null )
		{
			DestroyImmediate( actualTriggerProxy.gameObject );
			actualTriggerProxy = null;
		}
	}

	public void EnterVolume( AmplifyColorVolumeBase volume )
	{
		if ( !enteredVolumes.Contains( volume ) )
			enteredVolumes.Insert( 0, volume );
	}

	public void ExitVolume( AmplifyColorVolumeBase volume )
	{
		if ( enteredVolumes.Contains( volume ) )
			enteredVolumes.Remove( volume );
	}

	private void UpdateVolumes()
	{
		if ( volumesBlending )
			currentVolumeEffects.BlendValues( this, blendVolumeEffects, effectVolumesBlendAdjusted );

		Transform reference = ( TriggerVolumeProxy == null ) ? transform : TriggerVolumeProxy;

		if ( actualTriggerProxy.transform.parent != reference )
		{
			actualTriggerProxy.Reference = reference;
			actualTriggerProxy.gameObject.layer = reference.gameObject.layer;
		}

		AmplifyColorVolumeBase foundVolume = null;
		int maxPriority = int.MinValue;

		// Find volume with higher priority
		foreach ( AmplifyColorVolumeBase vol in enteredVolumes )
		{
			if ( vol.Priority > maxPriority )
			{
				foundVolume = vol;
				maxPriority = vol.Priority;
			}
		}

		// Trigger blend on volume transition
		if ( foundVolume != currentVolumeLut )
		{
			currentVolumeLut = foundVolume;
			Texture blendTex = ( foundVolume == null ? worldLUT : foundVolume.LutTexture );
			float blendTime = ( foundVolume == null ? ExitVolumeBlendTime : foundVolume.EnterBlendTime );

			if ( volumesBlending && !blendingFromMidBlend && blendTex == LutTexture )
			{
				// Going back to previous volume optimization
				LutTexture = volumesLutBlendTexture;
				volumesLutBlendTexture = blendTex;
				volumesBlendingTimeCountdown = blendTime * ( ( volumesBlendingTime - volumesBlendingTimeCountdown ) / volumesBlendingTime );
				volumesBlendingTime = blendTime;
				currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect( EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted );
				effectVolumesBlendAdjust = 1 - volumesBlendAmount;
				volumesBlendAmount = 1 - volumesBlendAmount;
			}
			else
			{
				if ( volumesBlending )
				{
					materialBlendCache.SetFloat( "_lerpAmount", volumesBlendAmount );

					if ( blendingFromMidBlend )
					{
						Graphics.Blit( midBlendLUT, blendCacheLut );
						materialBlendCache.SetTexture( "_RgbTex", blendCacheLut );
					}
					else
						materialBlendCache.SetTexture( "_RgbTex", LutTexture );

					materialBlendCache.SetTexture( "_LerpRgbTex", ( volumesLutBlendTexture != null ) ? volumesLutBlendTexture : defaultLut );

					Graphics.Blit( midBlendLUT, midBlendLUT, materialBlendCache );

					blendCacheLut.DiscardContents();
				#if UNITY_4
					midBlendLUT.MarkRestoreExpected();
				#endif

					currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect( EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted );
					effectVolumesBlendAdjust = 0.0f;
					blendingFromMidBlend = true;
				}
				VolumesBlendTo( blendTex, blendTime );
			}

			blendVolumeEffects = ( foundVolume == null ? worldVolumeEffects : foundVolume.EffectContainer.GetVolumeEffect( this ) );
			if ( blendVolumeEffects == null )
				blendVolumeEffects = worldVolumeEffects;
		}
	}

	private void SetupShader()
	{
		colorSpace = QualitySettings.activeColorSpace;
		qualityLevel = QualityLevel;
		string linear = ( colorSpace == ColorSpace.Linear ) ? "Linear" : "";

		shaderBase = Shader.Find( "Hidden/Amplify Color/Base" + linear );
		shaderBlend = Shader.Find( "Hidden/Amplify Color/Blend" + linear );
		shaderBlendCache = Shader.Find( "Hidden/Amplify Color/BlendCache" );
		shaderMask = Shader.Find( "Hidden/Amplify Color/Mask" + linear );
		shaderBlendMask = Shader.Find( "Hidden/Amplify Color/BlendMask" + linear );
	}

	private void ReleaseMaterials()
	{
		if ( materialBase != null )
		{
			DestroyImmediate( materialBase );
			materialBase = null;
		}
		if ( materialBlend != null )
		{
			DestroyImmediate( materialBlend );
			materialBlend = null;
		}
		if ( materialBlendCache != null )
		{
			DestroyImmediate( materialBlendCache );
			materialBlendCache = null;
		}
		if ( materialMask != null )
		{
			DestroyImmediate( materialMask );
			materialMask = null;
		}
		if ( materialBlendMask != null )
		{
			DestroyImmediate( materialBlendMask );
			materialBlendMask = null;
		}
	}

	private Texture2D CreateDefaultLut()
	{
		const int maxSize = LutSize - 1;

		defaultLut = new Texture2D( LutWidth, LutHeight, TextureFormat.RGB24, false, true ) { hideFlags = HideFlags.HideAndDontSave };
		defaultLut.name = "DefaultLut";
		defaultLut.hideFlags = HideFlags.DontSave;
		defaultLut.anisoLevel = 1;
		defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] colors = new Color32[ LutWidth * LutHeight ];

		for ( int z = 0; z < LutSize; z++ )
		{
			int zoffset = z * LutSize;

			for ( int y = 0; y < LutSize; y++ )
			{
				int yoffset = zoffset + y * LutWidth;

				for ( int x = 0; x < LutSize; x++ )
				{
					float fr = x / ( float ) maxSize;
					float fg = y / ( float ) maxSize;
					float fb = z / ( float ) maxSize;
					byte br = ( byte ) ( fr * 255 );
					byte bg = ( byte ) ( fg * 255 );
					byte bb = ( byte ) ( fb * 255 );
					colors[ yoffset + x ] = new Color32( br, bg, bb, 255 );
				}
			}
		}

		defaultLut.SetPixels32( colors );
		defaultLut.Apply();

		return defaultLut;
	}

	private void CreateHelperTextures()
	{
		ReleaseTextures();

		blendCacheLut = new RenderTexture( LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear ) { hideFlags = HideFlags.HideAndDontSave };
		blendCacheLut.name = "BlendCacheLut";
		blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		blendCacheLut.useMipMap = false;
		blendCacheLut.anisoLevel = 0;
		blendCacheLut.Create();

		midBlendLUT = new RenderTexture( LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear ) { hideFlags = HideFlags.HideAndDontSave };
		midBlendLUT.name = "MidBlendLut";
		midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		midBlendLUT.useMipMap = false;
		midBlendLUT.anisoLevel = 0;
		midBlendLUT.Create();
	#if UNITY_4
		midBlendLUT.MarkRestoreExpected();
	#endif

		CreateDefaultLut();
	}

	bool CheckMaterialAndShader( Material material, string name )
	{
		if ( material == null || material.shader == null )
		{
			Debug.LogError( "[AmplifyColor] Error creating " + name + " material. Effect disabled." );
			enabled = false;
		}
		else if ( !material.shader.isSupported )
		{
			Debug.LogError( "[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled." );
			enabled = false;
		}
		else
			material.hideFlags = HideFlags.HideAndDontSave;
		return enabled;
	}

#if !UNITY_3
	void SwitchToMobile( Material mat )
	{
		mat.EnableKeyword( "AC_QUALITY_MOBILE" );
		mat.DisableKeyword( "AC_QUALITY_STANDARD" );
	}

	void SwitchToStandard( Material mat )
	{
		mat.EnableKeyword( "AC_QUALITY_STANDARD" );
		mat.DisableKeyword( "AC_QUALITY_MOBILE" );
	}
#endif

	private bool CreateMaterials()
	{
		SetupShader();
		if ( !CheckShaders() )
			return false;

		ReleaseMaterials();

		materialBase = new Material( shaderBase );
		materialBlend = new Material( shaderBlend );
		materialBlendCache = new Material( shaderBlendCache );
		materialMask = new Material( shaderMask );
		materialBlendMask = new Material( shaderBlendMask );

		CheckMaterialAndShader( materialBase, "BaseMaterial" );
		CheckMaterialAndShader( materialBlend, "BlendMaterial" );
		CheckMaterialAndShader( materialBlendCache, "BlendCacheMaterial" );
		CheckMaterialAndShader( materialMask, "MaskMaterial" );
		CheckMaterialAndShader( materialBlendMask, "BlendMaskMaterial" );

		if ( !enabled )
			return false;

		if ( QualityLevel == AmplifyColor.Quality.Mobile )
		{
		#if UNITY_3
			Shader.EnableKeyword( "AC_QUALITY_MOBILE" );
			Shader.DisableKeyword( "AC_QUALITY_STANDARD" );
		#else
			SwitchToMobile( materialBase );
			SwitchToMobile( materialBlend );
			SwitchToMobile( materialBlendCache );
			SwitchToMobile( materialMask );
			SwitchToMobile( materialBlendMask );
		#endif
		}
		else
		{
		#if UNITY_3
			Shader.DisableKeyword( "AC_QUALITY_MOBILE" );
			Shader.EnableKeyword( "AC_QUALITY_STANDARD" );
		#else
			SwitchToStandard( materialBase );
			SwitchToStandard( materialBlend );
			SwitchToStandard( materialBlendCache );
			SwitchToStandard( materialMask );
			SwitchToStandard( materialBlendMask );
		#endif
		}

		CreateHelperTextures();
		return true;
	}

	private void ReleaseTextures()
	{
		if ( blendCacheLut != null )
		{
			DestroyImmediate( blendCacheLut );
			blendCacheLut = null;
		}

		if ( midBlendLUT != null )
		{
			DestroyImmediate( midBlendLUT );
			midBlendLUT = null;
		}

		if ( defaultLut != null )
		{
			DestroyImmediate( defaultLut );
			defaultLut = null;
		}
	}

	public static bool ValidateLutDimensions( Texture lut )
	{
		bool valid = true;
		if ( lut != null )
		{
			if ( ( lut.width / lut.height ) != lut.height )
			{
				Debug.LogWarning( "[AmplifyColor] Lut " + lut.name + " has invalid dimensions." );
				valid = false;
			}
			else
			{
				if ( lut.anisoLevel != 0 )
					lut.anisoLevel = 0;
			}
		}
		return valid;
	}

	private void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		BlendAmount = Mathf.Clamp01( BlendAmount );

		if ( colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel )
			CreateMaterials();

		bool validLut = ValidateLutDimensions( LutTexture );
		bool validLutBlend = ValidateLutDimensions( LutBlendTexture );
		bool skip = ( LutTexture == null && LutBlendTexture == null && volumesLutBlendTexture == null );

		if ( !validLut || !validLutBlend || skip )
		{
			Graphics.Blit( source, destination );
			return;
		}

		Texture lut = ( LutTexture == null ) ? defaultLut : LutTexture;
		Texture lutBlend = LutBlendTexture;

		int pass = !GetComponent<Camera>().hdr ? 0 : 1;
		bool blend = ( BlendAmount != 0.0f ) || blending;
		bool requiresBlend = blend || ( blend && lutBlend != null );
		bool useBlendCache = requiresBlend;

		Material material;
		if ( requiresBlend || volumesBlending )
		{
			if ( MaskTexture != null )
				material = materialBlendMask;
			else
				material = materialBlend;
		}
		else
		{
			if ( MaskTexture != null )
				material = materialMask;
			else
				material = materialBase;
		}

		material.SetFloat( "_lerpAmount", BlendAmount );
		if ( MaskTexture != null )
			material.SetTexture( "_MaskTex", MaskTexture );

		if ( volumesBlending )
		{
			volumesBlendAmount = Mathf.Clamp01( volumesBlendAmount );
			materialBlendCache.SetFloat( "_lerpAmount", volumesBlendAmount );

			if ( blendingFromMidBlend )
				materialBlendCache.SetTexture( "_RgbTex", midBlendLUT );
			else
				materialBlendCache.SetTexture( "_RgbTex", lut );

			materialBlendCache.SetTexture( "_LerpRgbTex", ( volumesLutBlendTexture != null ) ? volumesLutBlendTexture : defaultLut );

			Graphics.Blit( lut, blendCacheLut, materialBlendCache );
		}

		if ( useBlendCache )
		{
			materialBlendCache.SetFloat( "_lerpAmount", BlendAmount );

			RenderTexture temp = null;
			if ( volumesBlending )
			{
				temp = RenderTexture.GetTemporary( blendCacheLut.width, blendCacheLut.height, blendCacheLut.depth, blendCacheLut.format, RenderTextureReadWrite.Linear );

				Graphics.Blit( blendCacheLut, temp );

				materialBlendCache.SetTexture( "_RgbTex", temp );
			}
			else
				materialBlendCache.SetTexture( "_RgbTex", lut );

			materialBlendCache.SetTexture( "_LerpRgbTex", ( lutBlend != null ) ? lutBlend : defaultLut );

			Graphics.Blit( lut, blendCacheLut, materialBlendCache );

			if ( temp != null )
				RenderTexture.ReleaseTemporary( temp );

			material.SetTexture( "_RgbBlendCacheTex", blendCacheLut );

		}
		else if ( volumesBlending )
		{
			material.SetTexture( "_RgbBlendCacheTex", blendCacheLut );
		}
		else
		{
			if ( lut != null )
				material.SetTexture( "_RgbTex", lut );
			if ( lutBlend != null )
				material.SetTexture( "_LerpRgbTex", lutBlend );
		}

		Graphics.Blit( source, destination, material, pass );

		if ( useBlendCache || volumesBlending )
			blendCacheLut.DiscardContents();
	}

#if TRIAL
	void OnGUI()
	{
		if ( watermark != null )
			GUI.DrawTexture( new Rect( 15, Screen.height - watermark.height - 12, watermark.width, watermark.height ), watermark );
	}
#endif
}
