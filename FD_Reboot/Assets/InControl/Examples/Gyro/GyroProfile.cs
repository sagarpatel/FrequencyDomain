using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace GyroExample
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager script, which is attached to the InControl
	// game object in this example scene.
	//
	public class GyroProfile : UnityInputDeviceProfile
	{
		public GyroProfile()
		{
			Name = "Gyroscope";
			Meta = "Gyroscope on iOS.";

			// This profile only works on mobile.
			SupportedPlatforms = new[] {
				"iPhone",
				"Android"
			};

			AnalogMappings = new[] {
				new InputControlMapping {
					Handle = "Move Left",
					Target = InputControlType.LeftStickLeft,
					Source = new UnityGyroAxisSource( UnityGyroAxisSource.GyroAxis.X ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
					Raw = true,
					Scale = 3.0f
				},
				new InputControlMapping {
					Handle = "Move Right",
					Target = InputControlType.LeftStickRight,
					Source = new UnityGyroAxisSource( UnityGyroAxisSource.GyroAxis.X ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
					Raw = true,
					Scale = 3.0f
				},
				new InputControlMapping {
					Handle = "Move Up",
					Target = InputControlType.LeftStickUp,
					Source = new UnityGyroAxisSource( UnityGyroAxisSource.GyroAxis.X ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
					Raw = true,
					Scale = 3.0f
				},
				new InputControlMapping {
					Handle = "Move Down",
					Target = InputControlType.LeftStickDown,
					Source = new UnityGyroAxisSource( UnityGyroAxisSource.GyroAxis.X ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
					Raw = true,
					Scale = 3.0f
				},
			};
		}
	}
}

