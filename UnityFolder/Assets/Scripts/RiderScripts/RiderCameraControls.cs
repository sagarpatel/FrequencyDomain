using UnityEngine;
using System.Collections;
using InControl;

public class RiderCameraControls : MonoBehaviour 
{
	public AnimationCurve controlCurve;

	float pitchRange = 25.0f;
	float yawRange = 15.0f;

	void Update()
	{

		if(InputManager.Devices.Count > 1)
		{
			var inputDeviceRider = InputManager.Devices[0];

			float rawPitch = inputDeviceRider.RightStickY;
			float rawYaw = inputDeviceRider.RightStickX;

			float pitchStep = controlCurve.Evaluate( Mathf.Abs( rawPitch ) );
			float yawStep =  controlCurve.Evaluate( Mathf.Abs( rawYaw ) );

			float pitchAngle = Mathf.Sign(rawPitch) * Mathf.LerpAngle(0, pitchRange, pitchStep);
			float yawAngle = Mathf.Sign(rawYaw) * Mathf.LerpAngle(0,  yawRange, yawStep);

			transform.localRotation = Quaternion.Euler( new Vector3(pitchAngle, yawAngle, 0) );

		}


	}



}
