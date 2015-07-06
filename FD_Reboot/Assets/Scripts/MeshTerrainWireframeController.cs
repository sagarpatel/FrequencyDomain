using UnityEngine;
using System.Collections;

public class MeshTerrainWireframeController : MonoBehaviour 
{
	MeshTerrainGenerator m_meshTerrainGenerator;
	Material m_meshTerrainMaterial;
	string m_cutoffPropertyName = "_Cutoff";

	float m_wireframeMin = 0.04f;
	float m_wireframeMax = 1.0f;

	float m_currentWireframeValue = 0.0f;
	float m_wireframeValueDecay = 0.150f;

	void Start()
	{
		m_meshTerrainGenerator = GetComponent<MeshTerrainGenerator>();
		m_meshTerrainMaterial = m_meshTerrainGenerator.m_meshStripsMaterial;
		m_currentWireframeValue = m_wireframeMin;
	}

	void Update()
	{
		m_currentWireframeValue = Mathf.Clamp(m_currentWireframeValue - m_wireframeValueDecay * Time.deltaTime, m_wireframeMin, m_wireframeMax);
		m_meshTerrainMaterial.SetFloat(m_cutoffPropertyName, m_currentWireframeValue);
	}

	public void IncrementWireframeValue(float incrementValue)
	{
		m_currentWireframeValue = Mathf.Clamp(m_currentWireframeValue + incrementValue, m_wireframeMin, m_wireframeMax);
	}

}
