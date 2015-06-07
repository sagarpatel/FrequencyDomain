using UnityEngine;
using System.Collections;

public class MeshStripGenerator : MonoBehaviour 
{

	public void GenerateMeshStrip(int collumnsCount, float collumnWidth)
	{

		GameObject meshStripGO = new GameObject();
		meshStripGO.transform.parent = transform;
		meshStripGO.transform.localPosition = Vector3.zero;
		meshStripGO.transform.localRotation = Quaternion.identity;
		meshStripGO.transform.localScale = Vector3.one;
		meshStripGO.name = transform.name + "_MeshStrip";

		meshStripGO.AddComponent<MeshRenderer>();


	}

}
