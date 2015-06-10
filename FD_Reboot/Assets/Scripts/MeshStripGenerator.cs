using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshStripGenerator : MonoBehaviour 
{
	List<int> m_trianglesList_Up;
	List<int> m_trianglesList_Down;
	List<Vector3> m_normalsList;

	Vector3[] m_verticesArray;
	Vector3[] m_verticesArray_BackRow;
	Vector3[] m_verticesArray_FrontRow;

	int m_widthVerticesCount;

	Mesh m_mesh_Up;
	Mesh m_mesh_Down;


	public void GenerateMeshStrip(int collumnsCount, float collumnWidth, float rowDepth, Material meshMaterial)
	{
		m_widthVerticesCount = collumnsCount;

		GameObject meshStripGO_Up = new GameObject();
		meshStripGO_Up.transform.parent = transform;
		meshStripGO_Up.transform.localPosition = Vector3.zero;
		meshStripGO_Up.transform.localRotation = Quaternion.identity;
		meshStripGO_Up.transform.localScale = Vector3.one;
		meshStripGO_Up.name = transform.name + "_MeshStrip_Up";
		meshStripGO_Up.AddComponent<MeshRenderer>();
		meshStripGO_Up.AddComponent<MeshFilter>();


		GameObject meshStripGO_Down = new GameObject();
		meshStripGO_Down.transform.parent = transform;
		meshStripGO_Down.transform.localPosition = Vector3.zero;
		meshStripGO_Down.transform.localRotation = Quaternion.identity;
		meshStripGO_Down.transform.localScale = Vector3.one;
		meshStripGO_Down.name = transform.name + "_MeshStripn_Down";
		meshStripGO_Down.AddComponent<MeshRenderer>();
		meshStripGO_Down.AddComponent<MeshFilter>();

		meshStripGO_Up.GetComponent<MeshRenderer>().material = meshMaterial;
		meshStripGO_Down.GetComponent<MeshRenderer>().material = meshMaterial;

		List<Vector3> verticesList =  new List<Vector3>();
		// generate vertices , in collumns pairs
		for(int i = 0; i < collumnsCount; i++)
		{
			verticesList.Add( new Vector3(i * collumnWidth, 0, 0));
			verticesList.Add( new Vector3(i * collumnWidth, 0, rowDepth));
		}

		m_trianglesList_Up = new List<int>();
		m_trianglesList_Down = new List<int>();
		// generate triangles
		for(int i = 0; i < collumnsCount - 1; i += 2)
		{
			// bottom left triangle top side
			m_trianglesList_Up.Add(i);
			m_trianglesList_Up.Add(i+1);
			m_trianglesList_Up.Add(i+2);

			// bottom left triangle bottom side
			m_trianglesList_Down.Add(i);
			m_trianglesList_Down.Add(i+2);
			m_trianglesList_Down.Add(i+1);

			// top right triangle top side
			m_trianglesList_Up.Add(i+1);
			m_trianglesList_Up.Add(i+3);
			m_trianglesList_Up.Add(i+2);


			// top right triangle bottom side
			m_trianglesList_Down.Add(i+1);
			m_trianglesList_Down.Add(i+2);
			m_trianglesList_Down.Add(i+3);

		}

		m_verticesArray = verticesList.ToArray();
		List<Vector3> frontRowVertsList = new List<Vector3>();
		List<Vector3> backRowVertsList = new List<Vector3>();
		for(int i = 0; i < verticesList.Count; i += 2)
		{
			frontRowVertsList.Add(verticesList[i]);
			backRowVertsList.Add(verticesList[i+1]);
		}
		m_verticesArray_BackRow = frontRowVertsList.ToArray();
		m_verticesArray_FrontRow = backRowVertsList.ToArray();

		Mesh mesh_Up = meshStripGO_Up.GetComponent<MeshFilter>().mesh;
		mesh_Up.MarkDynamic();
		mesh_Up.vertices = m_verticesArray;
		mesh_Up.triangles = m_trianglesList_Up.ToArray();
		m_mesh_Up = mesh_Up;

		Mesh mesh_Down = meshStripGO_Down.GetComponent<MeshFilter>().mesh;
		mesh_Down.MarkDynamic();
		mesh_Down.vertices = m_verticesArray;
		mesh_Down.triangles = m_trianglesList_Down.ToArray();
		m_mesh_Down = mesh_Down;

		m_mesh_Up.RecalculateNormals();
		m_mesh_Down.RecalculateNormals();

		m_mesh_Up.RecalculateBounds();
		m_mesh_Down.RecalculateBounds();

		float xOffset = m_mesh_Up.bounds.extents.x / 2.0f;
		meshStripGO_Up.transform.localPosition = new Vector3(-xOffset, 0, 0);
		meshStripGO_Down.transform.localPosition = new Vector3(-xOffset, 0, 0);

	}

	public void SetRowsVertices(Vector3[] frontRow_VertsArray, Vector3[] backRowVerts_Array)
	{
		if(frontRow_VertsArray != null && frontRow_VertsArray.Length != 0)
		{
			for(int i = 0; i < m_verticesArray.Length; i += 2)
			{
				m_verticesArray[i] = frontRow_VertsArray[i/2];
			}
			m_verticesArray_BackRow = frontRow_VertsArray;
		}

		if(backRowVerts_Array != null && backRowVerts_Array.Length != 0)
		{
			for(int i = 0; i < m_verticesArray.Length; i += 2)
			{
				m_verticesArray[i+1] = backRowVerts_Array[i/2];
			}
			m_verticesArray_FrontRow = backRowVerts_Array;
		}

		m_mesh_Up.MarkDynamic();
		m_mesh_Down.MarkDynamic();
		m_mesh_Up.vertices = m_verticesArray;
		m_mesh_Down.vertices = m_verticesArray;
		m_mesh_Up.RecalculateNormals();
		m_mesh_Down.RecalculateNormals();

	}

	public Vector3[] GetBackRowVertices()
	{
		return m_verticesArray_BackRow;
	}

	public Vector3[] GetFrontRowVertices()
	{
		return m_verticesArray_FrontRow;
	}

	// Just used for testing
	/*
	void Start()
	{
		GenerateMeshStrip(256, 0.50f, 1.7f);
	}

	void Update()
	{
		Vector3[] freshBackRow = GetBackRowVertices();
		for(int i = 0; i < freshBackRow.Length; i++)
		{
			freshBackRow[i].y =  Mathf.Sin( 20.50025f * Time.time * (float)i/(float)freshBackRow.Length);
		}

		SetRowsVertices(freshBackRow, null);

	}
	*/

}
