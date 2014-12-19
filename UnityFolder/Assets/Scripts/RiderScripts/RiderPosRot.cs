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

		Transform closestMeshlineTransform = meshlinesGenerator.GetClosestMeshLineTransform(transform.position, transform.rotation);



		if(closestMeshlineTransform == null)
			return;

		transform.position = closestMeshlineTransform.position;
		transform.rotation = closestMeshlineTransform.rotation;
		
	
	}



}
