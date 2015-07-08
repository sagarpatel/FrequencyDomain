using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using InControl;

public class MeshCreatureActions: PlayerActionSet
{
	public PlayerAction YawLeft;
	public PlayerAction YawRight;
	public PlayerAction PitchUp;
	public PlayerAction PitchDown;
	public PlayerAction RollLeft;
	public PlayerAction RollRight;
	public PlayerTwoAxisAction YawPitch;
	public PlayerOneAxisAction Roll;
	public PlayerAction BendIn;
	public PlayerAction BendOut;
	public PlayerOneAxisAction Bend;
	public PlayerAction SpeedUp;

	public MeshCreatureActions()
	{
		YawLeft = CreatePlayerAction("Yaw Left");
		YawRight = CreatePlayerAction("Yaw Right");
		PitchUp = CreatePlayerAction("Pitch Up");
		PitchDown = CreatePlayerAction("Pitch Down");
		RollLeft = CreatePlayerAction("Roll Left");
		RollRight = CreatePlayerAction("Roll Right");
		YawPitch = CreateTwoAxisPlayerAction(YawLeft, YawRight, PitchDown, PitchUp);
		Roll = CreateOneAxisPlayerAction(RollLeft, RollRight);

		BendIn = CreatePlayerAction("Bend In");
		BendOut = CreatePlayerAction("Bend Out");
		Bend = CreateOneAxisPlayerAction(BendOut, BendIn);

		SpeedUp = CreatePlayerAction("Speed Up");
	}

	// for controller
	public MeshCreatureActions(int deviceIndex)
	{
		YawLeft = CreatePlayerAction("Yaw Left");
		YawRight = CreatePlayerAction("Yaw Right");
		PitchUp = CreatePlayerAction("Pitch Up");
		PitchDown = CreatePlayerAction("Pitch Down");
		RollLeft = CreatePlayerAction("Roll Left");
		RollRight = CreatePlayerAction("Roll Right");
		YawPitch = CreateTwoAxisPlayerAction(YawLeft, YawRight, PitchDown, PitchUp);
		Roll = CreateOneAxisPlayerAction(RollLeft, RollRight);

		BendIn = CreatePlayerAction("Bend In");
		BendOut = CreatePlayerAction("Bend Out");
		Bend = CreateOneAxisPlayerAction(BendOut, BendIn);

		SpeedUp = CreatePlayerAction("Speed Up");

		if(deviceIndex >= 0)
		{
			DeviceIndex = deviceIndex;
		}
	}

}


public class MeshCreatureController : MonoBehaviour 
{
	MeshCreaturePhysics m_meshCreaturePhysics;
	MeshTerrainGenerator m_meshTerrainGenerator;
	MeshCreatureActions m_meshCreatureInputs;
	MeshTerrainBendPhysics m_meshTerrainBendPhysics;

	float yawScale = 15.0f;
	float pitchScale = 20.0f;
	float rollScale = 20.0f;
	float bendScale = 0.250f;

	void OnEnable()
	{
		if(InputManager.Devices.Count >= 2)
			m_meshCreatureInputs = new MeshCreatureActions(1);
		else
			m_meshCreatureInputs = new MeshCreatureActions();

		//keyboard controls
		m_meshCreatureInputs.YawLeft.AddDefaultBinding(Key.A);
		m_meshCreatureInputs.YawRight.AddDefaultBinding(Key.D);
		m_meshCreatureInputs.PitchUp.AddDefaultBinding(Key.W);
		m_meshCreatureInputs.PitchDown.AddDefaultBinding(Key.S);
		m_meshCreatureInputs.RollLeft.AddDefaultBinding(Key.Q);
		m_meshCreatureInputs.RollRight.AddDefaultBinding(Key.E);
		m_meshCreatureInputs.BendIn.AddDefaultBinding(Key.Z);
		m_meshCreatureInputs.BendOut.AddDefaultBinding(Key.X);
		m_meshCreatureInputs.SpeedUp.AddDefaultBinding(Key.F);

		// gamepad controls
		m_meshCreatureInputs.YawLeft.AddDefaultBinding( InputControlType.LeftStickLeft );
		m_meshCreatureInputs.YawRight.AddDefaultBinding( InputControlType.LeftStickRight );
		m_meshCreatureInputs.PitchUp.AddDefaultBinding( InputControlType.LeftStickUp );
		m_meshCreatureInputs.PitchDown.AddDefaultBinding( InputControlType.LeftStickDown );
		m_meshCreatureInputs.RollLeft.AddDefaultBinding( InputControlType.LeftTrigger );
		m_meshCreatureInputs.RollRight.AddDefaultBinding( InputControlType.RightTrigger );
		m_meshCreatureInputs.BendIn.AddDefaultBinding( InputControlType.LeftBumper );
		m_meshCreatureInputs.BendOut.AddDefaultBinding( InputControlType.RightBumper );
		m_meshCreatureInputs.SpeedUp.AddDefaultBinding( InputControlType.Action1 );
	}

	void Start()
	{
		m_meshCreaturePhysics = GetComponent<MeshCreaturePhysics>();
		m_meshTerrainGenerator = GetComponent<MeshTerrainGenerator>();
		m_meshTerrainBendPhysics = GetComponent<MeshTerrainBendPhysics>();
	}

	void Update()
	{
		float inputYaw = m_meshCreatureInputs.YawPitch.X * yawScale * Time.deltaTime;
		float inputPitch = m_meshCreatureInputs.YawPitch.Y * pitchScale * Time.deltaTime;
		float inputRoll = m_meshCreatureInputs.Roll.Value * rollScale * Time.deltaTime;
		float inputBend = m_meshCreatureInputs.Bend.Value * bendScale * Time.deltaTime;
		float inputSpeedUp = m_meshCreatureInputs.SpeedUp.Value * Time.deltaTime;

		m_meshCreaturePhysics.IncrementCreatureRotationalVel( inputPitch , inputYaw, -inputRoll );
		m_meshCreaturePhysics.IncrementExtraSpeedStep(inputSpeedUp);
		m_meshTerrainBendPhysics.IncrementMeshBendVelocity(inputBend);
	}

}
