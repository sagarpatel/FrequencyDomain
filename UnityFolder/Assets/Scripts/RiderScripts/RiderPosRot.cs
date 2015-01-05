using UnityEngine;
using System.Collections;

public class RiderPosRot : MonoBehaviour 
{
	MeshLinesGenerator meshlinesGenerator;
	public float widthOffset = 0;
	
	void Awake()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}
	
	
	
	
	void Update()
	{
		Vector3 calPos = Vector3.zero;
		Quaternion calRot = Quaternion.identity;

		meshlinesGenerator.GetClosestMeshLineTransform(transform.position, transform.rotation, out calPos, out calRot);

	

		transform.position = calPos;
		transform.rotation = calRot;

		transform.position += transform.right * widthOffset;
	
	}



}
