using UnityEngine;
using System.Collections;

public class CameraHolderRotation : MonoBehaviour 
{

	public Transform targetLookAtTransform;
	Transform calculationsTransform;
	float lerpScale = 1.0f;

	void Start()
	{
		GameObject temp = new GameObject();
		temp.name = "CameraHolderRotation_CalculationsTransform";
		calculationsTransform = temp.transform;

		calculationsTransform.rotation = targetLookAtTransform.rotation;
	}


	void Update()
	{
		//calculationsTransform.rotation = Quaternion.Slerp(calculationsTransform.rotation, targetLookAtTransform.rotation, lerpScale * Time.deltaTime );
		//transform.rotation = calculationsTransform.rotation;

		transform.LookAt(targetLookAtTransform, targetLookAtTransform.up);

	}

}
