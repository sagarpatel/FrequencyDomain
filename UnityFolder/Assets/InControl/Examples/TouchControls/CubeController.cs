using System;
using UnityEngine;
using InControl;


namespace TouchExample
{
	public class CubeController : MonoBehaviour
	{
		void Update()
		{
			// Use last device which provided input.
			var inputDevice = InputManager.ActiveDevice;

			// Set target object material color based on which action is pressed.
			renderer.material.color = GetColorFromActionButtons( inputDevice );

			// Rotate target object.
			transform.Rotate( Vector3.down,  500.0f * Time.deltaTime * inputDevice.Direction.X, Space.World );
			transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * inputDevice.Direction.Y, Space.World );
		}


		Color GetColorFromActionButtons( InputDevice inputDevice )
		{
			if (inputDevice.Action1)
			{
				return Color.green;
			}

			if (inputDevice.Action2)
			{
				return Color.red;
			}

			if (inputDevice.Action3)
			{
				return Color.blue;
			}

			if (inputDevice.Action4)
			{
				return Color.yellow;
			}

			return Color.white;
		}
	}
}

