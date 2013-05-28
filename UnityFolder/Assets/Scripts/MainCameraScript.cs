using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour 
{
	public float distanceAhead = 100;
	Vector3 cameraTarget = new Vector3();
	Vector3 playerVelocity = new Vector3();
	public float rollSensitivity = 0.1f;
	public float yawSensitivity = 0.05f;
	public float pitchSensitivity = 0.2f;

	


	// Use this for initialization
	void Start () 
	{
		playerVelocity = ((PlayerScript)( (GameObject.FindWithTag("Player")).GetComponent("PlayerScript"))).velocity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		playerVelocity = ((PlayerScript)( (GameObject.FindWithTag("Player")).GetComponent("PlayerScript"))).velocity;
		cameraTarget = transform.parent.position;
		cameraTarget.x -= distanceAhead;
		//cameraTarget.y = playerVelocity.y* 0.1f;
		//cameraTarget.y = cameraTarget.y/3.0f;
		cameraTarget.y = transform.parent.position.y/1.5f;
		
		transform.LookAt( cameraTarget, Vector3.up );	

		
		
		transform.localEulerAngles = new Vector3(transform.eulerAngles.x + -playerVelocity.x * pitchSensitivity , 
											transform.eulerAngles.y + playerVelocity.z *  yawSensitivity, 
											transform.eulerAngles.z + -playerVelocity.z * rollSensitivity);

		


		//Debug.Log( Input.GetAxis("LeftTrigger") );
		//Debug.Log( Input.GetAxis("RightTrigger") );
		//Debug.Log( Input.GetAxis("Horizontal") );

	}

}
