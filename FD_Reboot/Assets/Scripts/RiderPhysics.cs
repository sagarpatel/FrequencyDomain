﻿using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{
	float m_currentHeightOffset = 0;
	float m_currentProgressOnTerrain_Depth = 0; // 0 is front of terrain, 1 is the furthest back
	float m_currentProgressOnTerrain_Widrg = 0; // -1 is full left, +1 is full right
	
	MeshTerrainGenerator m_meshTerrainGenerator;
	
	[Range(-1,1)]
	public float d_widthTEst = 0.0f;
	
	[Range(0,1)]
	public float d_DepthTest = 0.0f;
	
	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
		
	}
	
	void Update()
	{
		
		UpdateRiderWolrdPosRot(d_DepthTest, d_widthTEst);
		
	}
	
	void UpdateRiderWolrdPosRot(float terrainRatio_Depth, float terrainRatio_Width)
	{
		int meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
		int frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;
		
		int targetMeshStripIndexOffset = (int)(terrainRatio_Depth * (float)meshStripsCount);
		int targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
		MeshStripGenerator targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];
		
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;
		targetMeshStripGenerator.CalculatePositionOnStrip( terrainRatio_Width, m_currentHeightOffset, out pos, out rot);
		
		transform.position = pos;
		transform.rotation = rot;
	}
}
