using System;


namespace InControl
{
	// Tested with ADT-1
	// Profile by Artūras 'arturaz' Šlajus <arturas@tinylabproductions.com>
	//
	// @cond nodoc
	[AutoDiscover]
	public class AndroidTVRemoteProfile : UnityInputDeviceProfile
	{
		public AndroidTVRemoteProfile()
		{
			Name = "Android TV Remote";
			Meta = "Android TV Remotet on Android TV";

			SupportedPlatforms = new[] {
				"Android"
			};

			JoystickNames = new[] {
				"touch-input",
				"navigation-input"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlType.Action1,
					Source = Button0
				}
			};

			AnalogMappings = new[] {
				DPadLeftMapping( Analog4 ),
				DPadRightMapping( Analog4 ),
				DPadUpMapping( Analog5 ),
				DPadDownMapping( Analog5 ),
			};
		}
	}
	// @endcond
}
