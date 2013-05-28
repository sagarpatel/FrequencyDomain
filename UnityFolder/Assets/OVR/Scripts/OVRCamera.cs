/************************************************************************************

Filename    :   OVRCamera.cs
Content     :   Interface to camera class
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/

//#define MSAA_ENABLED // Not available in Unity 4 as of yet

using UnityEngine;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Camera))]

//-------------------------------------------------------------------------------------
// ***** OVRCamera
//
// OVRCamera is used to render into a Unity Camera class. 
// This component handles reading the Rift tracker and positioning the camera position
// and rotation. It also is responsible for properly rendering the final output, which
// also the final lens correction pass.
//
public class OVRCamera : OVRComponent
{	
	#region Member Variables
	// PRIVATE MEMBERS
	// If CameraTextureScale is not 1.0f, we will render to this texture 
	private RenderTexture	CameraTexture	  	= null;
	// Default material, just blit texture over to final buffer
	private Material		BlitMaterial		= null;	
	// Color only material, used for drawing quads on-screen
	private Material 		ColorOnlyMaterial   = null;
	private Color			QuadColor 			= Color.red;
	// Scaled size of final render buffer
	// A value of 1 will not create a render buffer but will render directly to final
	// backbuffer
 	private float			CameraTextureScale 	= 1.0f;
	
	// We will search for camera controller and set it here for access to its members
	private OVRCameraController CameraController = null;
	
	// PUBLIC MEMBERS
	// camera position...	
	// From root of camera to neck (translation only)
	[HideInInspector]
	public Vector3 NeckPosition = new Vector3(0.0f, 0.0f,  0.0f);	
	// From neck to eye (rotation and translation; x will be different for each eye)
	[HideInInspector]
	public Vector3 EyePosition  = new Vector3(0.0f, 0.09f, 0.16f);
	
	// STATIC MEMBERS
	// We will grab the actual orientation that is used by the cameras in a shared location.
	// This will allow multiple OVRCameraControllers to eventually be uused in a scene, and 
	// only one orientation will be used to syncronize all camera orientation
	static private Quaternion CameraOrientation = Quaternion.identity;
	#endregion
	
	#region Monobehaviour Member Functions	
	// Awake
	new void Awake()
	{
		base.Awake ();
		
		// Material used to blit from one render texture to another
		if(BlitMaterial == null)
		{
			BlitMaterial = new Material (
				"Shader \"BlitCopy\" {\n" +
				"	SubShader { Pass {\n" +
				" 		ZTest Off Cull Off ZWrite Off Fog { Mode Off }\n" +
				"		SetTexture [_MainTex] { combine texture}"	+
				"	}}\n" +
				"Fallback Off }"
			);
		}
		
		// Material used for drawing color only polys into a render texture
		// Used by Latency tester
		if(ColorOnlyMaterial == null)
		{
			ColorOnlyMaterial = new Material (

			    "Shader \"Solid Color\" {\n" +
    			"Properties {\n" +
                "_Color (\"Color\", Color) = (1,1,1)\n" +
                "}\n" +
    			"SubShader {\n" +
    			"Color [_Color]\n" +
    			"Pass {}\n" +
    			"}\n" +
    			"}"		
			);
		}	
	}

	// Start
	new void Start()
	{
		base.Start ();		
		
		// Get the OVRCameraController
		CameraController = gameObject.transform.parent.GetComponent<OVRCameraController>();
		
		if(CameraController == null)
			Debug.LogWarning("WARNING: OVRCameraController not found!");
		
		// NOTE: MSAA TEXTURES NOT AVAILABLE YET
		// Set CameraTextureScale (increases the size of the texture we are rendering into
		// for a better pixel match when post processing the image through lens distortion)
#if MSAA_ENABLED
		CameraTextureScale = OVRDevice.DistortionScale();
#endif		
		// If CameraTextureScale is not 1.0f, create a new texture and assign to target texture
		// Otherwise, fall back to normal camera rendering
		if((CameraTexture == null) && (CameraTextureScale > 1.0f))
		{
			int w = (int)(Screen.width / 2.0f * CameraTextureScale);
			int h = (int)(Screen.height * CameraTextureScale);
			CameraTexture = new RenderTexture(  w, h, 24); // 24 bit colorspace
			
			// NOTE: MSAA TEXTURES NOT AVAILABLE YET
			// This value should be the default for MSAA textures
			// CameraTexture.antiAliasing = 4; 
			// Set it within the project
#if MSAA_ENABLED
			CameraTexture.antiAliasing = QualitySettings.antiAliasing;
#endif
		}
	}

	// Update
	new void Update()
	{
		base.Update ();
	}
	
	// OnPreCull
	void OnPreCull()
	{
		// NOTE: Setting the camera here increases latency, but ensures
		// that all Unity sub-systems that rely on camera location before
		// being set to render are satisfied. 
		if(CameraController.CallInPreRender == false)
			SetCameraOrientation();
	
	}
	
	// OnPreRender
	void OnPreRender()
	{
		// NOTE: Better latency performance here, but messes up water rendering and other
		// systems that rely on the camera to be set before PreCull takes place.
		if(CameraController.CallInPreRender == true)
			SetCameraOrientation();
		
		if(CameraController.WireMode == true)
			GL.wireframe = true;
		
		// Set new buffers and clear color and depth
		if(CameraTexture != null)
		{
			Graphics.SetRenderTarget(CameraTexture);
			GL.Clear (true, true, gameObject.camera.backgroundColor);
		}
	}
	
	// OnPostRender
	void OnPostRender()
	{
		if(CameraController.WireMode == true)
			GL.wireframe = false;
	}
	
	// OnRenderImage
	void  OnRenderImage (RenderTexture source, RenderTexture destination)
	{		
		bool flipImage = false;
		
		// Test for flip: If we are in WindowsEditor or WindowsPlayer, we
		// will make sure to set the flip, since the flip is meant for
		// DirectX. The next version of the integration will remove this 
		// code in favor of flipping in the shader
		if ( (Application.platform == RuntimePlatform.WindowsEditor) ||
    		 (Application.platform == RuntimePlatform.WindowsPlayer) )
		{
			flipImage = true;
		}
		
		Camera cam = gameObject.camera;

		// Use either source input or CameraTexutre, if it exists
		RenderTexture SourceTexture = source;
		
		if (CameraTexture != null)
		{
			SourceTexture = CameraTexture;
			flipImage = false; // If MSAA is on, this will be true
		}
		else
		{
			// Check if quality settings are set
			if( (QualitySettings.antiAliasing == 0) ||
			    (cam.actualRenderingPath == RenderingPath.DeferredLighting) )
			{
				flipImage = false; // If MSAA is on, this will be true
			}
		}
				
		// Replace null material with lens correction material
		Material material = null;
		
		if(CameraController.LensCorrection == true)
		{
			if(CameraController.Chromatic == true)
				material = GetComponent<OVRLensCorrection>().GetMaterial_CA();
			else
				material = GetComponent<OVRLensCorrection>().GetMaterial();				
		}
		
		// Draw to final destination
#if MSAA_ENABLED
		// todo: re-visit how to target final back buffer when MSAA on render
		// targets are enabled in future Unity 4x release
		Blit(SourceTexture, null, material, flipImage);
#else
		Blit(SourceTexture, destination, material, flipImage);
		//Graphics.Blit(SourceTexture, destination, material);
#endif
		// Run latency test by drawing out quads to the destination buffer
		LatencyTest(destination);
		
	}
	#endregion
	
	#region OVRCamera Functions
	// SetCameraOrientation
	void SetCameraOrientation()
	{
		Quaternion q   = Quaternion.identity;
		Vector3    dir = Vector3.forward;		
		
		// Main camera has a depth of 0, so it will be rendered first
		if(gameObject.camera.depth == 0.0f)
		{			
			// If desired, update parent transform y rotation here
			// This is useful if we want to track the current location of
			// of the head.
			// TODO: Future support for x and z, and possibly change to a quaternion
			if(CameraController.TrackerRotatesY == true)
			{
				Vector3 a = gameObject.camera.transform.rotation.eulerAngles;
				a.x = 0; 
				a.z = 0;
				gameObject.transform.parent.transform.eulerAngles = a;
			}
				
			// Read shared data from CameraController	
			if(CameraController != null)
			{				
				// Read sensor here (prediction on or off)
				if(CameraController.PredictionOn == false)
					OVRDevice.GetOrientation(0, ref CameraOrientation);
				else
					OVRDevice.GetPredictedOrientation(0, ref CameraOrientation);				
			}
			
			// This needs to go as close to reading Rift orientation inputs
			OVRDevice.ProcessLatencyInputs();			
		}
		
		// Calculate the rotation Y offset that is getting updated externally
		// (i.e. like a controller rotation)
		float yRotation = 0.0f;
		CameraController.GetYRotation(ref yRotation);
		q = Quaternion.Euler(0.0f, yRotation, 0.0f);
		dir = q * Vector3.forward;
		q.SetLookRotation(dir, Vector3.up);
	
		// Multiply the camera controllers offset orientation (allow follow of orientation offset)
		Quaternion orientationOffset = Quaternion.identity;
		CameraController.GetOrientationOffset(ref orientationOffset);
		q = orientationOffset * q;
		
		// Multiply in the current HeadQuat (q is now the latest best rotation)
		if(CameraController != null)
			q = q * CameraOrientation;
		
		// * * *
		// Update camera rotation
		gameObject.camera.transform.rotation = q;
		
		// * * *
		// Update camera position (first add Offset to parent transform)
		gameObject.camera.transform.position = 
		gameObject.camera.transform.parent.transform.position + NeckPosition;
	
		// Adjust neck by taking eye position and transforming through q
		gameObject.camera.transform.position += q * EyePosition;		
	}
	
	// Blit - Copies one render texture onto another through a material
	// flip will flip the render horizontally
	void Blit (RenderTexture source, RenderTexture dest, Material m, bool flip) 
	{
		Material material = m;
		
		// Default to blitting material if one doesn't get passed in
		if(material == null)
			material = BlitMaterial;
		
		// Make the destination texture the target for all rendering
		RenderTexture.active = dest;  		
		
		// Assign the source texture to a property from a shader
		source.SetGlobalShaderProperty ("_MainTex");	
		
		// Set up the simple Matrix
		GL.PushMatrix ();
		GL.LoadOrtho ();
		for(int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			DrawQuad(flip);
		}
		GL.PopMatrix ();
	}
	
	// DrawQuad
	void DrawQuad(bool flip)
	{
		GL.Begin (GL.QUADS);
		
		if(flip == true)
		{
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex3( 0.0f, 0.0f, 0.1f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex3( 1.0f, 0.0f, 0.1f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex3( 1.0f, 1.0f, 0.1f );
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex3( 0.0f, 1.0f, 0.1f );
		}
		else
		{
			GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex3( 0.0f, 0.0f, 0.1f );
			GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex3( 1.0f, 0.0f, 0.1f );
			GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex3( 1.0f, 1.0f, 0.1f );
			GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex3( 0.0f, 1.0f, 0.1f );
		}
		
		GL.End();
	}
	

	
	// LatencyTest
	void LatencyTest(RenderTexture dest)
	{
		byte r = 0,g = 0, b = 0;
		
		// See if we get a string back to send to the debug out
		string s = Marshal.PtrToStringAnsi(OVRDevice.GetLatencyResultsString());
		if (s != null)
		{
			string result = 
			"\n\n---------------------\nLATENCY TEST RESULTS:\n---------------------\n";
			result += s;
			result += "\n\n\n";
			print(result);
		}
		
		if(OVRDevice.DisplayLatencyScreenColor(ref r, ref g, ref b) == false)
			return;
		
		RenderTexture.active = dest;  		
		Material material = ColorOnlyMaterial;
		QuadColor.r = (float)r / 255.0f;
		QuadColor.g = (float)g / 255.0f;
		QuadColor.b = (float)b / 255.0f;
		material.SetColor("_Color", QuadColor);
		GL.PushMatrix();
    	material.SetPass(0);
    	GL.LoadOrtho();
    	GL.Begin(GL.QUADS);
    	GL.Vertex3(0.3f,0.3f,0);
    	GL.Vertex3(0.3f,0.7f,0);
    	GL.Vertex3(0.7f,0.7f,0);
    	GL.Vertex3(0.7f,0.3f,0);
    	GL.End();
    	GL.PopMatrix();
		
	}


	///////////////////////////////////////////////////////////
	// PUBLIC FUNCTIONS
	///////////////////////////////////////////////////////////
		
	// SetPerspectiveOffset
	public void SetPerspectiveOffset(ref Vector3 offset)
	{
		// NOTE: Unity skyboxes do not currently use the projection matrix, so
		// if one wants to use a skybox with the Rift it must be implemented 
		// manually		
		gameObject.camera.ResetProjectionMatrix();
    	Matrix4x4 tm = Matrix4x4.identity;
    	tm.SetColumn (3, new Vector4 (offset.x, offset.y, 0.0f, 1));
    	gameObject.camera.projectionMatrix = tm * gameObject.camera.projectionMatrix;
	}
	#endregion
	
}
