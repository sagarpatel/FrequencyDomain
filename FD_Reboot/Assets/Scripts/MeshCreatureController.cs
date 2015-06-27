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

	float yawScale = 10.0f;
	float pitchScale = 10.0f;
	float rollScale = 10.0f;
	float bendScale = 0.250f;

	void OnEnable()
	{
		m_meshCreatureInputs = new MeshCreatureActions();

		m_meshCreatureInputs.YawLeft.AddDefaultBinding(Key.A);
		m_meshCreatureInputs.YawRight.AddDefaultBinding(Key.D);
		m_meshCreatureInputs.PitchUp.AddDefaultBinding(Key.W);
		m_meshCreatureInputs.PitchDown.AddDefaultBinding(Key.S);
		m_meshCreatureInputs.RollLeft.AddDefaultBinding(Key.Q);
		m_meshCreatureInputs.RollRight.AddDefaultBinding(Key.E);
		m_meshCreatureInputs.BendIn.AddDefaultBinding(Key.Z);
		m_meshCreatureInputs.BendOut.AddDefaultBinding(Key.X);
	}

	void Start()
	{
		m_meshCreaturePhysics = GetComponent<MeshCreaturePhysics>();
		m_meshTerrainGenerator = GetComponent<MeshTerrainGenerator>();
	}

	void Update()
	{
		float inputYaw = m_meshCreatureInputs.YawPitch.X * yawScale * Time.deltaTime;
		float inputPitch = m_meshCreatureInputs.YawPitch.Y * pitchScale * Time.deltaTime;
		float inputRoll = m_meshCreatureInputs.Roll.Value * rollScale * Time.deltaTime;
		float inputBend = m_meshCreatureInputs.Bend.Value * bendScale * Time.deltaTime;

		m_meshCreaturePhysics.IncrementCreatureRotationalVel( inputPitch , inputYaw, -inputRoll );
		m_meshTerrainGenerator.IncrementMeshBend(inputBend);
	}

}
