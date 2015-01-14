using UnityEngine;
using System.Collections;

public class RiderCameraVelocityRoll : MonoBehaviour 
{
	RiderPhysics riderPhysics;
	float maxRollAngle = 30.0f;

	void Start()
	{
		riderPhysics = FindObjectOfType<RiderPhysics>(); // TOOD: fix this, only works now because there only 1 rider
	}

	void Update()
	{

		Vector3 rotationVec = new Vector3(0,0,0);
		float step = 0.5f * (1.0f + riderPhysics.GetSideMoveProgressRatio()); // normalize to 0 to 1 range
		Debug.Log(step);
		rotationVec.z =  Mathf.Lerp(maxRollAngle, -maxRollAngle, step);
		transform.localEulerAngles = rotationVec;
	}



}
