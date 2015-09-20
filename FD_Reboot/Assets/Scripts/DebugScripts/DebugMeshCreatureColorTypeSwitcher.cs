using UnityEngine;
using System.Collections;

public class DebugMeshCreatureColorTypeSwitcher : MonoBehaviour 
{

	MeshTerrainGenerator m_meshTerrainGenerator;

	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
	}

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.V))
			SwithcMeshColorType();

	}

	void SwithcMeshColorType()
	{
		m_meshTerrainGenerator.p_isMeshColorLive = !m_meshTerrainGenerator.p_isMeshColorLive;
	}
}
