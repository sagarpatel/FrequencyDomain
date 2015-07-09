﻿using UnityEngine;
using System.Collections;

public class GhostRiderCreature : MonoBehaviour 
{
	GameObject m_headPart;
	GameObject[] m_bodyPartsArray;
	Vector4[] m_movementsDataArray;
	Color[] m_colorsDataArray;

	int m_moveCounter = 0;

	MeshTerrainGenerator m_meshTerrainGenerator;

	float m_riderDataDepthRatio
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].x;
		}
	}

	float m_riderDataWidthRatio
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].y;
		}
	}

	float m_riderDataHeightOffsetTerrain
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].z;
		}
	}

	float m_riderDataBarrelRollAngle
	{
		get
		{
			return m_movementsDataArray[m_moveCounter].w;
		}
	}

	float m_ghostDepthRatio = 0;
	float m_ghostDepthSpeed = 0.120f;
	float m_ghostSpeedAccumulator = 0;
	float m_bodyPartMoveStepScaler = 25.0f;
	float m_moveToFrontAnimationDuration = 1.0f;
	float m_headToTailDistanceTriggerThreashold = 1.0f;

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
			bodyPart.name += "_" + i;
			bodyPart.transform.parent = transform;
			bodyPart.transform.position = m_headPart.transform.position;
			bodyPart.transform.rotation = m_headPart.transform.rotation;

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

			m_ghostSpeedAccumulator += m_ghostDepthSpeed * Time.deltaTime;
			m_ghostDepthRatio = Mathf.Clamp( m_riderDataDepthRatio + m_ghostSpeedAccumulator, 0, 1);

			targetMeshStripIndexOffset = (int)( m_ghostDepthRatio * (float)meshStripsCount);
			targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
			targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];

			targetMeshStripGenerator.CalculatePositionOnStrip_Ghost( m_riderDataWidthRatio, m_riderDataHeightOffsetTerrain, out pos, out rot);

			m_headPart.transform.position = pos;
			m_headPart.transform.rotation = rot;
			LerpBodyParts();

			// first pos/rot etc set
			if(m_moveCounter == 0)
			{
				for(int i = 1; i < m_bodyPartsArray.Length; i++)
				{
					m_bodyPartsArray[i].transform.position = m_headPart.transform.position;
					m_bodyPartsArray[i].transform.rotation = m_headPart.transform.rotation;
				}
			}

			m_moveCounter ++;
			yield return null;
		}

		Debug.Log(gameObject.name + " Main animation complete " + Time.frameCount);
		
		// main animation complete, now moving the crature to the front over fixed amount of time
		m_moveCounter --;
		float moveToFrontTimeCounter = 0;
		while(moveToFrontTimeCounter < m_moveToFrontAnimationDuration)
		{
			meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
			frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;	

			float step = moveToFrontTimeCounter/m_moveToFrontAnimationDuration;
			m_ghostDepthRatio = Mathf.Lerp( m_ghostDepthRatio, 1, step);
			
			targetMeshStripIndexOffset = (int)( m_ghostDepthRatio * (float)meshStripsCount);
			targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
			targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];
			
			targetMeshStripGenerator.CalculatePositionOnStrip_Ghost( m_riderDataWidthRatio, m_riderDataHeightOffsetTerrain, out pos, out rot);
			
			m_headPart.transform.position = pos;
			m_headPart.transform.rotation = rot;
			LerpBodyParts();
			
			moveToFrontTimeCounter += Time.deltaTime;
			yield return null;
		}

		Debug.Log(gameObject.name + "Move to front complete " + Time.frameCount);
		
		// head now at front position, waiting for body parts to catch up
		Transform lastPartTransform = m_bodyPartsArray[m_bodyPartsArray.Length -1].transform;
		while(true)
		{
			float lastPartToHeadDist = Vector3.Distance(m_headPart.transform.position, lastPartTransform.position);
			if(lastPartToHeadDist < m_headToTailDistanceTriggerThreashold)
			{
				break;
			}
			else
			{
				LerpBodyParts();
			}

			yield return null;
		}

		Debug.Log(gameObject.name + " Parts catch up complete " + Time.frameCount);
		
		// do raise and explosion animation


		Destroy(gameObject);
	}

	void LerpBodyParts()
	{
		m_bodyPartsArray[0].transform.position = Vector3.Lerp( m_bodyPartsArray[0].transform.position, m_headPart.transform.position, m_bodyPartMoveStepScaler * Time.deltaTime);
		for(int i = 1; i < m_bodyPartsArray.Length; i++)
		{
			m_bodyPartsArray[i].transform.position = Vector3.Lerp( m_bodyPartsArray[i].transform.position, m_bodyPartsArray[i-1].transform.position, m_bodyPartMoveStepScaler * Time.deltaTime);
			m_bodyPartsArray[i].transform.rotation = Quaternion.Slerp( m_bodyPartsArray[i].transform.rotation, m_bodyPartsArray[i-1].transform.rotation, m_bodyPartMoveStepScaler * Time.deltaTime);
		}
	}


}
