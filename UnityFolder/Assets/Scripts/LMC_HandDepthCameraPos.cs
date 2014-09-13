using UnityEngine;
using System.Collections;
using Leap;

public class LMC_HandDepthCameraPos : MonoBehaviour 
{
	Controller lmcController;

	CameraHolderTargetMovement cameraHolderTagetMovement;
	Vector3 localPosMin = new Vector3(0, 5, -20);
	Vector3 localPosMax = new Vector3(0, 100, -1000);

	void Start () 
	{
		lmcController = new Controller();
		if (lmcController == null)
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");

		cameraHolderTagetMovement = GetComponent<CameraHolderTargetMovement>();
	}
	
	void Update () 
	{
		cameraHolderTagetMovement.isOverwritten = false;
		if (lmcController == null)
			return;

		cameraHolderTagetMovement.isOverwritten = true; // enforce lerp camera pos

		Frame frame = lmcController.Frame();
		HandList hands = frame.Hands;
		Hand firstHand = hands[0];

		Vector3 relativePalmPos = firstHand.PalmPosition.ToUnityScaled();
		float handDepth = Mathf.Clamp( -relativePalmPos.z, -0.3f, 0.3f); // seems to range between -0.3 and 0.3
		float normalizedStep = 0.5f + (0.5f * ( handDepth * 1.0f / 0.3f )); // this should range between 0 and 1

		cameraHolderTagetMovement.MoveCameraTargetPosition(localPosMin, localPosMax, normalizedStep);
	}
}
