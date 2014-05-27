using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(PVA))]
public class MeshGeneratorCreatureControls : MonoBehaviour 
{
	PVA pva;

	public float hControlScale = 10.0f;
	public float vControlScale = 10.0f;
	public float forwardControlScale = 10.0f;

	public float rotationControlScale = 10.0f;

	//read only
	public Vector3 controlDelta;

	public Vector3 rotDelta;

	public float lerpToIdentityScale = 10.0f;
	
	bool turnBoost_x = false;
	bool turnBoost_y = false;


	void Start () 
	{
		pva = GetComponent<PVA>();
	}
	

	void Update () 
	{

		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;		
		float zAcc = inputDevice.Action1 * forwardControlScale ;//* Time.deltaTime;

		controlDelta = transform.forward * zAcc;

		if(controlDelta.magnitude > 0)
			pva.isLinearDecay = false;
		else
			pva.isLinearDecay = true;

		pva.acceleration = controlDelta;


		//  --------  triggers / tilt/ rotation --------------

		float triggerLeft = inputDevice.LeftTrigger * rotationControlScale;
		float triggerRight = -inputDevice.RightTrigger * rotationControlScale;
		float xAcc = inputDevice.LeftStickX * hControlScale;
		float yAcc = inputDevice.LeftStickY * vControlScale;

		
		if(xAcc > 0 && pva.rotationalVelocity.x < 0)
			turnBoost_x = true;
		if(xAcc < 0 && pva.rotationalVelocity.x > 0)
			turnBoost_x = true;

		if(yAcc > 0 && pva.rotationalVelocity.y < 0)
			turnBoost_y = true;
		if(yAcc < 0 && pva.rotationalVelocity.y > 0)
			turnBoost_y = true;

		if(xAcc == 0)
			turnBoost_x = false;
		if(yAcc == 0)
			turnBoost_y = false;

		if(turnBoost_x)
			xAcc *= 4.0f;
		if(turnBoost_y)
			yAcc *= 4.0f;

		rotDelta.x = xAcc;
		rotDelta.y = yAcc;
		rotDelta.z = triggerLeft + triggerRight;

		if(rotDelta.magnitude > 0)
			pva.isAngularDecay = false;
		else
			pva.isAngularDecay = true;

		pva.rotationalAcceleration = rotDelta;


		// force velocity orientation
		Vector3 correctedVel = pva.velocity.magnitude * transform.forward;
		pva.velocity = correctedVel;

	}

	/*

	void OnDrawGizmos()
	{
		  Gizmos.DrawRay(transform.position, transform.forward * 5.0f);
		  Gizmos.color = Color.green;
		  Gizmos.DrawRay(transform.position, pva.velocity.normalized * 5.0f);
		  //Gizmos.DrawRay(transform.position, transform.TransformDirection( pva.velocity.normalized ) * 5.0f);
	}

	*/
}
