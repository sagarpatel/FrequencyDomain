using UnityEngine;
using System.Collections;

public class CameraHolderLerp : MonoBehaviour 
{
	public Transform cameraHolderTransform;
	float lerpSpeed = 10.0f;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Vector3 targetPos = cameraHolderTransform.position;
		Quaternion targetRot = cameraHolderTransform.rotation;

		Vector3 finalPos = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
		Quaternion finalRot = Quaternion.Slerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);

		transform.position = finalPos;
		transform.rotation = finalRot;

	}


}
