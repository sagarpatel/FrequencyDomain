using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using InControl;


public class RiderActions : PlayerActionSet
{
	public PlayerAction Warp;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Down;
	public PlayerAction Up;
	public PlayerTwoAxisAction Move;
	public PlayerAction RollRight;
	public PlayerAction RollLeft;
	public PlayerOneAxisAction BarrelRoll;

	// keyboard inputs
	public RiderActions()
	{
		Warp = CreatePlayerAction( "Warp" );
		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
		RollRight = CreatePlayerAction("Roll Right");
		RollLeft = CreatePlayerAction("Roll Left");
		BarrelRoll = CreateOneAxisPlayerAction(RollLeft, RollRight);
	}

	// controller inputs
	public RiderActions(int deviceIndex)
	{
		Warp = CreatePlayerAction( "Warp" );
		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
		RollRight = CreatePlayerAction("Roll Right");
		RollLeft = CreatePlayerAction("Roll Left");
		BarrelRoll = CreateOneAxisPlayerAction(RollLeft, RollRight);

		if(deviceIndex >= 0)
		{
			DeviceIndex = deviceIndex;
		}
	}
}


public class RiderController : MonoBehaviour 
{
	RiderActions m_riderInput;
	RiderPhysics m_riderPhysics;
	RiderCamera_BarrelRollPhysics m_riderCameraBarrelRollPhysics;
	RiderCamera_FOVController m_riderCameraFOVController;

	float m_widthMoveScale = 0.250f;
	float m_depthMoveScale = 0.150f;
	float m_barrelRollInputScaler = 900.0f;
	float m_barrelRollInputTimeCounter = 0;

	public AnimationCurve barrelRollInputCurve;

	void OnEnable()
	{
		m_riderInput = new RiderActions();

		// keyboard controlls
		m_riderInput.Warp.AddDefaultBinding( Key.Space );
		m_riderInput.Up.AddDefaultBinding( Key.I );
		m_riderInput.Down.AddDefaultBinding( Key.K );
		m_riderInput.Left.AddDefaultBinding( Key.J );
		m_riderInput.Right.AddDefaultBinding( Key.L );
		m_riderInput.RollRight.AddDefaultBinding( Key.O );
		m_riderInput.RollLeft.AddDefaultBinding( Key.U );

		// gamepad controls
		m_riderInput.Warp.AddDefaultBinding( InputControlType.Action3 );
		m_riderInput.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
		m_riderInput.Right.AddDefaultBinding( InputControlType.LeftStickRight );
		m_riderInput.Up.AddDefaultBinding( InputControlType.LeftStickUp );
		m_riderInput.Down.AddDefaultBinding( InputControlType.LeftStickDown );
		m_riderInput.RollRight.AddDefaultBinding( InputControlType.RightTrigger );
		m_riderInput.RollLeft.AddDefaultBinding( InputControlType.LeftTrigger );
	}

	void Start()
	{
		m_riderPhysics = GetComponent<RiderPhysics>();
		m_riderCameraBarrelRollPhysics = GetComponentInChildren<RiderCamera_BarrelRollPhysics>();
		m_riderCameraFOVController = GetComponentInChildren<RiderCamera_FOVController>();
	}

	void Update()
	{
		float widthMove = m_riderInput.Move.X * m_widthMoveScale * Time.deltaTime;
		float depthMove = m_riderInput.Move.Y * m_depthMoveScale * Time.deltaTime;
		float barrelRollInput = m_riderInput.BarrelRoll;

		if(Mathf.Abs(barrelRollInput) > 0)
			m_barrelRollInputTimeCounter += Time.deltaTime;
		else
			m_barrelRollInputTimeCounter = 0;

		m_barrelRollInputTimeCounter = Mathf.Clamp(m_barrelRollInputTimeCounter, 0, 1);
		float barrelRollIncrement = Mathf.Sign(barrelRollInput) * barrelRollInputCurve.Evaluate(m_barrelRollInputTimeCounter) * m_barrelRollInputScaler * Time.deltaTime ;

		m_riderPhysics.IncrementWidthDepthVelocities(widthMove, depthMove);
		m_riderCameraBarrelRollPhysics.IncrementBarrelRollVelocity( barrelRollIncrement );

		float warpInput = m_riderInput.Warp;
		m_riderCameraFOVController.RiderCameraFOVInput(warpInput);
	}

}
