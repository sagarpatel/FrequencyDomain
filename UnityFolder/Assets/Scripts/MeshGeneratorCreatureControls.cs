using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(PVA))]
public class MeshGeneratorCreatureControls : MonoBehaviour 
{
	PVA pva;

	public float hControlSpeed = 10.0f;
	public float vControlSpeed = 10.0f;

	//read only
	public Vector3 deltaV;


	void Start () 
	{
		pva = GetComponent<PVA>();
	}
	

	void Update () 
	{

		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;

		float xTranslation = inputDevice.Direction.x * hControlSpeed * Time.deltaTime;
		float yTranslation = inputDevice.Direction.y * vControlSpeed * Time.deltaTime;
		
		deltaV.x = xTranslation;
		deltaV.y = yTranslation;
		deltaV.z = 0;

		pva.acceleration = deltaV;


	}
}
