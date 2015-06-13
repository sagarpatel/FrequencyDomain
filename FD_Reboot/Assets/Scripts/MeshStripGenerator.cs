using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshStripGenerator : MonoBehaviour 
{
	List<int> m_trianglesList_Up;
	List<int> m_trianglesList_Down;
	
	Vector3[] m_verticesArray_Right;
	Vector3[] m_verticesArray_BackRow_Right;
	Vector3[] m_verticesArray_FrontRow_Right;

	Mesh m_mesh_Up_Right;
	Mesh m_mesh_Down_Right;

	Vector3[] m_verticesArray_Left;
	Vector3[] m_verticesArray_BackRow_Left;
	Vector3[] m_verticesArray_FrontRow_Left;

	Mesh m_mesh_Up_Left;
	Mesh m_mesh_Down_Left;

	int m_widthVerticesCount;
	

	public void GenerateMeshStrip(int collumnsCount, float collumnWidth, float rowDepth, Material meshMaterial)
	{
		m_widthVerticesCount = collumnsCount;

		// setup GameObjects that will be holding the meshses
		GameObject meshStripGO_Up_Right = new GameObject();
		meshStripGO_Up_Right.transform.parent = transform;
		meshStripGO_Up_Right.transform.localPosition = Vector3.zero;
		meshStripGO_Up_Right.transform.localRotation = Quaternion.identity;
		meshStripGO_Up_Right.transform.localScale = Vector3.one;
		meshStripGO_Up_Right.name = transform.name + "_MeshStrip_Up_Right";
		meshStripGO_Up_Right.AddComponent<MeshRenderer>();
		meshStripGO_Up_Right.AddComponent<MeshFilter>();


		GameObject meshStripGO_Down_Right = new GameObject();
		meshStripGO_Down_Right.transform.parent = transform;
		meshStripGO_Down_Right.transform.localPosition = Vector3.zero;
		meshStripGO_Down_Right.transform.localRotation = Quaternion.identity;
		meshStripGO_Down_Right.transform.localScale = Vector3.one;
		meshStripGO_Down_Right.name = transform.name + "_MeshStrip_Down_Right";
		meshStripGO_Down_Right.AddComponent<MeshRenderer>();
		meshStripGO_Down_Right.AddComponent<MeshFilter>();

		
		GameObject meshStripGO_Up_Left = new GameObject();
		meshStripGO_Up_Left.transform.parent = transform;
		meshStripGO_Up_Left.transform.localPosition = Vector3.zero;
		meshStripGO_Up_Left.transform.localRotation = Quaternion.identity;
		meshStripGO_Up_Left.transform.localScale = Vector3.one;
		meshStripGO_Up_Left.name = transform.name + "_MeshStrip_Up_Left";
		meshStripGO_Up_Left.AddComponent<MeshRenderer>();
		meshStripGO_Up_Left.AddComponent<MeshFilter>();
		
		
		GameObject meshStripGO_Down_Left = new GameObject();
		meshStripGO_Down_Left.transform.parent = transform;
		meshStripGO_Down_Left.transform.localPosition = Vector3.zero;
		meshStripGO_Down_Left.transform.localRotation = Quaternion.identity;
		meshStripGO_Down_Left.transform.localScale = Vector3.one;
		meshStripGO_Down_Left.name = transform.name + "_MeshStrip_Down_Left";
		meshStripGO_Down_Left.AddComponent<MeshRenderer>();
		meshStripGO_Down_Left.AddComponent<MeshFilter>();

		// set meshes material
		meshStripGO_Up_Right.GetComponent<MeshRenderer>().sharedMaterial = meshMaterial;
		meshStripGO_Down_Right.GetComponent<MeshRenderer>().sharedMaterial = meshMaterial;
		meshStripGO_Up_Left.GetComponent<MeshRenderer>().sharedMaterial = meshMaterial;
		meshStripGO_Down_Left.GetComponent<MeshRenderer>().sharedMaterial = meshMaterial;


		// generate vertices, Right, Left get their unique set 
		List<Vector3> verticesList_Right = new List<Vector3>();
		List<Vector3> verticesList_Left = new List<Vector3>();
		// generate vertices , in collumns pairs
		for(int i = 0; i < collumnsCount; i++)
		{
			verticesList_Right.Add( new Vector3(i * collumnWidth, 0, 0));
			verticesList_Right.Add( new Vector3(i * collumnWidth, 0, rowDepth));

			verticesList_Left.Add( new Vector3(-i * collumnWidth, 0, 0)); 
			verticesList_Left.Add( new Vector3(-i * collumnWidth, 0, rowDepth));
		}

		// save vertices into arrays, setup the rows vertices arrays
		m_verticesArray_Right = verticesList_Right.ToArray();
		m_verticesArray_Left = verticesList_Left.ToArray();
		List<Vector3> frontRowVertsList_Right = new List<Vector3>();
		List<Vector3> backRowVertsList_Right = new List<Vector3>();
		List<Vector3> frontRowVertsList_Left = new List<Vector3>();
		List<Vector3> backRowVertsList_Left = new List<Vector3>();
		for(int i = 0; i < verticesList_Right.Count; i += 2)
		{
			backRowVertsList_Right.Add(verticesList_Right[i]);
			frontRowVertsList_Right.Add(verticesList_Right[i+1]);
			backRowVertsList_Left.Add(verticesList_Left[i]);
			frontRowVertsList_Left.Add(verticesList_Left[i+1]);
		}
		m_verticesArray_BackRow_Right = backRowVertsList_Right.ToArray();
		m_verticesArray_FrontRow_Right = frontRowVertsList_Right.ToArray();
		m_verticesArray_BackRow_Left = backRowVertsList_Left.ToArray();
		m_verticesArray_FrontRow_Left = frontRowVertsList_Left.ToArray();

		// generate triangles list, will be the same indicies for Right and Left, so no need to have their own set
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

		//////////// Calculations End

		// assign calculated vertices, etc to the mesh component, call refresh functions etc.

		// Right side mesh
		Mesh mesh_Up_Right = meshStripGO_Up_Right.GetComponent<MeshFilter>().mesh;
		mesh_Up_Right.MarkDynamic();
		mesh_Up_Right.vertices = m_verticesArray_Right;
		mesh_Up_Right.triangles = m_trianglesList_Down.ToArray(); //m_trianglesList_Up.ToArray();
		m_mesh_Up_Right = mesh_Up_Right;

		Mesh mesh_Down_Right = meshStripGO_Down_Right.GetComponent<MeshFilter>().mesh;
		mesh_Down_Right.MarkDynamic();
		mesh_Down_Right.vertices = m_verticesArray_Right;
		mesh_Down_Right.triangles = m_trianglesList_Up.ToArray(); //m_trianglesList_Down.ToArray();
		m_mesh_Down_Right = mesh_Down_Right;

		m_mesh_Up_Right.RecalculateNormals();
		m_mesh_Down_Right.RecalculateNormals();

		m_mesh_Up_Right.RecalculateBounds();
		m_mesh_Down_Right.RecalculateBounds();

		// Left side mesh
		Mesh mesh_Up_Left = meshStripGO_Up_Left.GetComponent<MeshFilter>().mesh;
		mesh_Up_Left.MarkDynamic();
		mesh_Up_Left.vertices = m_verticesArray_Left;
		mesh_Up_Left.triangles = m_trianglesList_Up.ToArray();
		m_mesh_Up_Left = mesh_Up_Left;

		Mesh mesh_Down_Left = meshStripGO_Down_Left.GetComponent<MeshFilter>().mesh;
		mesh_Down_Left.MarkDynamic();
		mesh_Down_Left.vertices = m_verticesArray_Left;
		mesh_Down_Left.triangles = m_trianglesList_Down.ToArray();
		m_mesh_Down_Left = mesh_Down_Left;

		m_mesh_Up_Left.RecalculateNormals();
		m_mesh_Down_Left.RecalculateNormals();

		m_mesh_Up_Left.RecalculateBounds();
		m_mesh_Down_Left.RecalculateBounds();



		//float xOffset = m_mesh_Up_Right.bounds.extents.x / 2.0f;
		//meshStripGO_Up_Right.transform.localPosition = new Vector3(-xOffset, 0, 0);
		//meshStripGO_Down_Right.transform.localPosition = new Vector3(-xOffset, 0, 0);

	}

	public void SetRowsVertices_Right(Vector3[] frontRow_VertsArray_Right, Vector3[] backRowVerts_Array_Right)
	{
		if(frontRow_VertsArray_Right != null && frontRow_VertsArray_Right.Length != 0)
		{
			for(int i = 0; i < m_verticesArray_Right.Length; i += 2)
			{
				m_verticesArray_Right[i] = frontRow_VertsArray_Right[i/2];
			}
			m_verticesArray_FrontRow_Right = frontRow_VertsArray_Right;
		}

		if(backRowVerts_Array_Right != null && backRowVerts_Array_Right.Length != 0)
		{
			for(int i = 0; i < m_verticesArray_Right.Length; i += 2)
			{
				m_verticesArray_Right[i+1] = backRowVerts_Array_Right[i/2];
			}
			m_verticesArray_BackRow_Right = backRowVerts_Array_Right;
		}

		m_mesh_Up_Right.MarkDynamic();
		m_mesh_Down_Right.MarkDynamic();
		m_mesh_Up_Right.vertices = m_verticesArray_Right;
		m_mesh_Down_Right.vertices = m_verticesArray_Right;
		m_mesh_Up_Right.RecalculateNormals();
		m_mesh_Down_Right.RecalculateNormals();
	}

	public void SetRowsVertices_Left(Vector3[] frontRow_VertsArray_Left, Vector3[] backRowVerts_Array_Left)
	{
		if(frontRow_VertsArray_Left != null && frontRow_VertsArray_Left.Length != 0)
		{
			for(int i = 0; i < m_verticesArray_Left.Length; i += 2)
			{
				m_verticesArray_Left[i] = frontRow_VertsArray_Left[i/2];
			}
			m_verticesArray_FrontRow_Left = frontRow_VertsArray_Left;
		}
		
		if(backRowVerts_Array_Left != null && backRowVerts_Array_Left.Length != 0)
		{
			for(int i = 0; i < m_verticesArray_Left.Length; i += 2)
			{
				m_verticesArray_Left[i+1] = backRowVerts_Array_Left[i/2];
			}
			m_verticesArray_BackRow_Left = backRowVerts_Array_Left;
		}
		
		m_mesh_Up_Left.MarkDynamic();
		m_mesh_Down_Left.MarkDynamic();
		m_mesh_Up_Left.vertices = m_verticesArray_Left;
		m_mesh_Down_Left.vertices = m_verticesArray_Left;
		m_mesh_Up_Left.RecalculateNormals();
		m_mesh_Down_Left.RecalculateNormals();
	}

	public Vector3[] GetBackRowVertices_Right()
	{
		return m_verticesArray_BackRow_Right;
	}
	public Vector3[] GetFrontRowVertices_Right()
	{
		return m_verticesArray_FrontRow_Right;
	}

	public Vector3[] GetBackRowVertices_Left()
	{
		return m_verticesArray_BackRow_Left;
	}
	public Vector3[] GetFrontRowVertices_Left()
	{
		return m_verticesArray_FrontRow_Left;
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
