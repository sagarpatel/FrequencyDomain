using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour 
{
	float currentHeight;
	float m_currentProgressOnTerrain_Depth = 0; // 0 is front of terrain, 1 is the furthest back
	float m_currentProgressOnTerrain_Widrg = 0; // -1 is full left, +1 is full right

	MeshTerrainGenerator m_meshTerrainGenerator;

	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();

	}

	void Update()
	{



	}



}
