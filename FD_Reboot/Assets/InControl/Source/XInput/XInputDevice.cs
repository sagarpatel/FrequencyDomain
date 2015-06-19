#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


namespace InControl
{
	public class XInputDevice : InputDevice
	{
		public int DeviceIndex { get; private set; }

		XInputDeviceManager owner;
		GamePadState state;


		public XInputDevice( int deviceIndex, XInputDeviceManager owner )
			: base( "Xbox 360 Controller (XInput)" )
		{
			this.owner = owner;

			DeviceIndex = deviceIndex;
			SortOrder = deviceIndex;

			Meta = "XInput Device #" + deviceIndex;

			AddControl( InputControlType.LeftStickLeft, "Left Stick Left" );
			AddControl( InputControlType.LeftStickRight, "Left Stick Right" );
			AddControl( InputControlType.LeftStickUp, "Left Stick Up" );
			AddControl( InputControlType.LeftStickDown, "Left Stick Down" );

			AddControl( InputControlType.RightStickLeft, "Right Stick Left" );
			AddControl( InputControlType.RightStickRight, "Right Stick Right" );
			AddControl( InputControlType.RightStickUp, "Right Stick Up" );
			AddControl( InputControlType.RightStickDown, "Right Stick Down" );

			AddControl( InputControlType.LeftTrigger, "Left Trigger" );
			AddControl( InputControlType.RightTrigger, "Right Trigger" );

			AddControl( InputControlType.DPadUp, "DPad Up" );
			AddControl( InputControlType.DPadDown, "DPad Down" );
			AddControl( InputControlType.DPadLeft, "DPad Left" );
			AddControl( InputControlType.DPadRight, "DPad Right" );

			AddControl( InputControlType.Action1, "A" );
			AddControl( InputControlType.Action2, "B" );
			AddControl( InputControlType.Action3, "X" );
			AddControl( InputControlType.Action4, "Y" );

			AddControl( InputControlType.LeftBumper, "Left Bumper" );
			AddControl( InputControlType.RightBumper, "Right Bumper" );

			AddControl( InputControlType.LeftStickButton, "Left Stick Button" );
			AddControl( InputControlType.RightStickButton, "Right Stick Button" );

			AddControl( InputControlType.Start, "Start" );
			AddControl( InputControlType.Back, "Back" );
		}


		public override void Update( ulong updateTick, float deltaTime )
		{
			GetState();

			UpdateLeftStickWithValue( state.ThumbSticks.Left.Vector, updateTick, deltaTime );
			UpdateRightStickWithValue( state.ThumbSticks.Right.Vector, updateTick, deltaTime );

			UpdateWithValue( InputControlType.LeftTrigger, state.Triggers.Left, updateTick, deltaTime );
			UpdateWithValue( InputControlType.RightTrigger, state.Triggers.Right, updateTick, deltaTime );

			UpdateWithState( InputControlType.DPadUp, state.DPad.Up == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadDown, state.DPad.Down == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadLeft, state.DPad.Left == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadRight, state.DPad.Right == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.Action1, state.Buttons.A == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action2, state.Buttons.B == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action3, state.Buttons.X == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action4, state.Buttons.Y == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftBumper, state.Buttons.LeftShoulder == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.RightBumper, state.Buttons.RightShoulder == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftStickButton, state.Buttons.LeftStick == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.RightStickButton, state.Buttons.RightStick == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.Start, state.Buttons.Start == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Back, state.Buttons.Back == ButtonState.Pressed, updateTick, deltaTime );

			Commit( updateTick, deltaTime );
		}


		public override void Vibrate( float leftMotor, float rightMotor )
		{
			GamePad.SetVibration( (PlayerIndex) DeviceIndex, leftMotor, rightMotor );
		}


		internal void GetState()
		{
			state = owner.GetState( DeviceIndex );
		}


		public bool IsConnected
		{
			get { return state.IsConnected; }
		}
	}
}
#endif
