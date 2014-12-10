using UnityEngine;
using System.Collections;

public class CameraHolderTargetMovement : MonoBehaviour 
{
	Vector3 localPosMin = new Vector3(0,20,-100);
	Vector3 localPosMax = new Vector3(0, 40, -300);

	PVA creaturePVA;

	void Start () 
	{
		creaturePVA = transform.parent.GetComponent<PVA>();
	}
	
	void Update () 
	{
		float velRatio = creaturePVA.velocity.magnitude / creaturePVA.maxVelocityMagnitude;
		float angVelRatio = creaturePVA.rotationalVelocity.magnitude / creaturePVA.maxRotationalVelocityMagnitude;

		float step = 0.5f * (velRatio + angVelRatio);
		
		transform.localPosition = Vector3.Lerp(localPosMin, localPosMax, step);
	}
}
