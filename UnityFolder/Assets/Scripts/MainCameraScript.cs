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

	float rsHorizontalSensitivity = 200.0f;
	float rsVerticalSensitibity = 200.0f;
	Vector3 lookAtOffsetPosition = new Vector3();
	float lookAtOffsetFriction = 2f;

	bool isWireFrameMode = true;

	LMC_PlayerControls lmcPlayerControls;

	// Use this for initialization
	void Start () 
	{
		playerVelocity = ((PlayerScript)( (GameObject.FindWithTag("Player")).GetComponent("PlayerScript"))).velocity;
		lmcPlayerControls = (LMC_PlayerControls)GameObject.FindWithTag("LMC").GetComponent<LMC_PlayerControls>();
		//Screen.showCursor = false;
		//Screen.lockCursor = true;
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

		
		cameraTarget += lookAtOffsetPosition;
		
		if( ((PlayerScript)( (GameObject.FindWithTag("Player")).GetComponent("PlayerScript"))).isOVR == false )	
		{
			// apply right stick camera control
			// hacked up mouse, not good but there anyways
			float zLookAtOffset = Mathf.Pow( Input.GetAxis("RSHorizontal"), 5) * rsHorizontalSensitivity * Time.deltaTime;
			zLookAtOffset += Input.GetAxis("Mouse X") * Time.deltaTime * 150.0f;
			zLookAtOffset += lmcPlayerControls.horizontalLook * rsHorizontalSensitivity * Time.deltaTime;

			float yLookAtOffset = Mathf.Pow( Input.GetAxis("RSVertical"), 3) * rsVerticalSensitibity * Time.deltaTime ;
			yLookAtOffset += -Input.GetAxis("Mouse Y") * Time.deltaTime * 150.0f;

			lookAtOffsetPosition += new Vector3( 0, -yLookAtOffset, zLookAtOffset );
			lookAtOffsetPosition -= lookAtOffsetPosition * lookAtOffsetFriction * Time.deltaTime;

			transform.LookAt( cameraTarget, Vector3.up );	
			
			transform.localEulerAngles = new Vector3(transform.eulerAngles.x + -playerVelocity.x * pitchSensitivity , 
													transform.eulerAngles.y + playerVelocity.z *  yawSensitivity, 
													transform.eulerAngles.z + -playerVelocity.z * rollSensitivity);
		}
		


		if(Input.GetButtonDown("Toggle WireFrame Mode"))
		{
			if(isWireFrameMode == true)
				isWireFrameMode = false;
			else
				isWireFrameMode = true;
		}

	}


	// based off code seen in OVRCamera.cs
	void OnPreRender()
	{
		if(isWireFrameMode == true)
			GL.wireframe = true;
	}

	void OnPostRender()
	{
		if(isWireFrameMode == true)
			GL.wireframe = false;
	}

}
