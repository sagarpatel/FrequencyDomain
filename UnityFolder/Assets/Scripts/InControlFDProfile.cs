using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{

	public class InControlFDProfile : UnityInputDeviceProfile 
	{
		public InControlFDProfile()
		{
			Name = "FD Keyboard ";
			Meta = "Keyboard Control Mappings for FD";

			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move Forward - Mouse",
					Target = InputControlType.Action1,
					Source = MouseButton0
				},
				
				new InputControlMapping
				{
					Handle = "Move Forward - Keyboard",
					Target = InputControlType.Action1,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Toggle Amplitude Scaling",
					Target = InputControlType.Action2,
					Source = KeyCodeButton( KeyCode.U )
				},
				new InputControlMapping
				{
					Handle = "Toggle Audio Source",
					Target = InputControlType.Action4,
					Source = KeyCodeButton( KeyCode.T )
				},
				new InputControlMapping
				{
					Handle = "Toggle OVR",
					Target = InputControlType.LeftBumper,
					Source = KeyCodeButton( KeyCode.O )
				},
				
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move X",
					Target = InputControlType.LeftStickX,
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
				},
				new InputControlMapping
				{
					Handle = "Move Y",
					Target = InputControlType.LeftStickY,
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
				},
				new InputControlMapping
				{
					Handle = "Look X",
					Target = InputControlType.RightStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping
				{
					Handle = "Look Y",
					Target = InputControlType.RightStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				},
				/*
				new InputControlMapping
				{
					Handle = "Roll Left",
					Target = InputControlType.LeftTrigger,
					Source = KeyCodeAxis( KeyCode.None, KeyCode.Q )
				},*/	
				/*
				new InputControlMapping
				{
					Handle = "Roll ",
					Target = InputControlType.RightTrigger,
					Source = KeyCodeAxis( KeyCode.Q, KeyCode.E )
				}
				*/
				

			};
		}
		
	}

}