using UnityEngine;
using System.Collections;
using InControl;


public class CameraTypeHandler : MonoBehaviour 
{

	public GameObject normalCameraObject;
	public GameObject ovrCameraControllerObject;

	public Transform lookAtTarget;

	public bool isOVR = false;

	// Use this for initialization
	void Start () 
	{
		normalCameraObject.SetActive(!isOVR);
		ovrCameraControllerObject.SetActive(isOVR);

	}
	
	// Update is called once per frame
	void Update () 
	{

		var inputDevice = InputManager.ActiveDevice;		

		if(inputDevice.LeftBumper.WasPressed)
		{
			isOVR = !isOVR;
			
			normalCameraObject.SetActive(!isOVR);
			ovrCameraControllerObject.SetActive(isOVR);

		}

		normalCameraObject.transform.LookAt(lookAtTarget, transform.root.up);
		ovrCameraControllerObject.transform.LookAt(lookAtTarget, transform.root.up);
	
	}
}
