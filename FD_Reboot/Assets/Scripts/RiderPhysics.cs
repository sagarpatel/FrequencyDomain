using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{

	MeshTerrainGenerator m_meshTerrainGenerator;
	
	float m_widthRatio = 0; // 0 is front of terrain, 1 is the furthest back
	float m_depthRatio = 0; // -1 is full left, +1 is full right
	float m_heightOffset = 0;

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
	
	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
		
	}
	
	void Update()
	{
		// apply velocites

		
		// check for looping around mesh
		float nextWidthAbs = Mathf.Abs(m_widthRatio) + Mathf.Abs(m_widthVelocity * Time.deltaTime);
		if( m_meshTerrainGenerator.IsLoopClosed() == true && nextWidthAbs >= m_widthRange)
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

		m_depthRatio = Mathf.Clamp( m_depthRatio + m_depthVelocity * Time.deltaTime, m_depthRange_Min, m_depthRange_Max);
		m_heightOffset = Mathf.Clamp( m_heightOffset + m_heightVelocity * Time.deltaTime, m_heightOffsetRange_Min, m_heightOffsetRange_Max);

		// update velocity decays/gravity
		if(m_widthDecayFlag == true)
			m_widthVelocity -= m_widthVelocity * m_widthVelocityDecay * Time.deltaTime;
		if(m_depthDecayFlag == true)
			m_depthVelocity -= m_depthVelocity * m_depthVelocityDecay * Time.deltaTime;

		m_heightVelocity -= m_gravity * Time.deltaTime; // could do a check to see if touching groung to kill vel, but no real need for it now

		UpdateRiderWolrdPosRot();
		
	}
	
	void UpdateRiderWolrdPosRot()
	{
		int meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
		int frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;
		
		int targetMeshStripIndexOffset = (int)(m_depthRatio * (float)meshStripsCount);
		int targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
		MeshStripGenerator targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];
		
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		targetMeshStripGenerator.CalculatePositionOnStrip( m_widthRatio, m_heightOffset, out pos, out rot);
		
		transform.position = pos;
		transform.rotation = rot;
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
