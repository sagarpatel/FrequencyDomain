using UnityEngine;
using System.Collections;
using InControl;


public class CameraTypeHandler : MonoBehaviour 
{

	public GameObject normalCameraObject;
	public GameObject ovrCameraControllerObject;

	public bool isOVR = false;

	// Use this for initialization
	void Start () 
	{
	
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
	
	}
}
