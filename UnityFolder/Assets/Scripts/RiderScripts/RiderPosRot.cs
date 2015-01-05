using UnityEngine;
using System.Collections;

public class RiderPosRot : MonoBehaviour 
{
	MeshLinesGenerator meshlinesGenerator;
	public float widthOffset = 0;
	float meshlineWidth = 600; // TODO: need to get world size of mesh instead of hardoding measured value editor. Myabe using mesh Bounds could work
	
	void Awake()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}
	
	
	
	
	void Update()
	{
		Vector3 calPos = Vector3.zero;
		Quaternion calRot = Quaternion.identity;
		float heightOffset = 0;

		widthOffset = Mathf.Clamp(widthOffset, -meshlineWidth * 0.5f, meshlineWidth * 0.5f);
		float relativeLocationOnLine = 0.5f + widthOffset/meshlineWidth;

		meshlinesGenerator.CalculateClosestMeshLinePosition(transform.position, transform.rotation, relativeLocationOnLine , out calPos, out calRot, out heightOffset);

	

		transform.position = calPos;
		transform.rotation = calRot;

		transform.position += transform.right * widthOffset;
		transform.position += transform.up * heightOffset;


	}



}
