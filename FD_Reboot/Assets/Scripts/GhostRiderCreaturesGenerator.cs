using UnityEngine;
using System.Collections;

public class GhostRiderCreaturesGenerator : MonoBehaviour 
{
	public GameObject creatureHeadPartPrefab;
	public GameObject creatureBodyPartPrefab;

	int m_bodyPartsMin = 3;
	float m_airtimeToBodyPartsRatio = 5.0f;

	int m_ghostRiderCreaturesSpawnCounter = 0;

	MeshTerrainGenerator m_meshTerrainGenerator;

	public AnimationCurve m_partsExplosionSpeedCurve;

	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
	}

	public void SpawnGhostRiderCreature(Vector4[] riderDataArray, Color[] colorDataArray, float airTime)
	{
		int bodyPartsCount = m_bodyPartsMin + (int)(airTime * m_airtimeToBodyPartsRatio);

		GameObject ghostRiderCreature = new GameObject("Ghost Rider Creature " + m_ghostRiderCreaturesSpawnCounter);
		ghostRiderCreature.AddComponent<GhostRiderCreature>();
		ghostRiderCreature.GetComponent<GhostRiderCreature>().InitializeGhostRiderCreature(this, m_meshTerrainGenerator, riderDataArray, colorDataArray, creatureHeadPartPrefab, creatureBodyPartPrefab, bodyPartsCount);

		m_ghostRiderCreaturesSpawnCounter ++;
	}
}
