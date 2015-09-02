using UnityEngine;
using System.Collections;

public class DebugToggleOriginMesh : MonoBehaviour
{
    private MeshStripGenerator m_originMeshStripGenerator;
    private MeshTerrainGenerator m_meshTerrainGenerator;


    void Start()
    {
        m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
        m_originMeshStripGenerator = m_meshTerrainGenerator.m_meshCreatureOriginMesh;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) == true)
            m_originMeshStripGenerator.gameObject.SetActive(!m_originMeshStripGenerator.gameObject.activeSelf);

    }

}
