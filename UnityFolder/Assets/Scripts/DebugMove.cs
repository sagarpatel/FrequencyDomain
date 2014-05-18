using UnityEngine;
using System.Collections;
using InControl;


public class DebugMove : MonoBehaviour 
{

	public float hControlSpeed = 10.0f;
	public float vControlSpeed = 10.0f;
	public float heightControlSpeed = 10.0f;

	public Vector3 disaplcement = new Vector3(0, 0, 0);

	public Transform targetTransform;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;


		float xTranslation = -inputDevice.Direction.x * hControlSpeed * Time.deltaTime;
		float yTranslation = inputDevice.Direction.y * vControlSpeed * Time.deltaTime;
		float zTranslation = 0;;

		if(Input.GetKey("q"))
		{
			zTranslation = heightControlSpeed * Time.deltaTime;
		}
		if(Input.GetKey("e"))
		{
			zTranslation = -heightControlSpeed * Time.deltaTime;	
		}

		disaplcement.x = xTranslation;
		disaplcement.y = yTranslation;
		disaplcement.z = zTranslation;

		//transform.Translate(disaplcement, Space.World);

		transform.LookAt(targetTransform, transform.parent.up);


	}



}
