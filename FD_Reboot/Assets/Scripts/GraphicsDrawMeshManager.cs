using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GraphicsDrawMeshManager : MonoBehaviour 
{
	Material m_meshCreatureMaterial;
	MaterialPropertyBlock m_meshCreatureMaterialPropertyBlock;

	Mesh[] m_meshCreatureMeshesArray;
	MeshRenderer[] m_meshCreatureMeshRendererArray;

	Transform[] m_meshCreatureMeshStripTransformsArray;
	MeshStripGenerator[] m_meshCreatureMeshStripGeneratorsArray;
	MeshTerrainGenerator m_meshTerrainGenerator;

	DebugCameraSwitcher m_debugCameraSwitcher;

	void Start()
	{

		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
		m_debugCameraSwitcher = FindObjectOfType<DebugCameraSwitcher>();

		m_meshCreatureMaterialPropertyBlock = new MaterialPropertyBlock();
		m_meshCreatureMaterialPropertyBlock.AddColor("_Color", Color.black);
		m_meshCreatureMaterialPropertyBlock.AddColor("_WireframeBoundsRGB", Color.black);

		m_meshCreatureMeshStripTransformsArray = m_meshTerrainGenerator.m_meshStripGeneratorsArray.Select(m => m.transform).ToArray(); 
		m_meshCreatureMeshStripGeneratorsArray = m_meshCreatureMeshStripTransformsArray.Select(t => t.GetComponent<MeshStripGenerator>()).ToArray();

		List<Mesh> meshes = new List<Mesh>();
		List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
		for(int i = 0; i < m_meshCreatureMeshStripTransformsArray.Length; i++)
		{
			meshes.Add(m_meshCreatureMeshStripTransformsArray[i].GetChild(0).GetComponent<MeshFilter>().mesh);
			meshRenderers.Add(m_meshCreatureMeshStripTransformsArray[i].GetChild(0).GetComponent<MeshRenderer>() );

			meshes.Add(m_meshCreatureMeshStripTransformsArray[i].GetChild(2).GetComponent<MeshFilter>().mesh);
			meshRenderers.Add(m_meshCreatureMeshStripTransformsArray[i].GetChild(2).GetComponent<MeshRenderer>() );
		}

		m_meshCreatureMeshesArray = meshes.ToArray();
		m_meshCreatureMeshRendererArray = meshRenderers.ToArray();

		m_meshCreatureMaterial = m_meshTerrainGenerator.m_meshStripsMaterial;

		// kill normal rendering
		for(int i = 0; i < m_meshCreatureMeshRendererArray.Length; i++)
		{
			m_meshCreatureMeshRendererArray[i].enabled = false;
		}

	}

	void LateUpdate()
	{
		for(int i = 0; i < m_meshCreatureMeshStripGeneratorsArray.Length; i++)
		{
			m_meshCreatureMaterialPropertyBlock.SetColor("_Color", m_meshCreatureMeshStripGeneratorsArray[i].r_latestStripColor);
			m_meshCreatureMaterialPropertyBlock.SetColor("_WireframeBoundsRGB", m_meshCreatureMeshStripGeneratorsArray[i].r_latestStripWireframeBounds);

			Graphics.DrawMesh (m_meshCreatureMeshesArray[i], m_meshCreatureMeshStripTransformsArray[i].position, m_meshCreatureMeshStripTransformsArray[i].rotation, m_meshCreatureMaterial,0, m_debugCameraSwitcher.r_currentActiveCamera, 0, m_meshCreatureMaterialPropertyBlock );
			Graphics.DrawMesh (m_meshCreatureMeshesArray[i], m_meshCreatureMeshStripTransformsArray[i].position, m_meshCreatureMeshStripTransformsArray[i].rotation, m_meshCreatureMaterial,0, m_debugCameraSwitcher.r_currentActiveCamera, 0, m_meshCreatureMaterialPropertyBlock );
		}

	}

}
