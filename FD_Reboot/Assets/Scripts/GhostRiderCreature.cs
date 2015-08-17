using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostRiderCreature : MonoBehaviour 
{
	GameObject m_headPart;
	GameObject[] m_bodyPartsArray;

	Material m_headPartMaterial;
	Material[] m_bodyPartsMaterialsArray;

	Vector4[] m_movementsDataArray;
	Color[] m_colorsDataArray;
	Quaternion[] m_rotationsArray;

	MeshTerrainGenerator m_meshTerrainGenerator;

	float m_riderDataDepthRatio = 0;	
	float m_riderDataWidthRatio = 0;
	float m_riderDataHeightOffsetTerrain = 0;	
	float m_riderDataBarrelRollAngle = 0;
	
	float m_ghostDepthRatio = 0;
	float m_ghostDepthSpeed = 0.08f;
	float m_ghostSpeedAccumulator = 0;
	float m_bodyPartMoveStepScaler = 15.0f;
	float m_moveToFrontAnimationDuration = 1.0f;
	float m_headToTailDistanceTriggerThreashold = 1.0f;
	float m_raiseAnimationDuration = 0.5f;
	float m_explosionAnimationDuration = 1.0f;
	float m_explosionSpeed_Start = 500.0f;
	float m_explosionSpeed_End = 3000.0f;
	float m_ghostMovementAnimationDuration = 0;

	GhostRiderCreaturesGenerator m_ghostRiderCreatureGenerator;

	public void InitializeGhostRiderCreature(GhostRiderCreaturesGenerator generator, MeshTerrainGenerator meshTerrainGenerator, Vector4[] movementsDataArray, Color[] colorDataArray, Quaternion[] riderCameraRotationsArray, GameObject headPartPrefab, GameObject bodyPartPrefab, int bodyPartsCount, float moveAnimationDuration)
	{
		// copy params locally
		m_ghostRiderCreatureGenerator = generator;
		m_meshTerrainGenerator = meshTerrainGenerator;
		m_movementsDataArray = movementsDataArray;
		m_colorsDataArray = colorDataArray;
		m_rotationsArray = riderCameraRotationsArray;
		m_ghostMovementAnimationDuration = moveAnimationDuration;

		// create parts
		m_headPart = (GameObject)Instantiate(headPartPrefab);
		m_headPart.transform.parent = transform;
		m_headPart.transform.localPosition = Vector3.zero;
		m_headPart.transform.localRotation = Quaternion.identity;
		SetBarycentricPoints_CubeMesh(m_headPart.GetComponent<MeshFilter>().mesh);

		m_bodyPartsArray = new GameObject[bodyPartsCount];

		for(int i = 0; i < bodyPartsCount; i++)
		{
			GameObject bodyPart = (GameObject)Instantiate(bodyPartPrefab);
			bodyPart.name += "_" + i;
			bodyPart.transform.parent = transform;
			bodyPart.transform.position = m_headPart.transform.position;
			bodyPart.transform.rotation = m_headPart.transform.rotation;
			SetBarycentricPoints_CubeMesh(bodyPart.GetComponent<MeshFilter>().mesh);

			m_bodyPartsArray[i] = bodyPart;
		}

		m_headPartMaterial = m_headPart.GetComponent<MeshRenderer>().material;
		m_bodyPartsMaterialsArray = new Material[m_bodyPartsArray.Length];
		for(int i = 0; i < m_bodyPartsArray.Length; i++)
			m_bodyPartsMaterialsArray[i] = m_bodyPartsArray[i].GetComponent<MeshRenderer>().material;

		StartCoroutine(AnimateGhostRiderCreature());
	}

	// curent implentation is just a copy of the the pattern used for the mesh strips
	void SetBarycentricPoints_CubeMesh(Mesh cubeMesh)
	{
		Vector3[] meshVertices = cubeMesh.vertices;
		int[] trianglesIndicesArray = cubeMesh.triangles;
		// set up barycentric coordiantes points per vertex, laid out so that each triangle has own set
		Vector4[] barrycenterPoints = new Vector4[meshVertices.Length];
		Vector4 b_coord_1 = new Vector4(1,0,0,1);
		Vector4 b_coord_2 = new Vector4(0,1,0,1);
		Vector4 b_coord_3 = new Vector4(0,0,1,1);

		// figured out Unity's indices layout pattern, assiging barrycentric points accordingly
		for(int i = 0; i < trianglesIndicesArray.Length; i++)
		{
			// processing 1 face at a time --> 2 triangles --> 6 indices
			int vertexIndex = trianglesIndicesArray[i];
			if(i % 6 == 0)
				barrycenterPoints[vertexIndex] = b_coord_1;
			else if(i % 6 == 1)
				barrycenterPoints[vertexIndex] = b_coord_2;
			else if(i % 6 == 2)
				barrycenterPoints[vertexIndex] = b_coord_3;
			else if(i % 6 == 3)
				barrycenterPoints[vertexIndex] = b_coord_1;
			else if(i % 6 == 4)
				barrycenterPoints[vertexIndex] = b_coord_3;
			else if(i % 6 == 5)
				barrycenterPoints[vertexIndex] = b_coord_2;
		}

		cubeMesh.tangents = barrycenterPoints;
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

		float ghostDataAnimationTimeCounter = 0;

		while(ghostDataAnimationTimeCounter < m_ghostMovementAnimationDuration)
		{			
			meshStripsCount = m_meshTerrainGenerator.m_meshStripsPoolCount;
			frontStripIndex = m_meshTerrainGenerator.m_lastActivatedStripIndex;	

			float progress = ghostDataAnimationTimeCounter/m_ghostMovementAnimationDuration;
			float dataIndexLocation = progress * (float)(m_movementsDataArray.Length - 1);
			int dataIndexFloor = Mathf.FloorToInt(dataIndexLocation);
			int dataIndexCeil = Mathf.CeilToInt(dataIndexLocation);
			float dataIndexLerpStep = dataIndexLocation - (float)dataIndexFloor;

			//Debug.Log("Array length: " + m_movementsDataArray.Length + " ceil index: " + dataIndexCeil + " progress: " + progress);

			Vector4 lerpedRiderMovementData = Vector4.Lerp(m_movementsDataArray[dataIndexFloor], m_movementsDataArray[dataIndexCeil], dataIndexLerpStep);
			m_riderDataDepthRatio = lerpedRiderMovementData.x;
			m_riderDataWidthRatio = lerpedRiderMovementData.y;
			m_riderDataHeightOffsetTerrain = lerpedRiderMovementData.z;
			m_riderDataBarrelRollAngle = lerpedRiderMovementData.w; // looks like im not using this now since I'm passing the entire rotation in the other array

			Quaternion lerpedCameraRotation = Quaternion.Slerp(m_rotationsArray[dataIndexFloor], m_rotationsArray[dataIndexCeil], dataIndexLerpStep);
			Color lerpedColor = Color.Lerp(m_colorsDataArray[dataIndexFloor], m_colorsDataArray[dataIndexCeil], dataIndexLerpStep);

			m_ghostSpeedAccumulator += m_ghostDepthSpeed * Time.deltaTime;
			m_ghostDepthRatio = Mathf.Clamp( m_riderDataDepthRatio + m_ghostSpeedAccumulator, 0, 1);

			targetMeshStripIndexOffset = (int)( m_ghostDepthRatio * (float)meshStripsCount);
			targetMeshIndex = (frontStripIndex + targetMeshStripIndexOffset) % meshStripsCount;
			targetMeshStripGenerator = m_meshTerrainGenerator.m_meshStripGeneratorsArray[ targetMeshIndex ];

			targetMeshStripGenerator.CalculatePositionOnStrip_Ghost( m_riderDataWidthRatio, m_riderDataHeightOffsetTerrain, out pos, out rot);

			m_headPart.transform.position = pos;
			m_headPart.transform.rotation = lerpedCameraRotation; 
			m_headPartMaterial.color = lerpedColor;
			LerpBodyParts();

			// first pos/rot etc set
			if(ghostDataAnimationTimeCounter == 0)
			{
				for(int i = 1; i < m_bodyPartsArray.Length; i++)
				{
					m_bodyPartsArray[i].transform.position = m_headPart.transform.position;
					m_bodyPartsArray[i].transform.rotation = m_headPart.transform.rotation;
				}
			}

			ghostDataAnimationTimeCounter += Time.deltaTime;
			yield return null;
		}

		//Debug.Log(gameObject.name + " Main animation complete " + Time.frameCount);
		
		// main animation complete, now moving the creature to the front over fixed amount of time
		Vector3 upVectorAtFront = Vector3.up;
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

		//Debug.Log(gameObject.name + " Parts catch up complete " + Time.frameCount);
		
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
