using UnityEngine;
using System.Collections;
using InControl;


public class CameraOrbit : MonoBehaviour 
{	
	public Transform orbitAnchor;

	public float xOrbitSpeed = 10.0f;
	public float yOrbitSpeed = 10.0f;

	public float orbitRadius = 150.0f;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		var inputDevice = InputManager.ActiveDevice;		

		float xRot = orbitRadius * Mathf.Cos( inputDevice.RightStickX  ) * xOrbitSpeed * Time.deltaTime;
		float yRot = orbitRadius * Mathf.Sin( inputDevice.RightStickY  ) * yOrbitSpeed * Time.deltaTime;

		transform.RotateAround(orbitAnchor.position, orbitAnchor.up, xRot);
		transform.RotateAround(orbitAnchor.position, orbitAnchor.right, yRot);


	}


}
