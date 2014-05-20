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
			pva.isDecay = false;
		else
			pva.isDecay = true;

		pva.acceleration = controlDelta;


		// triggers / tilt/ rotation

		float triggerLeft = inputDevice.LeftTrigger * rotationControlScale;// * Time.deltaTime;
		float triggerRight = -inputDevice.RightTrigger * rotationControlScale;// * Time.deltaTime;

		float xAcc = inputDevice.Direction.x * hControlScale ;//* Time.deltaTime;
		float yAcc = inputDevice.Direction.y * vControlScale ;//* Time.deltaTime;

		rotDelta.x = xAcc;
		rotDelta.y = yAcc;
		rotDelta.z = triggerLeft + triggerRight;

		pva.rotationalAcceleration = rotDelta;

	}
}
