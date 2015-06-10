using UnityEngine;
using System.Collections;
using System.Linq;

public class MeshTerrainGenerator : MonoBehaviour 
{
	GameObject[] m_meshStripGeneratorsGOArray;
	MeshStripGenerator[] m_meshStripGeneratorsArray;
	int m_meshStripsPoolCount = 200;
	int m_lastActivatedStripIndex = 0;
	float m_distanceTravelledLastFrame = 0;
	float m_moveSpeed = 10.0f;
	Vector3 t_previousPosition;

	int m_stripsWidthVerticesCount = 256;
	float m_stripsWidthVerticesScale = 0.5f;

	Vector3[] m_lastGeneratedMeshStrip_FrontRowVerticesArray;
	Vector3[] t_calcFrontRowVertsArray; // used as buffer to perform calculations on

	void Start()
	{
		m_meshStripGeneratorsGOArray = new GameObject[m_meshStripsPoolCount];
		for(int i = 0; i < m_meshStripGeneratorsGOArray.Length; i++)
		{
			m_meshStripGeneratorsGOArray[i] = new GameObject();
			m_meshStripGeneratorsGOArray[i].name = "MeshStripGenerator_" + i.ToString();
			m_meshStripGeneratorsGOArray[i].transform.parent = transform;
			m_meshStripGeneratorsGOArray[i].transform.localPosition = Vector3.zero;
			m_meshStripGeneratorsGOArray[i].transform.localRotation = Quaternion.identity;
			m_meshStripGeneratorsGOArray[i].transform.localScale = Vector3.one;
			m_meshStripGeneratorsGOArray[i].AddComponent<MeshStripGenerator>();
		}
		m_meshStripGeneratorsArray = m_meshStripGeneratorsGOArray.Select(g => g.GetComponent<MeshStripGenerator>()).ToArray();

		for(int i = 0; i < m_meshStripGeneratorsGOArray.Length; i++)
		{
			m_meshStripGeneratorsArray[i].GenerateMeshStrip(m_stripsWidthVerticesCount, m_stripsWidthVerticesScale, 1.0f);
			m_meshStripGeneratorsGOArray[i].SetActive(false);
		}

		// for initing arrays with legit values
		m_lastGeneratedMeshStrip_FrontRowVerticesArray = m_meshStripGeneratorsArray[0].GetFrontRowVertices();
		t_calcFrontRowVertsArray = new Vector3[m_lastGeneratedMeshStrip_FrontRowVerticesArray.Length];
		t_calcFrontRowVertsArray = m_meshStripGeneratorsArray[0].GetFrontRowVertices();
		m_meshStripGeneratorsGOArray[0].SetActive(true);
	}

	void SpawnMeshStrip(int stripIndex, float stripDistanceFromPrevious)
	{
		m_meshStripGeneratorsGOArray[stripIndex].SetActive(true);
		// set vertices here
		for(int i = 0; i < t_calcFrontRowVertsArray.Length; i++)
		{
			t_calcFrontRowVertsArray[i] = m_lastGeneratedMeshStrip_FrontRowVerticesArray[i] + transform.forward * stripDistanceFromPrevious ;
		}
		m_meshStripGeneratorsArray[stripIndex].SetRowsVertices(t_calcFrontRowVertsArray, m_lastGeneratedMeshStrip_FrontRowVerticesArray);
		m_meshStripGeneratorsGOArray[stripIndex].transform.localPosition = Vector3.zero;
		m_meshStripGeneratorsGOArray[stripIndex].transform.localRotation = Quaternion.identity;
		m_meshStripGeneratorsGOArray[stripIndex].transform.localScale = Vector3.one;

		m_lastActivatedStripIndex = stripIndex;
		m_lastGeneratedMeshStrip_FrontRowVerticesArray = m_meshStripGeneratorsArray[stripIndex].GetFrontRowVertices();
	}


	void Update()
	{
		float travelledDistance = Vector3.Distance(transform.position, t_previousPosition);
		int nextStripSpawnIndex = (m_lastActivatedStripIndex + 1) % m_meshStripsPoolCount;
		SpawnMeshStrip(nextStripSpawnIndex, travelledDistance);

		transform.Translate( transform.forward * m_moveSpeed * Time.deltaTime );
		t_previousPosition = transform.position;
	}

}
