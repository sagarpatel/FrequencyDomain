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

	// keyboard inputs
	public RiderActions()
	{
		Warp = CreatePlayerAction( "Warp" );
		Left = CreatePlayerAction( "Move Left" );
		Right = CreatePlayerAction( "Move Right" );
		Up = CreatePlayerAction( "Move Up" );
		Down = CreatePlayerAction( "Move Down" );
		Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
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

	float m_widthMoveScale = 0.50f;
	float m_depthMoveScale = 0.50f;

	void OnEnable()
	{
		m_riderInput = new RiderActions();

		m_riderInput.Warp.AddDefaultBinding( Key.Space );
		m_riderInput.Up.AddDefaultBinding( Key.I );
		m_riderInput.Down.AddDefaultBinding( Key.K );
		m_riderInput.Left.AddDefaultBinding( Key.J );
		m_riderInput.Right.AddDefaultBinding( Key.L );

	}

	void Start()
	{
		m_riderPhysics = GetComponent<RiderPhysics>();
	}

	void Update()
	{

		float widthMove = m_riderInput.Move.X * m_widthMoveScale * Time.deltaTime;
		float depthMove = m_riderInput.Move.Y * m_depthMoveScale * Time.deltaTime;

		//Debug.Log("Width move: " + widthMove);
		//Debug.Log("Depth move: " + depthMove);

		m_riderPhysics.IncrementWidthDepthVelocities(widthMove, depthMove);
	}

}
