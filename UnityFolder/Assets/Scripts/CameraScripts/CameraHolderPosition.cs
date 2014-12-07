using UnityEngine;
using System.Collections;

public class CameraHolderPosition : MonoBehaviour 
{

	public Transform cameraHolderTransform;
	float lerpScale = 10.0f;


	void Update () 
	{
		
		Vector3 targetPos = cameraHolderTransform.position;

		Vector3 finalPos = Vector3.Lerp(transform.position, targetPos, lerpScale * Time.deltaTime);
		transform.position = finalPos;
	}


}
