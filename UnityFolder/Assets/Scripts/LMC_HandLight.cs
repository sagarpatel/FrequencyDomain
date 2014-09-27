using UnityEngine;
using System.Collections;
using Leap;


public class LMC_HandLight : MonoBehaviour 
{
	Light lmcLight;
	Transform lightTransform;

	Controller lmcController;

	LMC_FingertipsStitch lmcFingertipsStitch;

	void Start () 
	{
		lmcLight = GetComponent<Light>();
		lightTransform = lmcLight.transform;
		lmcFingertipsStitch = GameObject.FindObjectOfType<LMC_FingertipsStitch>().GetComponent<LMC_FingertipsStitch>();

		lmcController = new Controller();
		if (lmcController == null)
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");

		
	}


	void Update () 
	{
		if (lmcController == null)
			return;

		Frame frame = lmcController.Frame();
		HandList hands = frame.Hands;

		if(hands.Count == 0)
			return;

		Vector3 lightPos = lmcFingertipsStitch.jointsAveragePos;
		lightTransform.position = lightPos;

	}

}
