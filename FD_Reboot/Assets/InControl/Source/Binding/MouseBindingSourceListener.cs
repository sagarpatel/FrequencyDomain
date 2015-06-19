using System;
using UnityEngine;


namespace InControl
{
	public class MouseBindingSourceListener : BindingSourceListener
	{
		Mouse detectFound;
		int detectPhase;


		public void Reset()
		{
			detectFound = Mouse.None;
			detectPhase = 0; // Wait for release.
		}


		public BindingSource Listen( BindingListenOptions listenOptions, InputDevice device )
		{
			if (!listenOptions.IncludeMouseButtons)
			{
				return null;
			}

			if (detectFound != Mouse.None)
			{
				if (!IsPressed( detectFound ))
				{
					if (detectPhase == 2)
					{
						var bindingSource = new MouseBindingSource( detectFound );
						Reset();
						return bindingSource;
					}
				}
			}

			var control = ListenForControl();
			if (control != Mouse.None)
			{
				if (detectPhase == 1)
				{
					detectFound = control;
					detectPhase = 2; // Wait for release.
				}
			}
			else
			{
				if (detectPhase == 0)
				{
					detectPhase = 1; // Wait for press.
				}
			}

			return null;
		}


		bool IsPressed( Mouse control )
		{
			if (control == Mouse.LeftButton)
			{
				return Input.GetMouseButton( 0 );
			}

			if (control == Mouse.RightButton)
			{
				return Input.GetMouseButton( 1 );
			}

			if (control == Mouse.MiddleButton)
			{
				return Input.GetMouseButton( 2 );
			}

			return false;
		}


		Mouse ListenForControl()
		{
			if (Input.GetMouseButton( 0 ))
			{
				return Mouse.LeftButton;
			}

			if (Input.GetMouseButton( 1 ))
			{
				return Mouse.RightButton;
			}

			if (Input.GetMouseButton( 2 ))
			{
				return Mouse.MiddleButton;
			}

			return Mouse.None;
		}
	}
}

