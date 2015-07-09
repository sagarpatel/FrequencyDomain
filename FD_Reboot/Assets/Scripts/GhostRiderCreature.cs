using UnityEngine;
using System.Collections;

public class GhostRiderCreature : MonoBehaviour 
{
	GameObject m_headPart;
	GameObject[] m_bodyPartsArray;

	Material m_headPartMaterial;
	Material[] m_bodyPartsMaterialsArray;

	Vector4[] m_movementsDataArray;
	Color[] m_colorsDataArray;
	Quaternion[] m_rotationsArray;

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
	float m_raiseAnimationDuration = 0.5f;
	float m_explosionAnimationDuration = 1.0f;
	float m_explosionSpeed_Start = 500.0f;
	float m_explosionSpeed_End = 3000.0f;

	GhostRiderCreaturesGenerator m_ghostRiderCreatureGenerator;

	public void InitializeGhostRiderCreature(GhostRiderCreaturesGenerator generator, MeshTerrainGenerator meshTerrainGenerator, Vector4[] movementsDataArray, Color[] colorDataArray, Quaternion[] riderCameraRotationsArray, GameObject headPartPrefab, GameObject bodyPartPrefab, int bodyPartsCount)
	{
		m_ghostRiderCreatureGenerator = generator;
		m_meshTerrainGenerator = meshTerrainGenerator;

		m_headPart = (GameObject)Instantiate(headPartPrefab);
		m_headPart.transform.parent = transform;
		m_headPart.transform.localPosition = Vector3.zero;
		m_headPart.transform.localRotation = Quaternion.identity;

		m_bodyPartsArray = new GameObject[bodyPartsCount];
		m_movementsDataArray = movementsDataArray;
		m_colorsDataArray = colorDataArray;
		m_rotationsArray = riderCameraRotationsArray;

		for(int i = 0; i < bodyPartsCount; i++)
		{
			GameObject bodyPart = (GameObject)Instantiate(bodyPartPrefab);
			bodyPart.name += "_" + i;
			bodyPart.transform.parent = transform;
			bodyPart.transform.position = m_headPart.transform.position;
			bodyPart.transform.rotation = m_headPart.transform.rotation;

			m_bodyPartsArray[i] = bodyPart;
		}

		m_headPartMaterial = m_headPart.GetComponent<MeshRenderer>().material;
		m_bodyPartsMaterialsArray = new Material[m_bodyPartsArray.Length];
		for(int i = 0; i < m_bodyPartsArray.Length; i++)
			m_bodyPartsMaterialsArray[i] = m_bodyPartsArray[i].GetComponent<MeshRenderer>().material;

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
			m_headPart.transform.rotation = m_rotationsArray[m_moveCounter];  //Quaternion.Inverse( rot ) * m_rotationsArray[m_moveCounter]; 
			m_headPartMaterial.color = m_colorsDataArray[m_moveCounter];
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
		
		// main animation complete, now moving the creature to the front over fixed amount of time
		Vector3 upVectorAtFront = Vector3.up;
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

			upVectorAtFront = rot * Vector3.up;

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
		float raiseTimeCounter = 0;
		float targetHeight = 10.0f * (float)m_bodyPartsArray.Length;
		Vector3 headBasePos = m_headPart.transform.position;
		while(raiseTimeCounter < m_raiseAnimationDuration)
		{
			float step = raiseTimeCounter/m_raiseAnimationDuration;
			float lerpedHeight = Mathf.Lerp(0, targetHeight, step);

			m_headPart.transform.position = headBasePos + upVectorAtFront * lerpedHeight;

			LerpBodyParts();

			raiseTimeCounter += Time.deltaTime;
			yield return null;
		}

		Debug.Log(gameObject.name + " Raise up complete " + Time.frameCount);

		// do parts explosino
		m_headPart.GetComponent<MeshRenderer>().enabled = false;
		Vector3[] partsRandomDirectionArray = new Vector3[m_bodyPartsArray.Length];
		for(int i = 0; i < partsRandomDirectionArray.Length; i++)
			partsRandomDirectionArray[i] = Random.onUnitSphere;

		float explosionTimeCounter = 0;
		while(explosionTimeCounter < m_explosionAnimationDuration)
		{
			float progress = explosionTimeCounter/m_explosionAnimationDuration;
			float step = m_ghostRiderCreatureGenerator.m_partsExplosionSpeedCurve.Evaluate(progress);
			float lerpedSpeed = Mathf.Lerp(m_explosionSpeed_Start, m_explosionSpeed_End, step);
			for(int i = 0; i < m_bodyPartsArray.Length; i++)
				m_bodyPartsArray[i].transform.position += partsRandomDirectionArray[i] * lerpedSpeed * Time.deltaTime;

			explosionTimeCounter += Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}

	void LerpBodyParts()
	{
		m_bodyPartsArray[0].transform.position = Vector3.Lerp( m_bodyPartsArray[0].transform.position, m_headPart.transform.position, m_bodyPartMoveStepScaler * Time.deltaTime);
		m_bodyPartsArray[0].transform.rotation = Quaternion.Slerp( m_bodyPartsArray[0].transform.rotation, m_headPart.transform.rotation, m_bodyPartMoveStepScaler * Time.deltaTime);
		m_bodyPartsMaterialsArray[0].color = Color.Lerp( m_bodyPartsMaterialsArray[0].color, m_headPartMaterial.color, m_bodyPartMoveStepScaler * Time.deltaTime);
		for(int i = 1; i < m_bodyPartsArray.Length; i++)
		{
			m_bodyPartsArray[i].transform.position = Vector3.Lerp( m_bodyPartsArray[i].transform.position, m_bodyPartsArray[i-1].transform.position, m_bodyPartMoveStepScaler * Time.deltaTime);
			m_bodyPartsArray[i].transform.rotation = Quaternion.Slerp( m_bodyPartsArray[i].transform.rotation, m_bodyPartsArray[i-1].transform.rotation, m_bodyPartMoveStepScaler * Time.deltaTime);
			m_bodyPartsMaterialsArray[i].color = Color.Lerp( m_bodyPartsMaterialsArray[i].color, m_bodyPartsMaterialsArray[i-1].color, m_bodyPartMoveStepScaler * Time.deltaTime );
		}
	}


}
