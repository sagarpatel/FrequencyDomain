using UnityEngine;
using System.Collections;
using System.Linq;

public class MeshTerrainGenerator : MonoBehaviour 
{
	GameObject[] m_meshStripGeneratorsGOArray;
	public MeshStripGenerator[] m_meshStripGeneratorsArray;
	public int m_meshStripsPoolCount = 300;
	public int m_lastActivatedStripIndex = 0;
	float m_distanceTravelledLastFrame = 0;
	float m_moveSpeed = 250.0f;
	Vector3 t_previousPosition;

	int m_stripsWidthVerticesCount = 256;
	float m_stripsWidthVerticesScale = 3.5f;

	Vector3[] m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right;
	Vector3[] m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left;
	Quaternion m_lastGeneratedMeshStrip_Rotation;
	Transform m_lastGeneratedMeshStrip_Transform;
	Vector3[] t_calcFrontRowVertsArray_Right; // used as buffer to perform calculations on
	Vector3[] t_calcFrontRowVertsArray_Left;
	Vector3[] t_calcBackRowVertsArray_Right;
	Vector3[] t_calcBackRowVertsArray_Left;
	Vector3[] t_stripUpVectorsArray_Right;
	Vector3[] t_stripUpVectorsArray_Left;
	Quaternion t_diffQuaternion;
	MeshStripGenerator m_lastGeneratedMeshStrip;

	public Material m_meshStripsMaterial;

	float[] m_freshHeighValues_Right;
	float[] m_freshHeighValues_Left;

	float[] testHeightValues;

	float m_bendFactor = 0;

	float m_stripHalfWidth;
	float m_circleFormRadius;
	Vector3 m_circleCenterPos;
	Color m_currentMaterialColor;

	FrequencyDataManager m_frequencyDataManager;

	GameObject d_circleCenterObject;

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
		t_stripUpVectorsArray_Right = new Vector3[t_calcFrontRowVertsArray_Left.Length];
		t_stripUpVectorsArray_Left = new Vector3[t_calcFrontRowVertsArray_Left.Length];
		//t_calcFrontRowVertsArray = m_meshStripGeneratorsArray[0].GetFrontRowVertices(); // this just made a refernece, need pure local copy
		for(int i = 0; i < t_calcFrontRowVertsArray_Right.Length; i++)
		{
			t_calcFrontRowVertsArray_Right[i] = new Vector3();
			t_calcBackRowVertsArray_Right[i] = new Vector3();
			t_calcFrontRowVertsArray_Left[i] = new Vector3();
			t_calcBackRowVertsArray_Left[i] = new Vector3();
			t_stripUpVectorsArray_Left[i] = new Vector3();
			t_stripUpVectorsArray_Right[i] = new Vector3();
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

		m_stripHalfWidth = (m_stripsWidthVerticesCount-1) * m_stripsWidthVerticesScale;
		m_circleFormRadius = m_stripHalfWidth/Mathf.PI;
		m_circleCenterPos = new Vector3(0, m_circleFormRadius, 0);

		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();

		d_circleCenterObject = new GameObject("Circle Center");
		d_circleCenterObject.transform.parent = transform;
		d_circleCenterObject.transform.localPosition = m_circleCenterPos;

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
		Vector3 vertexUpVector_Right;
		Vector3 vertexUpVector_Left;
		Vector3 circleFormUp;

		// set vertices here
		for(int i = 0; i < t_calcFrontRowVertsArray_Right.Length; i++)
		{
			generatedFrontRowVertex_Right = GenerateFrontRowBaselineVertex(i,m_bendFactor);
			generatedFrontRowVertex_Left = generatedFrontRowVertex_Right;
			generatedFrontRowVertex_Left.x *= -1;

			m_circleCenterPos.y = Mathf.Sign(m_bendFactor) * Mathf.Abs(m_circleCenterPos.y);
			if(m_circleCenterPos.y > 0)
				circleFormUp = (m_circleCenterPos - generatedFrontRowVertex_Right).normalized;
			else
				circleFormUp = (generatedFrontRowVertex_Right - m_circleCenterPos).normalized;

			vertexUpVector_Right = Vector3.Lerp(Vector3.up, circleFormUp, Mathf.Abs( m_bendFactor ));

			vertexUpVector_Left = vertexUpVector_Right;
			vertexUpVector_Left.x *= -1;
			
			t_calcFrontRowVertsArray_Right[i] = generatedFrontRowVertex_Right + vertexUpVector_Right * m_freshHeighValues_Right[i]; //m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i] + transform.up * m_freshHeighValues_Right[i];// + transform.forward * stripDistanceFromPrevious;
			vertexWorldPos_Right = m_lastGeneratedMeshStrip_Transform.TransformPoint(m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i]);
			t_calcBackRowVertsArray_Right[i] = m_meshStripGeneratorsGOArray[stripIndex].transform.InverseTransformPoint(vertexWorldPos_Right) ; //t_diffQuaternion * m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i];
			
			t_calcFrontRowVertsArray_Left[i] = generatedFrontRowVertex_Left + vertexUpVector_Left * m_freshHeighValues_Left[i]; //m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i] + transform.up * m_freshHeighValues_Left[i];// + transform.forward * stripDistanceFromPrevious;
			vertexWorldPos_Left = m_lastGeneratedMeshStrip_Transform.TransformPoint(m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i]);
			t_calcBackRowVertsArray_Left[i] = m_meshStripGeneratorsGOArray[stripIndex].transform.InverseTransformPoint(vertexWorldPos_Left) ; //t_diffQuaternion * m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i];
		
