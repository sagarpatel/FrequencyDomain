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
	float m_moveSpeed = 40.0f;
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

	public Material meshStripsMaterial;

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
			m_meshStripGeneratorsArray[i].GenerateMeshStrip(m_stripsWidthVerticesCount, m_stripsWidthVerticesScale, 0.0f, meshStripsMaterial);
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
		// set vertices here
		for(int i = 0; i < t_calcFrontRowVertsArray_Right.Length; i++)
		{
			t_calcFrontRowVertsArray_Right[i] = m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i];// + transform.forward * stripDistanceFromPrevious;
			vertexWorldPos_Right = m_lastGeneratedMeshStrip_Transform.TransformPoint(m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i]);
			t_calcBackRowVertsArray_Right[i] = m_meshStripGeneratorsGOArray[stripIndex].transform.InverseTransformPoint(vertexWorldPos_Right) ; //t_diffQuaternion * m_lastGeneratedMeshStrip_FrontRowVerticesArray_Right[i];

			t_calcFrontRowVertsArray_Left[i] = m_lastGeneratedMeshStrip_FrontRowVerticesArray_Left[i];// + transform.forward * stripDistanceFromPrevious;
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


	void Update()
	{
		t_previousPosition = transform.position;
		transform.Translate( transform.forward * m_moveSpeed * Time.deltaTime );



		float travelledDistance = Vector3.Distance(transform.position, t_previousPosition);
		int nextStripSpawnIndex = (m_lastActivatedStripIndex + 1) % m_meshStripsPoolCount;
		SpawnMeshStrip(nextStripSpawnIndex);//, travelledDistance);


	}

}
