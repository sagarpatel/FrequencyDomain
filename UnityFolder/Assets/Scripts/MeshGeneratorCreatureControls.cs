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
	

	void Start () 
	{
		pva = GetComponent<PVA>();
	}
	

	void Update () 
	{

		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;

		
		float zAcc = inputDevice.Action1 * forwardControlScale ;//* Time.deltaTime;


		//controlDelta.x = xAcc;
		//controlDelta.y = yAcc;
		//controlDelta.z = zAcc;



		controlDelta = transform.forward * zAcc;

		if(controlDelta.magnitude > 0)
			pva.isLinearDecay = false;
		else
			pva.isLinearDecay = true;

		pva.acceleration = controlDelta;


		// triggers / tilt/ rotation

		float triggerLeft = inputDevice.LeftTrigger * rotationControlScale;// * Time.deltaTime;
		float triggerRight = -inputDevice.RightTrigger * rotationControlScale;// * Time.deltaTime;

		float xAcc = inputDevice.LeftStickX * hControlScale ;//* Time.deltaTime;
		float yAcc = inputDevice.LeftStickY * vControlScale ;//* Time.deltaTime;

		rotDelta.x = xAcc;
		rotDelta.y = yAcc;
		rotDelta.z = triggerLeft + triggerRight;

		if(rotDelta.magnitude > 0)
			pva.isAngularDecay = false;
		else
			pva.isAngularDecay = true;

		pva.rotationalAcceleration = rotDelta;

		//transform.rotation =  Quaternion.Slerp(transform.rotation, Quaternion.identity, inputDevice.Action2 * lerpToIdentityScale * Time.deltaTime);
		//pva.rotationalVelocity = Vector3.Lerp(pva.rotationalVelocity, Vector3.zero, inputDevice.Action3 * lerpToIdentityScale * Time.deltaTime);

	}
}
