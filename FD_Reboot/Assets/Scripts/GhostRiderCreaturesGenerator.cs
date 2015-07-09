using UnityEngine;
using System.Collections;

public class GhostRiderCreaturesGenerator : MonoBehaviour 
{
	public GameObject creatureHeadPartPrefab;
	public GameObject creatureBodyPartPrefab;

	int m_bodyPartsMin = 3;
	float m_airtimeToBodyPartsRatio = 5.0f;
	float m_airtimeToMovementDurationRatio = 1.5f;

	int m_ghostRiderCreaturesSpawnCounter = 0;

	MeshTerrainGenerator m_meshTerrainGenerator;

	public AnimationCurve m_partsExplosionSpeedCurve;

	void Start()
	{
		m_meshTerrainGenerator = FindObjectOfType<MeshTerrainGenerator>();
	}

	public void SpawnGhostRiderCreature(Vector4[] riderDataArray, Color[] colorDataArray, Quaternion[] riderCamRotationsArray, float airTime)
	{
		int bodyPartsCount = m_bodyPartsMin + (int)(airTime * m_airtimeToBodyPartsRatio);
		float animDuration = m_airtimeToBodyPartsRatio * airTime;

		GameObject ghostRiderCreature = new GameObject("Ghost Rider Creature " + m_ghostRiderCreaturesSpawnCounter);
		ghostRiderCreature.AddComponent<GhostRiderCreature>();
		ghostRiderCreature.GetComponent<GhostRiderCreature>().InitializeGhostRiderCreature(this, m_meshTerrainGenerator, riderDataArray, colorDataArray, riderCamRotationsArray, creatureHeadPartPrefab, creatureBodyPartPrefab, bodyPartsCount, animDuration);

		m_ghostRiderCreaturesSpawnCounter ++;
	}
}
