using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshStripGenerator : MonoBehaviour 
{
	List<int> m_trianglesList;
	List<Vector3> m_normalsList;

	Vector3[] m_verticesArray;
	Vector3[] m_verticesArray_FrontRow;
	Vector3[] m_verticesArray_BackRow;

	int m_widthVerticesCount;

	Mesh m_mesh;

	void Start()
	{
		GenerateMeshStrip(256, 0.50f, 1.7f);
	}


	public void GenerateMeshStrip(int collumnsCount, float collumnWidth, float rowDepth)
	{

		GameObject meshStripGO = new GameObject();
		meshStripGO.transform.parent = transform;
		meshStripGO.transform.localPosition = Vector3.zero;
		meshStripGO.transform.localRotation = Quaternion.identity;
		meshStripGO.transform.localScale = Vector3.one;
		meshStripGO.name = transform.name + "_MeshStrip";

		meshStripGO.AddComponent<MeshRenderer>();
		meshStripGO.AddComponent<MeshFilter>();

		List<Vector3> verticesList =  new List<Vector3>();
		// generate vertices , in collumns pairs
		for(int i = 0; i < collumnsCount; i++)
		{
			verticesList.Add( new Vector3(i * collumnWidth, 0, 0));
			verticesList.Add( new Vector3(i * collumnWidth, 0, rowDepth));
		}

		m_trianglesList = new List<int>();
		// generate triangles
		for(int i = 0; i < collumnsCount - 1; i += 2)
		{
			// bottom left triangle top side
			m_trianglesList.Add(i);
			m_trianglesList.Add(i+1);
			m_trianglesList.Add(i+2);

			// bottom left triangle bottom side
			m_trianglesList.Add(i);
			m_trianglesList.Add(i+2);
			m_trianglesList.Add(i+1);

			// top right triangle top side
			m_trianglesList.Add(i+1);
			m_trianglesList.Add(i+3);
			m_trianglesList.Add(i+2);


			// top right triangle bottom side
			m_trianglesList.Add(i+1);
			m_trianglesList.Add(i+2);
			m_trianglesList.Add(i+3);

		}

		m_verticesArray = verticesList.ToArray();
		List<Vector3> frontRowVertsList = new List<Vector3>();
		List<Vector3> backRowVertsList = new List<Vector3>();
		for(int i = 0; i < verticesList.Count; i += 2)
		{
			frontRowVertsList.Add(verticesList[i]);
			backRowVertsList.Add(verticesList[i+1]);
		}
		m_verticesArray_FrontRow = frontRowVertsList.ToArray();
		m_verticesArray_BackRow = backRowVertsList.ToArray();

		Mesh mesh = meshStripGO.GetComponent<MeshFilter>().mesh;
		mesh.MarkDynamic();
		mesh.vertices = m_verticesArray;
		mesh.triangles = m_trianglesList.ToArray();

		m_mesh = mesh;
	}

	void SetRowsVertices(Vector3[] frontRow_VertsArray, Vector3[] backRowVerts_Array)
	{
		if(frontRow_VertsArray != null && frontRow_VertsArray.Length != 0)
		{
			for(int i = 0; i < m_verticesArray.Length; i += 2)
			{
				m_verticesArray[i] = frontRow_VertsArray[i/2];
			}
			m_verticesArray_FrontRow = frontRow_VertsArray;
		}

		if(backRowVerts_Array != null && backRowVerts_Array.Length != 0)
		{
			for(int i = 0; i < m_verticesArray.Length; i += 2)
			{
				m_verticesArray[i+1] = backRowVerts_Array[i/2 + 1];
			}
			m_verticesArray_BackRow = backRowVerts_Array;
		}

		m_mesh.MarkDynamic();
		m_mesh.vertices = m_verticesArray;
	}


	void Update()
	{
		Vector3[] freshFrontRow = m_verticesArray_FrontRow;
		for(int i = 0; i < freshFrontRow.Length; i++)
		{
			freshFrontRow[i].y =  Mathf.Sin( 0.50025f * Time.time * i);
		}

		SetRowsVertices(freshFrontRow, null);

	}


}
