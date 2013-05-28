/************************************************************************************

Filename    :   OVRGUI.cs
Content     :   OVR GUI helper classclass
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
// ***** OVRGUI
//
// OVRGUI is a GUI help class that provides functions for drawing text and other elements
// to the screen
public class OVRGUI
{
	public  bool  Draw3D        = false;
	private float StereoSpreadX = -40.0f;
	private Font  FontReplace   = null;
	private float PixelWidth    = 1280.0f;
	private float PixelHeight   = 800.0f;
	
	
	// Get/SetSeteroSpreadX
	public void GetStereoSpreadX(ref float stereoSpreadX)
	{
		stereoSpreadX = StereoSpreadX;
	}
	public void SetStereoSpreadX(float stereoSpreadX)
	{
		StereoSpreadX = stereoSpreadX;
	}
	
	// Get/SetFontReplace
	public void GetFontReplace(ref Font fontReplace)
	{
		fontReplace = FontReplace;
	}
	public void SetFontReplace(Font fontReplace)
	{
		FontReplace = fontReplace;
	}
	
	// StereoBox - Values based on pixels in DK1 resolution of W: (1280 / 2) H: 800
	// TODO: Create overloaded function to take normalized float values from 0 - 1 on screen
	public void StereoBox(int X, int Y, int wX, int wY, ref string text, Color color)
	{
		Font prevFont = GUI.skin.font;
	
		if(Draw3D == true)
		{
			GUI.contentColor = color;
			
			if(GUI.skin.font != FontReplace)
				GUI.skin.font = FontReplace;
		
			float sSX = (float)Screen.width / PixelWidth;	
			float sSY = (float)Screen.height / PixelHeight;
			
			int x  = (int)((float) X * sSX * 1.75f);
			int wx = (int)((float)wX * sSY * 1.0f);
			
			GUI.Box(new Rect(x, Y, wx, wY), text);
		}
		else
		{
			// Deprecate this part of code; we will want to do everything in 3D space
			// on the RIFT (especially when HD versions of the Rift are available)
			float ploLeft = 0, ploRight = 0;
			float sSX = (float)Screen.width / PixelWidth;	
			float sSY = (float)Screen.height / PixelHeight;
			
			OVRDevice.GetPhysicalLensOffsets(ref ploLeft, ref ploRight); 
			
			int xL = (int)((float)X * sSX);
			int sSpreadX = (int)(StereoSpreadX * sSX);
			int xR = (Screen.width / 2) + xL + sSpreadX - 
			      // required to adjust for physical lens shift
			  	    (int)(ploLeft * (float)Screen.width / 2);
			int y = (int)((float)Y * sSY);
		
			GUI.contentColor = color;
		
			int sWX = (int)((float)wX * sSX);
			int sWY = (int)((float)wY * sSY);
		
			if(FontReplace != null)
				GUI.skin.font = FontReplace;
		
			GUI.Box(new Rect(xL, y, sWX, sWY), text);
			GUI.Box(new Rect(xR, y, sWX, sWY), text);	
		}
		
		GUI.skin.font = prevFont;
	}
}


