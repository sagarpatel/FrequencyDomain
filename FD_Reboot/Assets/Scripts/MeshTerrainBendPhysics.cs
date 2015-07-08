using UnityEngine;
using System.Collections;

public class MeshTerrainBendPhysics : MonoBehaviour 
{
	MeshTerrainGenerator m_meshTerrainGenerator;

	float m_currentBend = 0;
	float m_bendVelocity = 0.0f;
	float m_bendVeolcityDecay = 1.0f;

	float m_bendRange = 1.0f;
	float m_bendVelRange = 10.0f;
	bool m_bendDecayFlag = false;

	void Start()
	{
		m_meshTerrainGenerator = GetComponent<MeshTerrainGenerator>();
	}

	void Update()
	{

		m_currentBend = Mathf.Clamp( m_currentBend + m_bendVelocity * Time.deltaTime, -m_bendRange, m_bendRange);

		if(m_bendDecayFlag == true)
			m_bendVelocity -= m_bendVelocity * m_bendVeolcityDecay * Time.deltaTime;

		m_meshTerrainGenerator.SetMeshBendValue(m_currentBend);

		if(m_currentBend == -m_bendRange || m_currentBend == m_bendRange)
			m_bendVelocity = 0;

	}

	public void IncrementMeshBendVelocity(float velIncrement)
	{
		m_bendVelocity = Mathf.Clamp( m_bendVelocity + velIncrement, -m_bendVelRange, m_bendVelRange);
		if(velIncrement == 0)
			m_bendDecayFlag = true;
		else
			m_bendDecayFlag = false;
	}
}
