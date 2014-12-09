using UnityEngine;
using System.Collections;

public class RiderPosition : MonoBehaviour 
{
	MeshLinesGenerator meshlinesGenerator;

	void Awake()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}




	void Update()
	{

		transform.position = meshlinesGenerator.GetClosestMeshLinePosition(transform.position);
	}

}
