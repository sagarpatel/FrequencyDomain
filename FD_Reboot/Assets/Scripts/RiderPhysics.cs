﻿using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{

	MeshTerrainGenerator m_meshTerrainGenerator;
	
	float m_widthRatio = 0; // 0 is front of terrain, 1 is the furthest back
	float m_depthRatio = 0; // -1 is full left, +1 is full right
	float m_heightOffsetBaseCurve = 0;

	float m_widthVelocity = 0;
	float m_depthVelocity = 0;
	float m_heightVelocity = 0;
	
	float m_widthRange = 0.990f;
	float m_depthRange_Min = 0.10f;
	float m_depthRange_Max = 0.90f;
	float m_heightOffsetRange_Min = 0;
	float m_heightOffsetRange_Max = 200.0f; // TODO need to convert this to ratio

	float m_widthVelocityDecay = 1.0f;
	float m_depthVelocityDecay = 1.0f;
	bool m_widthDecayFlag = false;
	bool m_depthDecayFlag = false;

	float m_widthVelocity_Min = -1.0f;
	float m_widthVelocity_Max = 1.0f;
	float m_depthVelocity_Min = -1.0f;
	float m_depthVelocity_Max = 1.0f;

	float m_gravity = 10.0f;
	float m_oldTerrainHeight = 0;
	float m_heightAccumulator = 0;

	enum RiderHeightState
	{
		RisingGround,
		RisingAir,
		FallingAir,
		FallingGround
	};
	RiderHeightState m_newRiderHeightState = RiderHeightState.FallingGround;
	RiderHeightState m_oldRiderHeightState = RiderHeightState.FallingGround;
	
	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
		
	}
	
	void Update()
	{
		// apply velocites
		// do depth first to figure out the target mesh
		m_depthRatio = Mathf.Clamp( m_depthRatio + m_depthVelocity * Time.deltaTime, m_depthRange_Min, m_depthRange_Max);
		// get target mesh
		int meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
		int frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;		
		int targetMeshStripIndexOffset = (int)(m_depthRatio * (float)meshStripsCount);
		int targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
		MeshStripGenerator targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];

		// now that we know target meshstrip, we can check if its looping
		// check for looping around mesh
		float nextWidthAbs = Mathf.Abs(m_widthRatio) + Mathf.Abs(m_widthVelocity * Time.deltaTime);
		if( targetMeshStripGenerator.IsLoopClosed() == true && nextWidthAbs >= m_widthRange)
		{
			// loop around to the other side
			float widthLoopDiff = nextWidthAbs - m_widthRange;
			m_widthRatio = -Mathf.Sign(m_widthRatio) * (m_widthRange - widthLoopDiff);
		}
		else
		{
			// ordinary move
			m_widthRatio = Mathf.Clamp( m_widthRatio + m_widthVelocity * Time.deltaTime, -m_widthRange, m_widthRange );
		}



		// update velocity decays/gravity
		if(m_widthDecayFlag == true)
			m_widthVelocity -= m_widthVelocity * m_widthVelocityDecay * Time.deltaTime;
		if(m_depthDecayFlag == true)
			m_depthVelocity -= m_depthVelocity * m_depthVelocityDecay * Time.deltaTime;




		// handle terrain height
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		float newTerrainHeight = targetMeshStripGenerator.CalculateTerrainHeightValue(m_widthRatio);
		float terrainHeightDiff = newTerrainHeight - m_oldTerrainHeight;

		if(m_heightOffsetBaseCurve <= newTerrainHeight) // toucing ground
		{
			if(terrainHeightDiff > 0) // rising while hugging the terrain
			{
				m_newRiderHeightState = RiderHeightState.RisingGround;
				m_heightAccumulator += terrainHeightDiff;
				m_heightVelocity = 0; // kill gravity pull since rider is grinding up
			}
			else // riding terrain down
			{
				m_newRiderHeightState = RiderHeightState.FallingGround;
				m_heightOffsetBaseCurve = 0;
			}

		}
		else // rider in the air
		{
			m_heightVelocity -= m_gravity * Time.deltaTime;

			if(m_heightVelocity > 0) // in the air rising
			{
				m_newRiderHeightState = RiderHeightState.RisingAir;
			}
			else // in the air falling
			{
				m_newRiderHeightState = RiderHeightState.FallingAir;
			}
		}

		// first frame of jump
		if( m_newRiderHeightState == RiderHeightState.RisingAir && m_oldRiderHeightState == RiderHeightState.RisingGround)
		{
			m_heightOffsetBaseCurve = m_heightAccumulator;
			m_heightAccumulator = 0;
		}


		// calcluations for how high off the the mesh rider should be 
		m_heightOffsetBaseCurve = Mathf.Clamp( m_heightOffsetBaseCurve + m_heightVelocity * Time.deltaTime, m_heightOffsetRange_Min, m_heightOffsetRange_Max);
		float heighOffsetOffTerrain = newTerrainHeight + m_heightOffsetBaseCurve;

		targetMeshStripGenerator.CalculatePositionOnStrip( m_widthRatio, heighOffsetOffTerrain, out pos, out rot);		



		// final position calcluations and set
		transform.position = pos;
		transform.rotation = rot;

		m_oldTerrainHeight = newTerrainHeight;
		m_oldRiderHeightState = m_newRiderHeightState;
	}


		

	public void IncrementWidthDepthVelocities(float extraWdith, float extraDepth)
	{
		m_widthVelocity = Mathf.Clamp( m_widthVelocity + extraWdith, m_widthVelocity_Min, m_widthVelocity_Max);
		m_depthVelocity = Mathf.Clamp( m_depthVelocity + extraDepth, m_depthVelocity_Min, m_depthVelocity_Max);

		if(extraWdith == 0)
			m_widthDecayFlag = true;
		else
			m_widthDecayFlag = false;

		if(extraDepth == 0)
			m_depthDecayFlag = true;
		else
			m_depthDecayFlag = false;
	}
}
