using UnityEngine;
using System.Collections;
using InControl;
using Leap;


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

	Controller lmcController;

	void Start () 
	{
		pva = GetComponent<PVA>();

		lmcController = new Controller();

		if (lmcController == null) 
		{
      		Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");
    	}
	}
	

	void Update () 
	{
		rotDelta = Vector3.zero;

		// -------- FORWARD ACCELERATION --------------

		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;		
		float zAcc = inputDevice.Action1 * forwardControlScale ;//* Time.deltaTime;

		controlDelta = transform.forward * zAcc;

		if(controlDelta.magnitude > 0)
			pva.isLinearDecay = false;
		else
			pva.isLinearDecay = true;

		pva.acceleration = controlDelta;


		//  --------  ROTATIONAL ACCELERATION --------------

		float triggerLeft = inputDevice.LeftTrigger * rotationControlScale;
		float triggerRight = -inputDevice.RightTrigger * rotationControlScale;

		
		if(Input.GetKey(KeyCode.Q))
			triggerLeft += 1.0f * rotationControlScale;

		if(Input.GetKey(KeyCode.E))
			triggerRight += 1.0f * -rotationControlScale;


		float xAcc = inputDevice.LeftStickX * hControlScale;
		float yAcc = inputDevice.LeftStickY * vControlScale;

		// --------- LEAP PART ------------
		if(lmcController != null)
		{
			Frame frame = lmcController.Frame();
			HandList hands = frame.Hands;
			Hand firstHand = hands[0];

			if(firstHand.IsValid)
			{
				float yaw = firstHand.Direction.Yaw * 4.0f;
				float pitch = -firstHand.Direction.Pitch * 4.0f;
				float roll = firstHand.PalmNormal.Roll * 0.20f;

				xAcc += yaw;
				yAcc += pitch;
				rotDelta.z  += roll;
			}
		}

		// ------------ Turn boost adjustments ---------------
		
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
		rotDelta.z += triggerLeft + triggerRight;

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
