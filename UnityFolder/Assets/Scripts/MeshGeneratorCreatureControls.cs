using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(PVA))]
public class MeshGeneratorCreatureControls : MonoBehaviour 
{
	PVA pva;

	public float hControlScale = 10.0f;
	public float vControlScale = 10.0f;
	public float forwardControlScale = 10.0f;

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

		float xAcc = inputDevice.Direction.x * hControlScale * Time.deltaTime;
		float yAcc = inputDevice.Direction.y * vControlScale * Time.deltaTime;
		float zAcc = inputDevice.Action1 * forwardControlScale * Time.deltaTime;


		deltaV.x = xAcc;
		deltaV.y = yAcc;
		deltaV.z = zAcc;

		pva.acceleration = deltaV;


	}
}
