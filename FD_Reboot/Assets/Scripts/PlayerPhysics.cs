using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour 
{
	float currentHeight;
	float m_currentProgressOnTerrain_Depth = 0; // 0 is front of terrain, 1 is the furthest back
	float m_currentProgressOnTerrain_Widrg = 0; // -1 is full left, +1 is full right

	MeshTerrainGenerator m_meshTerrainGenerator;

	[Range(-1,1)]
	public float d_widthTEst = 0.0f;

	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();

	}

	void Update()
	{

		CalculateMinHeight(0,0);

	}

	void CalculateMinHeight(float terrainRatio_Depth, float terrainRatio_Width)
	{
		int meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
		int frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;

		int targetMeshStripIndexOffset = (int)(terrainRatio_Depth * (float)meshStripsCount);
		int targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
		MeshStripGenerator targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];

		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		targetMeshStripGenerator.CalculatePositionOnStrip( d_widthTEst,0, out pos, out rot);

		transform.position = pos;
		transform.rotation = rot;
	}



}
