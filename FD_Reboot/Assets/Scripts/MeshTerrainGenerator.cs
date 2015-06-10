using UnityEngine;
using System.Collections;
using System.Linq;

public class MeshTerrainGenerator : MonoBehaviour 
{

	MeshStripGenerator[] meshStripGeneratorsArray;
	int meshStripsPoolCount = 200;

	void Start()
	{
		GameObject[] meshStripGeneratorsGOArray = new GameObject[meshStripsPoolCount];
		for(int i = 0; i < meshStripGeneratorsGOArray.Length; i++)
		{
			meshStripGeneratorsGOArray[i] = new GameObject();
			meshStripGeneratorsGOArray[i].name = "MeshStripGenerator_" + i.ToString();
			meshStripGeneratorsGOArray[i].transform.parent = transform;
			meshStripGeneratorsGOArray[i].transform.localPosition = Vector3.zero;
			meshStripGeneratorsGOArray[i].transform.localRotation = Quaternion.identity;
			meshStripGeneratorsGOArray[i].transform.localScale = Vector3.one;
			meshStripGeneratorsGOArray[i].AddComponent<MeshStripGenerator>();
		}
		meshStripGeneratorsArray = meshStripGeneratorsGOArray.Select(g => g.GetComponent<MeshStripGenerator>()).ToArray();

	}


}
