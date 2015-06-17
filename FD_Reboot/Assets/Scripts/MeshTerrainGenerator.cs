using UnityEngine;
using System.Collections;
using System.Linq;

public class MeshTerrainGenerator : MonoBehaviour 
{
	GameObject[] m_meshStripGeneratorsGOArray;
	MeshStripGenerator[] m_meshStripGeneratorsArray;
	int m_meshStripsPoolCount = 300;
	int m_lastActivatedStripIndex = 0;
	float m_distanceTravelledLastFrame = 0;
	float m_moveSpeed = 60.0f;
	Vector3 t_previousPosition;

	int m_stripsWidthVerticesCount = 256;
	float m_stripsWidthVerticesScale = 0.5f;

	Vector3[] m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right;
	Vector3[] m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left;
	Quaternion m_lastGeneratedMeshStrip_Rotation;
	Transform m_lastGeneratedMeshStrip_Transform;
	Vector3[] t_calcFrontRowVertsArray_Right; // used as buffer to perform calculations on
	Vector3[] t_calcFrontRowVertsArray_Left;
	Vector3[] t_calcBackRowVertsArray_Right;
	Vector3[] t_calcBackRowVertsArray_Left;
	Quaternion t_diffQuaternion;

	public Material m_meshStripsMaterial;

	float[] m_freshHeighValues_Right;
	float[] m_freshHeighValues_Left;

	float[] testHeightValues;

	public float d_rotVel_x = 0;
	public float d_rotVel_y = 0;
	public float d_rotVel_z = 0;
	[Range(-1,1)]
	public float d_bendFactor = 0;
	float m_circleShapeDiameter;

	void Start()
	{
		GameObject meshStripsHolder = new GameObject("MeshStripsHolder");
		m_meshStripGeneratorsGOArray = new GameObject[m_meshStripsPoolCount];
		for(int i = 0; i < m_meshStripGeneratorsGOArray.Length; i++)
		{
			m_meshStripGeneratorsGOArray[i] = new GameObject();
			m_meshStripGeneratorsGOArray[i].name = "MeshStripGenerator_" + i.ToString();
			m_meshStripGeneratorsGOArray[i].transform.parent = meshStripsHolder.transform;
			m_meshStripGeneratorsGOArray[i].transform.localPosition = Vector3.zero;
			m_meshStripGeneratorsGOArray[i].transform.localRotation = Quaternion.identity;
			m_meshStripGeneratorsGOArray[i].transform.localScale = Vector3.one;
			m_meshStripGeneratorsGOArray[i].AddComponent<MeshStripGenerator>();
		}
		m_meshStripGeneratorsArray = m_meshStripGeneratorsGOArray.Select(g => g.GetComponent<MeshStripGenerator>()).ToArray();

		for(int i = 0; i < m_meshStripGeneratorsGOArray.Length; i++)
		{
			m_meshStripGeneratorsArray[i].GenerateMeshStrip(m_stripsWidthVerticesCount, m_stripsWidthVerticesScale, 0.0f, m_meshStripsMaterial);
			//m_meshStripGeneratorsGOArray[i].SetActive(false);
		}

		// for initing arrays with legit values
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right = m_meshStripGeneratorsArray[0].GetFrontRowVertices_Right();
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left = m_meshStripGeneratorsArray[0].GetFrontRowVertices_Left();

		t_calcFrontRowVertsArray_Right = new Vector3[m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right.Length];
		t_calcFrontRowVertsArray_Left = new Vector3[m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left.Length];
		t_calcBackRowVertsArray_Right = new Vector3[t_calcFrontRowVertsArray_Right.Length];
		t_calcBackRowVertsArray_Left = new Vector3[t_calcFrontRowVertsArray_Left.Length];
		//t_calcFrontRowVertsArray = m_meshStripGeneratorsArray[0].GetFrontRowVertices(); // this just made a refernece, need pure local copy
		for(int i = 0; i < t_calcFrontRowVertsArray_Right.Length; i++)
		{
			t_calcFrontRowVertsArray_Right[i] = new Vector3();
			t_calcBackRowVertsArray_Right[i] = new Vector3();
			t_calcFrontRowVertsArray_Left[i] = new Vector3();
			t_calcBackRowVertsArray_Left[i] = new Vector3();
		}
		m_lastGeneratedMeshStrip_Rotation = m_meshStripGeneratorsGOArray[0].transform.rotation;
		m_lastGeneratedMeshStrip_Transform = m_meshStripGeneratorsGOArray[0].transform;
		m_meshStripGeneratorsGOArray[0].SetActive(true);

		meshStripsHolder.SetActive(false);
		meshStripsHolder.SetActive(true);

		m_freshHeighValues_Right = new float[m_stripsWidthVerticesCount];
		m_freshHeighValues_Left = new float[m_stripsWidthVerticesCount];
		testHeightValues = new float[m_stripsWidthVerticesCount];
		for(int i = 0; i < m_freshHeighValues_Right.Length; i++)
		{
			m_freshHeighValues_Right[i] = 0;
			m_freshHeighValues_Left[i] = 0;
			testHeightValues[i] = 0;
		}

		m_circleShapeDiameter = (m_stripsWidthVerticesCount-1) * m_stripsWidthVerticesScale;
	}

