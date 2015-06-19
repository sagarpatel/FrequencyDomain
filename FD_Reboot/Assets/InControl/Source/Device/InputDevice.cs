using System;
using UnityEngine;


namespace InControl
{
	public class InputDevice
	{
		public static readonly InputDevice Null = new InputDevice( "None" );

		internal int SortOrder = int.MaxValue;

		public string Name { get; protected set; }
		public string Meta { get; protected set; }

		public ulong LastChangeTick { get; protected set; }

		public InputControl[] Controls { get; protected set; }

		public OneAxisInputControl LeftStickX { get; protected set; }
		public OneAxisInputControl LeftStickY { get; protected set; }
		public TwoAxisInputControl LeftStick { get; protected set; }

		public OneAxisInputControl RightStickX { get; protected set; }
		public OneAxisInputControl RightStickY { get; protected set; }
		public TwoAxisInputControl RightStick { get; protected set; }

		public OneAxisInputControl DPadX { get; protected set; }
		public OneAxisInputControl DPadY { get; protected set; }
		public TwoAxisInputControl DPad { get; protected set; }

		public InputControl Command { get; protected set; }

		public bool IsAttached { get; internal set; }


		public InputDevice( string name )
		{
			Name = name;
			Meta = "";

			LastChangeTick = 0;

			const int numInputControlTypes = (int) InputControlType.Count + 1;
			Controls = new InputControl[numInputControlTypes];

			LeftStickX = new OneAxisInputControl();
			LeftStickY = new OneAxisInputControl();
			LeftStick = new TwoAxisInputControl();
			LeftStick.LowerDeadZone = 0.2f;

			RightStickX = new OneAxisInputControl();
			RightStickY = new OneAxisInputControl();
			RightStick = new TwoAxisInputControl();
			RightStick.LowerDeadZone = 0.2f;

			DPadX = new OneAxisInputControl();
			DPadY = new OneAxisInputControl();
			DPad = new TwoAxisInputControl();
			DPad.LowerDeadZone = 0.2f;

			Command = AddControl( InputControlType.Command, "Command" );
		}


		public bool HasControl( InputControlType inputControlType )
		{
			return Controls[(int) inputControlType] != null;
		}


		public InputControl GetControl( InputControlType inputControlType )
		{
			var control = Controls[(int) inputControlType];
			return control ?? InputControl.Null;
		}


		// Warning: this is super inefficient. Don't use it unless you have to, m'kay?
		public static InputControlType GetInputControlTypeByName( string inputControlName )
		{
			return (InputControlType) Enum.Parse( typeof(InputControlType), inputControlName );
		}


		// Warning: this is super inefficient. Don't use it unless you have to, m'kay?
		public InputControl GetControlByName( string inputControlName )
		{
			var inputControlType = GetInputControlTypeByName( inputControlName );
			return GetControl( inputControlType );
		}


		public InputControl AddControl( InputControlType inputControlType, string handle )
		{
			var inputControl = new InputControl( handle, inputControlType );
			Controls[(int) inputControlType] = inputControl;
			return inputControl;
		}


		internal void UpdateWithState( InputControlType inputControlType, bool state, ulong updateTick, float deltaTime )
		{
			GetControl( inputControlType ).UpdateWithState( state, updateTick, deltaTime );
		}


		internal void UpdateWithValue( InputControlType inputControlType, float value, ulong updateTick, float deltaTime )
		{
			GetControl( inputControlType ).UpdateWithValue( value, updateTick, deltaTime );
		}


		internal void UpdateLeftStickWithValue( Vector2 value, ulong updateTick, float deltaTime )
		{
			LeftStickLeft.UpdateWithValue( Mathf.Max( 0.0f, -value.x ), updateTick, deltaTime );
			LeftStickRight.UpdateWithValue( Mathf.Max( 0.0f, value.x ), updateTick, deltaTime );

			if (InputManager.InvertYAxis)
			{
				LeftStickUp.UpdateWithValue( Mathf.Max( 0.0f, -value.y ), updateTick, deltaTime );
				LeftStickDown.UpdateWithValue( Mathf.Max( 0.0f, value.y ), updateTick, deltaTime );
			}
			else
			{
				LeftStickUp.UpdateWithValue( Mathf.Max( 0.0f, value.y ), updateTick, deltaTime );
				LeftStickDown.UpdateWithValue( Mathf.Max( 0.0f, -value.y ), updateTick, deltaTime );
			}
		}


		internal void UpdateRightStickWithValue( Vector2 value, ulong updateTick, float deltaTime )
		{
			RightStickLeft.UpdateWithValue( Mathf.Max( 0.0f, -value.x ), updateTick, deltaTime );
			RightStickRight.UpdateWithValue( Mathf.Max( 0.0f, value.x ), updateTick, deltaTime );

			if (InputManager.InvertYAxis)
			{
				RightStickUp.UpdateWithValue( Mathf.Max( 0.0f, -value.y ), updateTick, deltaTime );
				RightStickDown.UpdateWithValue( Mathf.Max( 0.0f, value.y ), updateTick, deltaTime );
			}
			else
			{
				RightStickUp.UpdateWithValue( Mathf.Max( 0.0f, value.y ), updateTick, deltaTime );
				RightStickDown.UpdateWithValue( Mathf.Max( 0.0f, -value.y ), updateTick, deltaTime );
			}
		}


