/************************************************************************************

Filename    :   OVRUtils.cs
Content     :   Base component OVR class
Created     :  	May 13, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using System.Collections.Generic;


//-------------------------------------------------------------------------------------
// ***** OVRUtils
//
// OVRUtils holds many static utility functions that can be used by any script.
//

public class OVRUtils
{
	// SetLocalTransformIdentity
	public static void SetLocalTransformIdentity(ref GameObject gameObject)
	{
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale    = Vector3.one;
	}
	
	// SetLocalTransformIdentity	
	public static void SetLocalTransform(ref GameObject gameObject, ref Transform xfrm)
	{
		gameObject.transform.localPosition = xfrm.position;
		gameObject.transform.localRotation = xfrm.rotation;
		gameObject.transform.localScale    = xfrm.localScale;
	}
}


