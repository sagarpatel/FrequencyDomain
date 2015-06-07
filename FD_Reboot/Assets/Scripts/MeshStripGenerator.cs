using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshStripGenerator : MonoBehaviour 
{
	List<Vector3> m_verticesList;
	List<int> m_trianglesList;
	List<Vector3> m_normalsList;

	void Start()
	{
		GenerateMeshStrip(4, 2.4f, 1.7f);
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

		m_verticesList =  new List<Vector3>();
		// generate vertices , in collumns pairs
		for(int i = 0; i < collumnsCount; i++)
		{
			m_verticesList.Add( new Vector3(i * collumnWidth, 0, 0));
			m_verticesList.Add( new Vector3(i * collumnWidth, 0, rowDepth));
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

		Mesh mesh = meshStripGO.GetComponent<MeshFilter>().mesh;
		mesh.vertices = m_verticesList.ToArray();
		mesh.triangles = m_trianglesList.ToArray();
	}

}
