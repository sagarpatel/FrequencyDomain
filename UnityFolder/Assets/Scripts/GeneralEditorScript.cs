using UnityEngine;
using System.Collections;

public class GeneralEditorScript : MonoBehaviour 
{

	public bool isActive = false;

	float playerMinHeight = 10;
	float playerMaxHeight = 400;
	float playerUpDownSpeed = 0.7f;
	float lerpValue = 0;

	AmplitudeEditorScript amplitudeEditor;
	GameObject playerObject;

	// Use this for initialization
	void Start () 
	{

		amplitudeEditor = (AmplitudeEditorScript)GetComponent("AmplitudeEditorScript");
		playerObject = GameObject.FindGameObjectWithTag("Player");
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		HandleInputs();


		if(isActive)
		{
			amplitudeEditor.isActive = true;
		}
		else
		{
			amplitudeEditor.isActive = false;
		}

	
	}

	void HandleInputs()
	{

		// check to toggle edit mode
		if(  Input.GetButtonDown("Toggle Edit Mode Button") == true )
		{
			if(isActive == false)
				isActive = true;
			else
				isActive =  false;
		}


		if(isActive)
		{
			Vector3 tempPosition = playerObject.transform.position;
			// handle player height
			if( Input.GetAxis("LeftTrigger") > 0 )
				lerpValue -= Input.GetAxis("LeftTrigger") * playerUpDownSpeed * Time.deltaTime;
			if( Input.GetAxis("RightTrigger") < 0 )
				lerpValue -= Input.GetAxis("RightTrigger") * playerUpDownSpeed * Time.deltaTime;

			lerpValue = Mathf.Clamp( lerpValue, 0, 1.0f);
			tempPosition.y = Mathf.Lerp( playerMinHeight, playerMaxHeight, lerpValue);

			playerObject.transform.position = tempPosition;

		}


	}



}
