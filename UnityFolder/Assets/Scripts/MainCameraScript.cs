using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour 
{
	public float distanceAhead = 0;
	Vector3 cameraTarget = new Vector3();
	Vector3 playerVelocity = new Vector3();
	public float rollSensitivity = 1.0f;
	public float pitchSensitivity = 1.0f;


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
		//cameraTarget.y = playerVelocity.y* 0.1f;
		
		transform.LookAt( cameraTarget, Vector3.up );

		transform.eulerAngles = new Vector3(transform.eulerAngles.x + -playerVelocity.x * pitchSensitivity , 
											transform.eulerAngles.y, 
											transform.eulerAngles.z + -playerVelocity.z * rollSensitivity);

	}

}
