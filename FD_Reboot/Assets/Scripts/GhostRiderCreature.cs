using UnityEngine;
using System.Collections;

public class GhostRiderCreature : MonoBehaviour 
{
	GameObject m_headPart;
	GameObject[] m_bodyPartsArray;
	Vector4[] m_movementsDataArray;
	Color[] m_colorsDataArray;

	int m_moveCounter = 0;

	MeshTerrainGenerator m_meshTerrainGenerator;

	float m_currentDepthRatio
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].x;
		}
	}

	float m_currentWidthRatio
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].y;
		}
	}

	float m_currentHeightOffsetTerrain
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].z;
		}
	}

	float m_currentBarrelRollAngle
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].w;
		}
	}

	public void InitializeGhostRiderCreature(MeshTerrainGenerator meshTerrainGenerator, Vector4[] movementsDataArray, Color[] colorDataArray, GameObject headPartPrefab, GameObject bodyPartPrefab, int bodyPartsCount)
	{
		m_meshTerrainGenerator = meshTerrainGenerator;

		m_headPart = (GameObject)Instantiate(headPartPrefab);
		m_headPart.transform.parent = transform;
		m_headPart.transform.localPosition = Vector3.zero;
		m_headPart.transform.localRotation = Quaternion.identity;

		m_bodyPartsArray = new GameObject[bodyPartsCount];
		m_movementsDataArray = movementsDataArray;
		m_colorsDataArray = m_colorsDataArray;

		for(int i = 0; i < bodyPartsCount; i++)
		{
			GameObject bodyPart = (GameObject)Instantiate(bodyPartPrefab);
			bodyPart.transform.parent = m_headPart.transform;
			bodyPart.transform.localPosition = Vector3.zero;
			bodyPart.transform.localRotation = Quaternion.identity;

			m_bodyPartsArray[i] = bodyPart;
		}

		StartCoroutine(AnimateGhostRiderCreature());
	}

	IEnumerator AnimateGhostRiderCreature()
	{
		Vector3 pos;
		Quaternion rot;
				
		int meshStripsCount;
		int frontStripIndex;
		int targetMeshStripIndexOffset;
		int targetMeshIndex;
		MeshStripGenerator targetMeshStripGenerator;

		while(m_moveCounter < m_movementsDataArray.Length)
		{			
			meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
			frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;		
			targetMeshStripIndexOffset = (int)(m_currentDepthRatio * (float)meshStripsCount);
			targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
			targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];

			targetMeshStripGenerator.CalculatePositionOnStrip( m_currentWidthRatio, m_currentHeightOffsetTerrain, out pos, out rot);

			transform.position = pos;
			transform.rotation = rot;

			m_moveCounter ++;
			yield return null;
		}

		Destroy(gameObject);

	}


}
