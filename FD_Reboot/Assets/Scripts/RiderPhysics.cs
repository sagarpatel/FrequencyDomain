using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiderPhysics : MonoBehaviour 
{

	MeshTerrainGenerator m_meshTerrainGenerator;
	MeshTerrainWireframeController m_meshTerrainWireframeController;

	float m_widthRatio = 0; // 0 is front of terrain, 1 is the furthest back
	float m_depthRatio = 0; // -1 is full left, +1 is full right
	float m_heightOffsetBaseCurve = 0;
	float m_heighOffsetOffTerrain = 0;
	float m_heightOffsetRaw = 0;

	float m_widthVelocity = 0;
	float m_depthVelocity = 0;
	float m_heightVelocity = 0;
	
	float m_widthRange = 0.9990f;
	float m_depthRange_Min = 0.10f;
	float m_depthRange_Max = 0.70f;
	float m_heightOffsetBaseCurveRange_Min = 0;
	float m_heightOffsetBaseCurveRange_Max = 300.0f; // TODO need to convert this to ratio

	float m_widthVelocityDecay = 2.0f;
	float m_depthVelocityDecay = 3.0f;
	bool m_widthDecayFlag = false;
	bool m_depthDecayFlag = false;

	float m_widthVelocity_Min = -1.0f;
	float m_widthVelocity_Max = 1.0f;
	float m_depthVelocity_Min = -0.250f;
	float m_depthVelocity_Max = 0.250f;

	float m_gravity = 200.0f;
	float m_newTerrainHeight = 0;
	float m_oldTerrainHeight = 0;
	float m_heightAccumulator = 0;
	float m_heightDeltaEpsilon = 0.1f;
	float m_heightAccumulatorScaler = 1.70f;

	float m_airtimeCounter = 0;
	float m_airtimeBurstScaler = 0.15f;
	float m_wireframeBurstAirTimethreashold = 0.5f;

	enum RiderHeightState
	{
		RisingGround,
		RisingAir,
		FallingAir,
		FallingGround
	};
	RiderHeightState m_newRiderHeightState = RiderHeightState.FallingGround;
	RiderHeightState m_oldRiderHeightState = RiderHeightState.FallingGround;

	List<Vector4> m_riderAirtimeStateRecordingList; // use x for depthRatio, y for widthRatio, z for height offset, w for barrel roll value
	List<Color> m_audioAirtimeStateRecordingList;
	List<Quaternion> m_riderCameraRotationsRecordingList;
	FrequencyDataManager m_frequencyDataManager;
	GhostRiderCreaturesGenerator m_ghostRiderCreaturesGenerator;
	Transform m_riderCameraTransform;
	
	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
		m_meshTerrainWireframeController = FindObjectOfType<MeshTerrainWireframeController>();
		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();
		m_ghostRiderCreaturesGenerator = FindObjectOfType<GhostRiderCreaturesGenerator>();
		m_riderCameraTransform = GetComponentInChildren<Camera>().transform;

		m_riderAirtimeStateRecordingList = new List<Vector4>();
		m_audioAirtimeStateRecordingList = new List<Color>();
		m_riderCameraRotationsRecordingList = new List<Quaternion>();
	}
	
	void Update()
	{
		m_oldTerrainHeight = m_newTerrainHeight;
		m_oldRiderHeightState = m_newRiderHeightState;

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
		if( targetMeshStripGenerator.IsLoopClosed() == true && nextWidthAbs >= m_widthRange && Mathf.Sign(m_widthVelocity) == Mathf.Sign(m_widthRatio) )
		{
			// loop around to the other side
			float widthLoopDiff = nextWidthAbs - m_widthRange;
			m_widthRatio = -Mathf.Sign(m_widthRatio) * (m_widthRange - widthLoopDiff);
			//Debug.Log("FLippin! " + Time.frameCount);
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
		m_newTerrainHeight = targetMeshStripGenerator.CalculateTerrainHeightValue(m_widthRatio);
		float terrainHeightDiff = m_newTerrainHeight - m_oldTerrainHeight;

		if(m_heightOffsetBaseCurve <= m_newTerrainHeight) // touching ground
		{
			if(terrainHeightDiff > m_heightDeltaEpsilon) // rising while hugging the terrain
			{
				m_newRiderHeightState = RiderHeightState.RisingGround;
				m_heightAccumulator += terrainHeightDiff;
				m_heightVelocity = 0; // kill gravity pull since rider is grinding up
			}
			else // riding terrain down
			{
				m_newRiderHeightState = RiderHeightState.FallingGround;
				m_heightOffsetBaseCurve = 0;
				m_heightOffsetRaw = 0;
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

		// increment air time
		if(m_newRiderHeightState == RiderHeightState.RisingAir || m_newRiderHeightState == RiderHeightState.FallingAir)
		{
			m_airtimeCounter += Time.deltaTime;
			m_riderAirtimeStateRecordingList.Add(new Vector4(m_depthRatio, m_widthRatio, m_heightOffsetBaseCurve, 0)); // TODO : add barrel roll rotatoin as w, height data is a frame let, meh rushing for bitsummit now
			m_audioAirtimeStateRecordingList.Add(m_frequencyDataManager.d_color);
			m_riderCameraRotationsRecordingList.Add(m_riderCameraTransform.rotation);
		}

		// first frame of jump
		if( m_oldRiderHeightState == RiderHeightState.RisingGround && terrainHeightDiff < -m_heightDeltaEpsilon )
		{
			//Debug.Log("jumpn, heigh accu: " + m_heightAccumulator);
			m_heightVelocity = m_heightAccumulatorScaler * m_heightAccumulator;
			m_heightAccumulator = 0;
			m_riderAirtimeStateRecordingList.Clear();
			m_audioAirtimeStateRecordingList.Clear();
			m_riderCameraRotationsRecordingList.Clear();
		}

		// first contact on ground after air time
		if( m_oldRiderHeightState == RiderHeightState.FallingAir && ( m_newRiderHeightState == RiderHeightState.FallingGround || m_newRiderHeightState == RiderHeightState.RisingGround ) ) //m_heightOffsetBaseCurve <= m_newTerrainHeight)
		{
			//Debug.Log(m_airtimeCounter);

			if(m_airtimeCounter > m_wireframeBurstAirTimethreashold)
			{
				//Debug.LogError("brst");
				//targetMeshStripGenerator.LaunchWireframeBurstAnimation(null, m_airtimeCounter * m_airtimeBurstScaler);
				//m_meshTerrainWireframeController.IncrementWireframeValue(m_airtimeCounter * m_airtimeBurstScaler);
				m_ghostRiderCreaturesGenerator.SpawnGhostRiderCreature(m_riderAirtimeStateRecordingList.ToArray(), m_audioAirtimeStateRecordingList.ToArray(), m_riderCameraRotationsRecordingList.ToArray(), m_airtimeCounter);
				//Debug.LogError("spawn");
			}
			m_airtimeCounter = 0;
		}

		// calcluations for how high off the the mesh rider should be 
		m_heightOffsetRaw = Mathf.Clamp( m_heightOffsetRaw + m_heightVelocity * Time.deltaTime,0, m_heightOffsetBaseCurveRange_Max);
		m_heightOffsetBaseCurve = Mathf.Clamp( m_heightOffsetBaseCurve + m_heightVelocity * Time.deltaTime, m_newTerrainHeight, m_heightOffsetBaseCurveRange_Max);
		m_heighOffsetOffTerrain = Mathf.Clamp( m_heightOffsetBaseCurve - m_newTerrainHeight, m_heightOffsetBaseCurveRange_Min, m_heightOffsetBaseCurveRange_Max);

		targetMeshStripGenerator.CalculatePositionOnStrip_Rider( m_widthRatio, m_heighOffsetOffTerrain, out pos, out rot);		

		// final position calcluations and set
		transform.position = pos;
		transform.rotation = rot;
	}


	public void IncrementWidthDepthVelocities(float extraWdith, float extraDepth)
	{
		if( Mathf.Sign(m_widthVelocity) != Mathf.Sign(extraWdith))
			extraWdith = 2.0f * extraWdith;
		if( Mathf.Sign(m_depthVelocity) != Mathf.Sign(extraDepth))
		   	extraDepth = 2.0f * extraDepth;

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

	public float CalculateVelocityRatio_Width()
	{
		return Mathf.Sign(m_widthVelocity) * Mathf.InverseLerp( 0, m_widthVelocity_Max, Mathf.Abs(m_widthVelocity));
	}

}
