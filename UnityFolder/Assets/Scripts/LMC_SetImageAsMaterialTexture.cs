using UnityEngine;
using System.Collections;
using Leap;


public class LMC_SetImageAsMaterialTexture : MonoBehaviour 
{
	// experimenting with --> https://developer.leapmotion.com/documentation/skeletal/csharp/devguide/Leap_Images.html
	Controller lmcController;
	Material targetMaterial;

	void Start () 
	{
		lmcController = new Controller();
		if (lmcController == null)
		{
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");
		}
		//lmcController.SetPolicyFlags(Controller.PolicyFlag.POLICY_IMAGES);

		targetMaterial = GetComponent<MeshRenderer>().material;
	
	}
	
	void Update () 
	{
		Frame frame = lmcController.Frame();
	

	}
}