		public virtual void Update( ulong updateTick, float deltaTime )
		{
			// Implemented by subclasses.
		}


		bool AnyCommandControlIsPressed()
		{
			for (int i = (int) InputControlType.Back; i <= (int) InputControlType.View; i++)
			{
				var control = Controls[i];
				if (control != null && control.IsPressed)
				{
					return true;
				}
			}

			return false;
		}


		public void Commit( ulong updateTick, float deltaTime )
		{
			int controlCount = Controls.Length;
			for (int i = 0; i < controlCount; i++)
			{
				var control = Controls[i];
				if (control != null)
				{
					control.Commit();

					if (control.HasChanged)
					{
						LastChangeTick = updateTick;
					}
				}
			}

			if (IsKnown)
			{
				GetControl( InputControlType.Command ).CommitWithState( AnyCommandControlIsPressed(), updateTick, deltaTime );
			}

			LeftStickX.CommitWithSides( LeftStickLeft, LeftStickRight, updateTick, deltaTime );
			LeftStickY.CommitWithSides( LeftStickDown, LeftStickUp, updateTick, deltaTime, InputManager.InvertYAxis );
			LeftStick.UpdateWithAxes( LeftStickX, LeftStickY, updateTick, deltaTime );

			RightStickX.CommitWithSides( RightStickLeft, RightStickRight, updateTick, deltaTime );
			RightStickY.CommitWithSides( RightStickDown, RightStickUp, updateTick, deltaTime, InputManager.InvertYAxis );
			RightStick.UpdateWithAxes( RightStickX, RightStickY, updateTick, deltaTime );

			DPadX.CommitWithSides( DPadLeft, DPadRight, updateTick, deltaTime );
			DPadY.CommitWithSides( DPadDown, DPadUp, updateTick, deltaTime, InputManager.InvertYAxis );
			DPad.UpdateWithAxes( DPadX, DPadY, updateTick, deltaTime );
		}


		public bool LastChangedAfter( InputDevice otherDevice )
		{
			return LastChangeTick > otherDevice.LastChangeTick;
		}


		internal void RequestActivation()
		{
			LastChangeTick = InputManager.CurrentTick;
		}


		public virtual void Vibrate( float leftMotor, float rightMotor )
		{
		}


		public void Vibrate( float intensity )
		{
			Vibrate( intensity, intensity );
		}


		public virtual bool IsSupportedOnThisPlatform
		{
			get { return true; }
		}


		public virtual bool IsKnown
		{
			get { return true; }
		}


		public bool MenuWasPressed
		{
			get
			{
				return GetControl( InputControlType.Command ).WasPressed;
			}
		}


		public InputControl AnyButton
		{
			get
			{
				int controlCount = Controls.GetLength( 0 );
				for (int i = 0; i < controlCount; i++)
				{
					var control = Controls[i];
					if (control != null && control.IsButton && control.IsPressed)
					{
						return control;
					}
				}

				return InputControl.Null;
			}
		}

		public InputControl LeftStickUp { get { return GetControl( InputControlType.LeftStickUp ); } }
		public InputControl LeftStickDown { get { return GetControl( InputControlType.LeftStickDown ); } }
		public InputControl LeftStickLeft { get { return GetControl( InputControlType.LeftStickLeft ); } }
		public InputControl LeftStickRight { get { return GetControl( InputControlType.LeftStickRight ); } }

		public InputControl RightStickUp { get { return GetControl( InputControlType.RightStickUp ); } }
		public InputControl RightStickDown { get { return GetControl( InputControlType.RightStickDown ); } }
		public InputControl RightStickLeft { get { return GetControl( InputControlType.RightStickLeft ); } }
		public InputControl RightStickRight { get { return GetControl( InputControlType.RightStickRight ); } }

		public InputControl DPadUp { get { return GetControl( InputControlType.DPadUp ); } }
		public InputControl DPadDown { get { return GetControl( InputControlType.DPadDown ); } }
		public InputControl DPadLeft { get { return GetControl( InputControlType.DPadLeft ); } }
		public InputControl DPadRight { get { return GetControl( InputControlType.DPadRight ); } }

		public InputControl Action1 { get { return GetControl( InputControlType.Action1 ); } }
		public InputControl Action2 { get { return GetControl( InputControlType.Action2 ); } }
		public InputControl Action3 { get { return GetControl( InputControlType.Action3 ); } }
		public InputControl Action4 { get { return GetControl( InputControlType.Action4 ); } }

		public InputControl LeftTrigger { get { return GetControl( InputControlType.LeftTrigger ); } }
		public InputControl RightTrigger { get { return GetControl( InputControlType.RightTrigger ); } }

		public InputControl LeftBumper { get { return GetControl( InputControlType.LeftBumper ); } }
		public InputControl RightBumper { get { return GetControl( InputControlType.RightBumper ); } }

		public InputControl LeftStickButton { get { return GetControl( InputControlType.LeftStickButton ); } }
		public InputControl RightStickButton { get { return GetControl( InputControlType.RightStickButton ); } }


		public TwoAxisInputControl Direction
		{
			get
			{
				return DPad.UpdateTick > LeftStick.UpdateTick ? DPad : LeftStick;
			}
		}
	}
}