	public void UpdateHeighValues(float[] heightsArray_Right, float[] heightsArray_Left)
	{
		for(int i = 0; i < m_freshHeighValues_Right.Length; i++)
		{
			m_freshHeighValues_Right[i] = heightsArray_Right[i];
			m_freshHeighValues_Left[i] = heightsArray_Left[i];
		}
	}
	
	void SpawnMeshStrip(int stripIndex)//, float stripDistanceFromPrevious)
	{
		m_meshStripGeneratorsGOArray[stripIndex].SetActive(true);
		
		m_meshStripGeneratorsGOArray[stripIndex].transform.position = transform.position;
		m_meshStripGeneratorsGOArray[stripIndex].transform.rotation = transform.rotation;
		m_meshStripGeneratorsGOArray[stripIndex].transform.localScale = Vector3.one;
		
		t_diffQuaternion = transform.rotation * Quaternion.Inverse( m_lastGeneratedMeshStrip_Rotation); 
		Vector3 vertexWorldPos_Right;
		Vector3 vertexWorldPos_Left;
		Vector3 generatedFrontRowVertex_Right;
		Vector3 generatedFrontRowVertex_Left;
		// set vertices here
		for(int i = 0; i < t_calcFrontRowVertsArray_Right.Length; i++)
		{
			generatedFrontRowVertex_Right = GenerateFrontRowBaselineVertex(i,d_bendFactor);
			generatedFrontRowVertex_Left = generatedFrontRowVertex_Right;
			generatedFrontRowVertex_Left.x = - generatedFrontRowVertex_Left.x;
			
			t_calcFrontRowVertsArray_Right[i] = generatedFrontRowVertex_Right + Vector3.up * m_freshHeighValues_Right[i]; //m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i] + transform.up * m_freshHeighValues_Right[i];// + transform.forward * stripDistanceFromPrevious;
			vertexWorldPos_Right = m_lastGeneratedMeshStrip_Transform.TransformPoint(m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i]);
			t_calcBackRowVertsArray_Right[i] = m_meshStripGeneratorsGOArray[stripIndex].transform.InverseTransformPoint(vertexWorldPos_Right) ; //t_diffQuaternion * m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i];
			
			t_calcFrontRowVertsArray_Left[i] = generatedFrontRowVertex_Left + Vector3.up * m_freshHeighValues_Left[i]; //m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i] + transform.up * m_freshHeighValues_Left[i];// + transform.forward * stripDistanceFromPrevious;
			vertexWorldPos_Left = m_lastGeneratedMeshStrip_Transform.TransformPoint(m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i]);
			t_calcBackRowVertsArray_Left[i] = m_meshStripGeneratorsGOArray[stripIndex].transform.InverseTransformPoint(vertexWorldPos_Left) ; //t_diffQuaternion * m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i];
		}
		m_meshStripGeneratorsArray[stripIndex].SetRowsVertices_Right(t_calcFrontRowVertsArray_Right, t_calcBackRowVertsArray_Right);
		m_meshStripGeneratorsArray[stripIndex].SetRowsVertices_Left(t_calcFrontRowVertsArray_Left, t_calcBackRowVertsArray_Left);
		
		m_lastActivatedStripIndex = stripIndex;
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right = m_meshStripGeneratorsArray[stripIndex].GetFrontRowVertices_Right();
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left = m_meshStripGeneratorsArray[stripIndex].GetFrontRowVertices_Left();
		m_lastGeneratedMeshStrip_Rotation = m_meshStripGeneratorsGOArray[stripIndex].transform.rotation;
		m_lastGeneratedMeshStrip_Transform = m_meshStripGeneratorsGOArray[stripIndex].transform;
	}
	
	Vector3 GenerateFrontRowBaselineVertex(int collumnIndex, float bendFactor)
	{
		float centerToEdgeRatio = (float)collumnIndex /(m_stripsWidthVerticesCount - 1); // making sure that final index gets ratio of 1

		Vector3 flatPos = Vector3.zero;
		flatPos.x = m_circleShapeDiameter * centerToEdgeRatio; //(float)collumnIndex * m_stripsWidthVerticesScale;
		flatPos.y = 0;

		Vector2 unitCirclePos = CalculatePosOnUnitCircle(centerToEdgeRatio);
		Vector3 circleFormPos = Vector3.zero;
		circleFormPos.x = m_circleShapeDiameter * unitCirclePos.x;
		circleFormPos.y = m_circleShapeDiameter * (unitCirclePos.y + 1.0f);

		Vector3 halfCircleTransitionPos = Vector3.zero;
		halfCircleTransitionPos.x = m_circleShapeDiameter * Mathf.Cos( bendFactor * 0.5f * Mathf.PI);
		halfCircleTransitionPos.y = m_circleShapeDiameter * Mathf.Sin( Mathf.Sqrt( bendFactor ) * 0.5f * Mathf.PI);

		//Vector3 circleTransitionFormPos = Vector3.Lerp(circleFormPos, halfCircleTransitionPos, centerToEdgeRatio);

		Vector3 lerpedPos = Vector3.Lerp(flatPos, circleFormPos, bendFactor);
		return lerpedPos;
	}

	Vector2 CalculatePosOnUnitCircle(float startToEndRatio)
	{
		float angle = Mathf.Lerp( -0.5f * Mathf.PI, 0.5f * Mathf.PI, startToEndRatio);
		float pos_x = Mathf.Cos(angle);
		float pos_y = Mathf.Sin(angle);
		return new Vector2(pos_x, pos_y);
	}

	void FixedUpdate()
	{
		for(int i = 0; i < testHeightValues.Length; i++)
		{
			//testHeightValues[i] = 0.250f * (float)i * Mathf.Sin( Mathf.Sin(0.009f * (float)i) *  Time.time);
			//testHeightValues[i] = Mathf.Sin(Time.time);// + 1.0f*(float)i/(float)testHeightValues.Length);
		}
		UpdateHeighValues(testHeightValues, testHeightValues);

		t_previousPosition = transform.position;
		transform.position += transform.forward * m_moveSpeed * Time.deltaTime;

		float travelledDistance = Vector3.Distance(transform.position, t_previousPosition);
		int nextStripSpawnIndex = (m_lastActivatedStripIndex + 1) % m_meshStripsPoolCount;
		SpawnMeshStrip(nextStripSpawnIndex);//, travelledDistance);

		transform.Rotate(new Vector3(d_rotVel_x, d_rotVel_y , d_rotVel_z) * Time.deltaTime);

	}

}
