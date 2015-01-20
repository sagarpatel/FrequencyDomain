using UnityEngine;
using System.Collections;

public class RiderCameraTargetMeshGeneratorCreature : MonoBehaviour 
{
	public GameObject meshCreatureGeneratorHead;
	public Camera riderCamera;
	Plane[] riderCamPlanes;
	Collider meshCreatureGeneratorHeadCollider;

	void Start()
	{
		riderCamPlanes = GeometryUtility.CalculateFrustumPlanes(riderCamera);
		meshCreatureGeneratorHeadCollider = meshCreatureGeneratorHead.GetComponent<Collider>();
	}

	void Update()
	{
		if(GeometryUtility.TestPlanesAABB(riderCamPlanes, meshCreatureGeneratorHeadCollider.bounds))
		{

		}
		else
		{
			Debug.Log(" MESH HEAD OUTOF VIEW!");
		}

	}

}
