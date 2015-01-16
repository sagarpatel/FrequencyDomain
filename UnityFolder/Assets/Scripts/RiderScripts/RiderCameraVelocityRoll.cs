using UnityEngine;
using System.Collections;

public class RiderCameraVelocityRoll : MonoBehaviour 
{
	RiderPhysics riderPhysics;
	float maxRollAngle = 21.0f;
	public AnimationCurve velocityToRollStepCurve;

	void Start()
	{
		riderPhysics = FindObjectOfType<RiderPhysics>(); // TOOD: fix this, only works now because there only 1 rider
	}

	void Update()
	{

		Vector3 rotationVec = new Vector3(0,0,0);
		//float step = 0.5f * (1.0f + riderPhysics.GetSideMoveProgressRatio()); // normalize to 0 to 1 range
		float step;
		float relOnLine = riderPhysics.GetSideMoveProgressRatio();

		if(relOnLine > 0)
		{
			step = velocityToRollStepCurve.Evaluate(relOnLine);
			rotationVec.z = Mathf.Lerp(0, -maxRollAngle, step );
		}
		else
		{
			step = velocityToRollStepCurve.Evaluate(-relOnLine);
			rotationVec.z = Mathf.Lerp(0, maxRollAngle, step );
		}


		transform.localEulerAngles = rotationVec;
	}



}
