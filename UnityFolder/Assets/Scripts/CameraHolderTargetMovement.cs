using UnityEngine;
using System.Collections;

public class CameraHolderTargetMovement : MonoBehaviour 
{
	Vector3 localPosMin = new Vector3(0,20,-100);
	Vector3 localPosMax = new Vector3(0, 40, -500);

	PVA creaturePVA;

	public bool isOverwritten = false; // used to enable LMC hand depth to take over camera target pos setting

	void Start () 
	{
		creaturePVA = transform.parent.GetComponent<PVA>();
	}
	
	void Update () 
	{

		if (isOverwritten == true)
			return;

		float velRatio = creaturePVA.velocity.magnitude / creaturePVA.maxVelocityMagnitude;
		float angVelRatio = creaturePVA.rotationalVelocity.magnitude / creaturePVA.maxRotationalVelocityMagnitude;

		float step = 0.5f * (velRatio + angVelRatio);

		MoveCameraTargetPosition(localPosMin, localPosMax, step);
	}

	// deprecated
	public void MoveCameraTargetPosition(Vector3 localMin, Vector3 localMax, float lerpStep)
	{
		transform.localPosition = Vector3.Lerp(localMin, localMax, lerpStep);
	}

	public void SetNewLocalPos(Vector3 localPos)
	{
		transform.localPosition = localPos;
	}

}
