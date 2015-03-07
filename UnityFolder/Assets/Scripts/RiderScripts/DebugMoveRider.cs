﻿using UnityEngine;
using System.Collections;
using InControl;

public class DebugMoveRider : MonoBehaviour 
{
	float forwardSpeedScale = 500.0f;
	float reverseSpeedScale = 400.0f;

	float sideSpeedScale = 100.0f;

	public Transform meshHeadObject;
	RiderPhysics riderPhysics;

	void Start()
	{
		riderPhysics = GetComponent<RiderPhysics>();
	}

	void Update()
	{
		Vector3 before = transform.position;

		if(Input.GetKey(KeyCode.Y) == true)
		{
			riderPhysics.MoveForward(1.0f);
		}
		if(Input.GetKey(KeyCode.H) == true)
		{
			riderPhysics.MoveForward(-1.0f);
		}

		if(Input.GetKey(KeyCode.J) == true)
		{
			riderPhysics.MoveSideways(1.0f);
		}

		if(Input.GetKey(KeyCode.G) == true)
		{
			riderPhysics.MoveSideways(-1.0f);
		}

		Vector3 after = transform.position;



		if(InputManager.Devices.Count > 1)
		{
			var inputDeviceRider = InputManager.Devices[0];
			
			if(inputDeviceRider == null)
			{
				Debug.Log("No player 1! controller");
			}
			else
			{
				riderPhysics.MoveForward(  inputDeviceRider.LeftStickY );
				riderPhysics.MoveSideways(  inputDeviceRider.LeftStickX );
			}
		}


	}

}
