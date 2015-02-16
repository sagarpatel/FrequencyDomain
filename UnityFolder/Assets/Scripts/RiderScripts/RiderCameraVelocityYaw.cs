using UnityEngine;
using System.Collections;

public class RiderCameraVelocityYaw : MonoBehaviour 
{

	RiderPhysics riderPhysics;
	float maxYawAngle = 10.0f;
	public AnimationCurve velocityToYawStepCurve;
	
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
			step = velocityToYawStepCurve.Evaluate(relOnLine);
			rotationVec.y = Mathf.Lerp(0, maxYawAngle, step );
		}
		else
		{
			step = velocityToYawStepCurve.Evaluate(-relOnLine);
			rotationVec.y = Mathf.Lerp(0, -maxYawAngle, step );
		}
		
		
		transform.localEulerAngles = rotationVec;
	}
}
