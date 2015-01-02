using UnityEngine;
using System.Collections;

public class RiderPosRot : MonoBehaviour 
{
	MeshLinesGenerator meshlinesGenerator;
	
	void Awake()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}
	
	
	
	
	void Update()
	{
		Vector3 calPos = Vector3.zero;
		Quaternion calRot = Quaternion.identity;

		meshlinesGenerator.GetClosestMeshLineTransform(transform.position, out calPos, out calRot);

	

		transform.position = calPos;
		transform.rotation = calRot;
		
	
	}



}
