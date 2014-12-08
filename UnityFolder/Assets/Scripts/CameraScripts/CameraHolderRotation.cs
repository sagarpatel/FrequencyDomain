using UnityEngine;
using System.Collections;

public class CameraHolderRotation : MonoBehaviour 
{

	public Transform targetLookAtTransform;
	Transform calculationsTransform;
	float lerpScale = 8.0f;

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

		Quaternion relativeRot = Quaternion.Inverse(targetLookAtTransform.rotation) * transform.rotation;
		Vector3 diffedEuler = -GetDiffEuler(relativeRot.eulerAngles);
		//Debug.Log(diffedEuler);
		//diffedEuler.z = Mathf.Lerp(0, diffedEuler.z, lerpScale * Time.deltaTime);
		//diffedEuler.z *= lerpScale * Time.deltaTime; //Mathf.Clamp( lerpScale * Time.deltaTime, 0, 1);


		//Quaternion targetRot = calculationsTransform.rotation * Quaternion.Euler(diffedEuler);

		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRot , lerpScale * Time.deltaTime);

		transform.rotation *= Quaternion.Euler(diffedEuler);


	}


	Vector3 GetDiffEuler(Vector3 eulerAngle)
	{
		Vector3 temp = Vector3.zero;

		if(eulerAngle.x > 180)
			temp.x = eulerAngle.x - 360.0f;
		else
			temp.x = eulerAngle.x;

		if(eulerAngle.y > 180)
			temp.y = eulerAngle.y - 360.0f;
		else
			temp.y = eulerAngle.y;


		if(eulerAngle.z > 180)
			temp.z = eulerAngle.z - 360.0f;
		else
			temp.z = eulerAngle.z;


		return temp;
	}

}
