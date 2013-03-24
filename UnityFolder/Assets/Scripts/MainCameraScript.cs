using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour 
{
	public float distanceAhead = 0;
	Vector3 cameraTarget = new Vector3();
	Vector3 playerVelocity = new Vector3();
	public float rollSensitivity = 1.0f;
	public float pitchSensitivity = 1.0f;

	public float barrelRollSensitivity = 1.0f;
	public float barrelRollDegradation = 0.95f;
	public float barrelRollTriggerCounter;


	// Use this for initialization
	void Start () 
	{
		playerVelocity = ((PlayerScript)(transform.parent.gameObject.GetComponent("PlayerScript"))).velocity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		playerVelocity = ((PlayerScript)(transform.parent.gameObject.GetComponent("PlayerScript"))).velocity;
		cameraTarget = transform.parent.position;
		cameraTarget.x -= distanceAhead;
		cameraTarget.y = playerVelocity.y* 0.1f;
		cameraTarget.y = cameraTarget.y/3.0f;
		
		transform.LookAt( cameraTarget, Vector3.up );

		// Handle barrel roll input
		// windows controls for triggers
		if( Input.GetAxis("LeftTrigger") > 0 )
			barrelRollTriggerCounter += Input.GetAxis("LeftTrigger");
		if( Input.GetAxis("RightTrigger") < 0 )
			barrelRollTriggerCounter += Input.GetAxis("RightTrigger");

		transform.eulerAngles = new Vector3(transform.eulerAngles.x + -playerVelocity.x * pitchSensitivity , 
											transform.eulerAngles.y, 
											transform.eulerAngles.z + -playerVelocity.z * rollSensitivity + barrelRollTriggerCounter * 10);

		barrelRollTriggerCounter =  barrelRollTriggerCounter * barrelRollDegradation;

		Debug.Log( Input.GetAxis("LeftTrigger") );
		Debug.Log( Input.GetAxis("RightTrigger") );
		//Debug.Log( Input.GetAxis("Horizontal") );

	}

}
