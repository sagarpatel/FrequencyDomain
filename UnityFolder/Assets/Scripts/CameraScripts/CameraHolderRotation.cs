using UnityEngine;
using System.Collections;

public class CameraHolderRotation : MonoBehaviour 
{

	public Transform targetLookAtTransform;
	Transform calculationsTransform;
	float lerpScale = 1.0f;

	PVA creaturePVA;

	void Start()
	{
		GameObject temp = new GameObject();
		temp.name = "CameraHolderRotation_CalculationsTransform";
		calculationsTransform = temp.transform;

		calculationsTransform.LookAt(targetLookAtTransform, targetLookAtTransform.up);
		transform.rotation = calculationsTransform.rotation;

		creaturePVA = GameObject.FindGameObjectWithTag("MeshCreature").GetComponent<PVA>();
	}


	void Update()
	{
		calculationsTransform.position = transform.position;
		calculationsTransform.LookAt(targetLookAtTransform, targetLookAtTransform.up);

		// keeps its tight on y axis rot to avoid joing offscreen
		float axisAvg = 0.75f * Mathf.Abs(creaturePVA.rotationalVelocity.y) + 0.25f * Mathf.Abs(creaturePVA.rotationalVelocity.x);
		float rotLerpAdd = axisAvg / creaturePVA.maxRotationalVelocityMagnitude;

		transform.rotation = Quaternion.Slerp(transform.rotation, calculationsTransform.rotation , rotLerpAdd + lerpScale * Time.deltaTime);


	
	}

}
