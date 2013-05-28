/************************************************************************************

Filename    :   OVRMagCalibration.cs
Content     :   Magnetometer calibration helper class
Created     :   May 1, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using System.Collections.Generic;


//-------------------------------------------------------------------------------------
// ***** OVRMagCalibration
//
// OVRMagCalibration is a helper class that allows for calibrating the magnetometer to be
// used for Yaw-drift corection. It can be used in either manual or auto mode.
// Manual mode will prompt the user to look in certain directions to calibrate the mag.
// Auto mode will let the user move the rift around and find arbitraty points for calibration
// to take place.
//
// When calibration is done, the user must set an orientation that will be used for yaw correction.
// 
public class OVRMagCalibration
{
	public enum MagCalibrationState { MagDisabled, 
									  MagManualGetReady, 
									  MagCalibrating, 
									  MagCalibrated, 
									  MagReady };

	public bool 	MagAutoCalibrate    = false; 	// Manual mode by default
	private 		MagCalibrationState	MagCalState = MagCalibrationState.MagDisabled;
	private float 	MagCalTimerFlash    = 0.5f;
	
	private Vector3 CurEulerRef = Vector3.zero;	
	
	private bool  	MagShowGeometry     = false;
	
	public OVRCameraController CameraController = null;
	public GameObject GeometryReference 		= null;
	public GameObject GeometryCompass  			= null;
	public Material GeometryReferenceMarkMat    = null;
	
	// * * * * * * * * * * * * *
	
	// Disabled
	public bool Disabled()
	{
		if(MagCalState == MagCalibrationState.MagDisabled)
			return true;
		
		return false;
	}
	
	// SetOVRCameraController
	public void SetOVRCameraController(ref OVRCameraController cameraController)
	{
		CameraController = cameraController;
	}
	
	// ShowGeometry
	public void ShowGeometry(bool show)
	{
		// Load up the prefab
		if(GeometryReference == null)
		{
			GeometryReference =
			GameObject.Instantiate(Resources.Load("OVRMagReference")) as GameObject;
			GeometryReferenceMarkMat = GeometryReference.transform.Find ("Mark").renderer.material;
		}
		
		if(GeometryReference != null)
		{
			GeometryReference.SetActive(show);		
			AttachGeometryToCamera(show, ref GeometryReference);
			OVRUtils.SetLocalTransformIdentity(ref GeometryReference);
		}
		
		// Load up the prefab
		if(GeometryCompass == null)
		{
			GeometryCompass =
			GameObject.Instantiate(Resources.Load("OVRMagCompass")) as GameObject;
		}
		
		if(GeometryCompass != null)
		{
			GeometryCompass.SetActive(show);
			AttachGeometryToCamera(show, ref GeometryCompass);
  			OVRUtils.SetLocalTransformIdentity(ref GeometryCompass);
		}
	}
	
	// AttachGeometryToRightCamera
	public void AttachGeometryToCamera(bool attach, ref GameObject go)
	{
		if(CameraController != null)
		{
			if(attach == true)
				CameraController.AttachGameObjectToCamera(ref go);
		}
	}	
	
	// UpdateGeometry
	public void UpdateGeometry()
	{
		if(MagShowGeometry == false)
			return;		
		if(CameraController == null)
			return;
		if((GeometryReference == null) || (GeometryCompass == null))
			return;
		
		// All set, we can update the geometry with camera and positon values
		Quaternion q = Quaternion.identity;
		if((CameraController != null) && (CameraController.PredictionOn == true))
			OVRDevice.GetPredictedOrientation(0, ref q);
		else
			OVRDevice.GetOrientation(0, ref q);
			
		Vector3 v = GeometryCompass.transform.localEulerAngles;
		v.y = -q.eulerAngles.y + CurEulerRef.y;
		GeometryCompass.transform.localEulerAngles = v;
		
		// Set the color of the marker to red if we are calibrating
		if(GeometryReferenceMarkMat != null)
		{
			Color c = Color.green;
			
			if(OVRDevice.IsMagYawCorrectionInProgress(0) == true)
				c = Color.red;
			
			GeometryReferenceMarkMat.SetColor("_Color", c);	
		}
	}
	
	// UpdateMagYawDriftCorrection
	public void UpdateMagYawDriftCorrection()
	{
		if(Input.GetKeyDown (KeyCode.Z) == true) 
		{
			if(MagCalState == MagCalibrationState.MagDisabled)
			{
				// Start calibration process
				if(MagAutoCalibrate == true)
				{	
					OVRDevice.BeginMagAutoCalibration(0);
					MagCalState = MagCalibrationState.MagCalibrating;
				}
				else
				{
					// Go to pre-manual calibration state (to allow for
					// setting refrence point)
					MagCalState = MagCalibrationState.MagManualGetReady;
					return;
				}
			}
			else if(MagCalState == MagCalibrationState.MagManualGetReady)
			{
				OVRDevice.SetMagReference(0);
				OVRDevice.EnableMagYawCorrection(0,true);
				
				Quaternion q = Quaternion.identity;
				if((CameraController != null) && (CameraController.PredictionOn == true))
					OVRDevice.GetPredictedOrientation(0, ref q);
				else
					OVRDevice.GetOrientation(0, ref q);

				CurEulerRef = q.eulerAngles;
				
				// Begin manual calibration
				OVRDevice.BeginMagManualCalibration(0);
				MagCalState = MagCalibrationState.MagCalibrating;
			}
			else
			{
				// Reset calibration process
				if(MagAutoCalibrate == true)
					OVRDevice.StopMagAutoCalibration(0);
				else
					OVRDevice.StopMagManualCalibration(0);
					
				OVRDevice.EnableMagYawCorrection(0,false);
				
				MagCalState = MagCalibrationState.MagDisabled;
				
				// Do not show geometry
				MagShowGeometry = false;
				ShowGeometry(MagShowGeometry);
				
				return;
			}
		}		
		
		
		// Check to see if calibration is completed
		if(MagCalState == MagCalibrationState.MagCalibrating)
		{
			if(MagAutoCalibrate == true)
				OVRDevice.UpdateMagAutoCalibration(0);
			else
				OVRDevice.UpdateMagManualCalibration(0);				
			
			if(OVRDevice.IsMagCalibrated(0) == true)
			{
				if(MagAutoCalibrate == true)
					MagCalState = MagCalibrationState.MagCalibrated;
				else
					// Manual Calibration take account of having set the
					// reference orientation.
					MagCalState = MagCalibrationState.MagReady;

			}
		}
		
		// If we are calibrated, we will set mag reference and 
		// enable yaw correction on a buton press
		if( (MagCalState == MagCalibrationState.MagCalibrated) || 
		    (MagCalState == MagCalibrationState.MagReady) )
		{
			if(Input.GetKeyDown (KeyCode.X) == true)
			{
				OVRDevice.SetMagReference(0);
				OVRDevice.EnableMagYawCorrection(0,true);
				MagCalState = MagCalibrationState.MagReady;
				
				Quaternion q = Quaternion.identity;
				if((CameraController != null) && (CameraController.PredictionOn == true))
					OVRDevice.GetPredictedOrientation(0, ref q);
					else
				OVRDevice.GetOrientation(0, ref q);

				CurEulerRef = q.eulerAngles;
			}				
				
			if((MagCalState == MagCalibrationState.MagReady) && 
			   (Input.GetKeyDown (KeyCode.F6)))
			{
			
				// Toggle showing geometry either on or off	
				if(MagShowGeometry == false)
				{
					MagShowGeometry = true;
					ShowGeometry(MagShowGeometry);
				}
				else
				{
					MagShowGeometry = false;
					ShowGeometry(MagShowGeometry);
				}
			}
			
			UpdateGeometry();
		}
	}
	
	// GUIMagYawDriftCorrection
	public void GUIMagYawDriftCorrection(int xLoc, int yLoc, 
										 int xWidth, int yWidth, 
										 ref OVRGUI guiHelper)
	{
		string strMagCal = "";
		Color c = Color.red;
		int xloc = xLoc;
		int xwidth = xWidth;
		
		switch(MagCalState)
		{
		case(MagCalibrationState.MagDisabled):
			strMagCal = "Mag Calibration OFF";
			break;
		
		case(MagCalibrationState.MagManualGetReady):
			strMagCal = "Manual Calibration: Look Forward, Press 'Z'..";
			c = Color.white;
			xloc -= 40;
			xwidth += 150;
			break;
		
		case(MagCalibrationState.MagCalibrating):
			if(MagCalTimerFlash > 0.2f)
				FormatCalibratingString(ref strMagCal);

			MagCalTimerFlash -= Time.deltaTime;
			if(MagCalTimerFlash < 0.0f)
				MagCalTimerFlash += 0.5f;
			
			c = Color.white;
			xloc -= 40;
			xwidth += 150;
			break;
		
		case(MagCalibrationState.MagCalibrated):
			strMagCal = "Mag Calibrated";	
			MagCalTimerFlash -= Time.deltaTime;
			if(MagCalTimerFlash < 0.0f)
				MagCalTimerFlash += 0.5f;
			
			c = Color.yellow;
			break;
		
		case(MagCalibrationState.MagReady):
			if(OVRDevice.IsMagYawCorrectionInProgress(0) == true)
			{
				if(MagCalTimerFlash > 0.2f)
				{
					strMagCal = "Mag CORRECTING...";
					strMagCal = System.String.Format ("Mag CORRECTING (deg)... {0:F3}", 
						OVRDevice.GetYawErrorAngle(0));
				}
				
				MagCalTimerFlash -= Time.deltaTime;
				if(MagCalTimerFlash < 0.0f)
					MagCalTimerFlash += 0.5f;
				
				xloc -= 40;
			    xwidth += 150;
	
				c = Color.red;
			}
			else
			{
				strMagCal = "Mag Correction ON";
				c = Color.green;
			}
	
			break;			
		}
				
		guiHelper.StereoBox (xloc, yLoc, xwidth, yWidth, ref strMagCal, c);		
	}
	
	// FormatCalibratingString
	void FormatCalibratingString(ref string str)
	{
		if(MagAutoCalibrate == true)
		{
			str = System.String.Format ("Mag Calibrating (AUTO)... Point {0} set", 
						OVRDevice.MagNumberOfSamples(0));
		}
		else
		{
			// Manual Calibration: Make sure to get proper direction
			str = "Mag Calibrating (MANUAL)... LOOK ";
			
			switch(OVRDevice.MagNumberOfSamples(0))
			{
				case(0): str += "FORWARD"; break;
				case(1): str += "UP"; break;
				case(2): str += "LEFT"; break;
				case(3): str += "RIGHT"; break;
			}
		}
	}
}