			t_stripUpVectorsArray_Right[i] = vertexUpVector_Right;
			t_stripUpVectorsArray_Left[i] = vertexUpVector_Left;
		}
		m_meshStripGeneratorsArray[stripIndex].SetRowsVertices_Right(t_calcFrontRowVertsArray_Right, t_calcBackRowVertsArray_Right, t_stripUpVectorsArray_Right, m_freshHeighValues_Right, m_bendFactor);
		m_meshStripGeneratorsArray[stripIndex].SetRowsVertices_Left(t_calcFrontRowVertsArray_Left, t_calcBackRowVertsArray_Left, t_stripUpVectorsArray_Left, m_freshHeighValues_Left, m_bendFactor);
		
		m_lastActivatedStripIndex = stripIndex;
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right = m_meshStripGeneratorsArray[stripIndex].GetFrontRowVertices_Right();
		m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left = m_meshStripGeneratorsArray[stripIndex].GetFrontRowVertices_Left();
		m_lastGeneratedMeshStrip_Rotation = m_meshStripGeneratorsGOArray[stripIndex].transform.rotation;
		m_lastGeneratedMeshStrip_Transform = m_meshStripGeneratorsGOArray[stripIndex].transform;

		if(m_lastGeneratedMeshStrip != null)
		{
			m_meshStripGeneratorsArray[stripIndex].AverageNormalsForCommonVertices(m_lastGeneratedMeshStrip.m_mesh_Up_Right, m_lastGeneratedMeshStrip.m_mesh_Up_Left);
		}

		m_lastGeneratedMeshStrip = m_meshStripGeneratorsArray[stripIndex];
	}
	
	Vector3 GenerateFrontRowBaselineVertex(int collumnIndex, float bendFactor)
	{
		float centerToEdgeRatio = (float)collumnIndex /(m_stripsWidthVerticesCount - 1); // making sure that final index gets ratio of 1

		Vector3 flatPos = Vector3.zero;
		flatPos.x = m_stripHalfWidth * centerToEdgeRatio; //(float)collumnIndex * m_stripsWidthVerticesScale;
		flatPos.y = 0;

		Vector2 unitCirclePos = CalculatePosOnUnitCircle(centerToEdgeRatio);
		Vector3 circleFormPos = Vector3.zero;
		circleFormPos.x = m_circleFormRadius * unitCirclePos.x;
		circleFormPos.y = m_circleFormRadius * (unitCirclePos.y + 1.0f);

		Vector3 halfCircleTransitionPos = Vector3.zero;
		halfCircleTransitionPos.x = m_circleFormRadius * Mathf.Cos( bendFactor * 0.5f * Mathf.PI);
		halfCircleTransitionPos.y = m_circleFormRadius * Mathf.Sin( bendFactor * 0.5f * Mathf.PI);

		float distToCenter_halfCircleTransition = Vector3.Distance(halfCircleTransitionPos, m_circleCenterPos);
		float distToCenter_circleForm = Vector3.Distance(circleFormPos, m_circleCenterPos);

		Vector3 circleTransitionFormPos = Vector3.zero;

		// not sure why this isn't working yet 
		/*
		if(distToCenter_circleForm > distToCenter_halfCircleTransition)
			circleTransitionFormPos = circleFormPos;
		else
			circleTransitionFormPos = halfCircleTransitionPos;
		*/
		circleTransitionFormPos = circleFormPos;

		Vector3 lerpedPos = Vector3.Lerp(flatPos, circleTransitionFormPos, Mathf.Abs(bendFactor));
		lerpedPos.y = Mathf.Sign(bendFactor) * lerpedPos.y;
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
		//d_bendFactor = Mathf.Sin( d_bendOscilationFrequency * Time.time);
		Profiler.BeginSample("Get FFT Data");
		float[] dataArray = m_frequencyDataManager.GetFreshFFTData();
		Profiler.EndSample();

		Profiler.BeginSample("Update Height values array");
		int dataLength = dataArray.Length;
		int meshToDataRatio = testHeightValues.Length/ dataLength;
		for(int i = 0; i < testHeightValues.Length; i++)
		{
			//testHeightValues[i] = 0.050f * (float)i * Mathf.Sin( Mathf.Sin(0.009f * (float)i) *  Time.time);
			//testHeightValues[i] = Mathf.Sin(Time.time);// + 1.0f*(float)i/(float)testHeightValues.Length);
			testHeightValues[i] = 400.0f * dataArray[i/meshToDataRatio];
		}
		UpdateHeighValues(testHeightValues, testHeightValues);
		Profiler.EndSample();

		float travelledDistance = Vector3.Distance(transform.position, t_previousPosition);
		int nextStripSpawnIndex = (m_lastActivatedStripIndex + 1) % m_meshStripsPoolCount;

		Profiler.BeginSample("Spawn new mesh strip");
		SpawnMeshStrip(nextStripSpawnIndex);//, travelledDistance);
		Profiler.EndSample();

		Profiler.BeginSample("Get and set new RGB");
		m_currentMaterialColor = m_frequencyDataManager.GetFreshRGB();
		//m_meshStripsMaterial.color = m_currentMaterialColor;
		SetMeshTerrainColor(m_currentMaterialColor);
		float cutoff = 0.02f + 0.1720f * (m_currentMaterialColor.r + m_currentMaterialColor.g +m_currentMaterialColor.b); //0.02f + 0.9733f * m_currentMaterialColor.r; //0.025f + 0.2f * Mathf.Abs(Mathf.Sin(0.95f * Time.time));
		//m_meshStripsMaterial.SetFloat("_Cutoff", cutoff);
		//SetMeshTerrainWireframeValue(cutoff);
		Profiler.EndSample();
		//m_meshStripsMaterial.SetColor("_EmissionColor", m_currentMaterialColor);
	}

	public void SetMeshBendValue(float bendValue)
	{
		m_bendFactor = bendValue;
	}
	
	void SetMeshTerrainColor(Color color)
	{
		m_meshStripGeneratorsArray[m_lastActivatedStripIndex].SetMeshStripColor(color); // testing out strip colors instead of global color
		//for(int i = 0; i < m_meshStripGeneratorsArray.Length; i++)
		//	m_meshStripGeneratorsArray[i].SetMeshStripColor(color);
	}

	void SetMeshTerrainWireframeValue(float wireframeValue)
	{
		//m_meshStripGeneratorsArray[m_lastActivatedStripIndex].SetMeshStripWireframeValue(wireframeValue); // testing out strip wirefrafme
		for(int i = 0; i < m_meshStripGeneratorsArray.Length; i++)
			m_meshStripGeneratorsArray[i].SetMeshStripWireframeValue(wireframeValue);
	}

}
