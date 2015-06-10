﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshStripGenerator : MonoBehaviour 
{
	List<int> m_trianglesList_Up;
	List<int> m_trianglesList_Down;
	List<Vector3> m_normalsList;

	Vector3[] m_verticesArray;
	Vector3[] m_verticesArray_FrontRow;
	Vector3[] m_verticesArray_BackRow;

	int m_widthVerticesCount;

	Mesh m_mesh_Up;
	Mesh m_mesh_Down;

	void Start()
	{
		GenerateMeshStrip(256, 0.50f, 1.7f);
	}


	public void GenerateMeshStrip(int collumnsCount, float collumnWidth, float rowDepth)
	{

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
		m_verticesArray_FrontRow = frontRowVertsList.ToArray();
		m_verticesArray_BackRow = backRowVertsList.ToArray();

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

		m_mesh_Up.MarkDynamic();
		m_mesh_Down.MarkDynamic();
		m_mesh_Up.vertices = m_verticesArray;
		m_mesh_Down.vertices = m_verticesArray;
		m_mesh_Up.RecalculateNormals();
		m_mesh_Down.RecalculateNormals();

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
