using UnityEngine;
using System.Collections;

public class GeneralEditorScript : MonoBehaviour 
{

	public bool isActive = false;

	float playerMinHeight = 0;
	float playerMaxHeight = 400;
	float playerUpDownSpeed = 0.7f;
	float lerpValue = 0;

	public GUISkin guiSkin;

	AmplitudeEditorScript amplitudeEditor;
	FrequencyEditorScript frequencyEditor;
	GameObject playerObject;

	FileBrowserGameObjectScript mainFileBrowserScript;

	// Use this for initialization
	void Start () 
	{

		amplitudeEditor = (AmplitudeEditorScript)GetComponent("AmplitudeEditorScript");
		frequencyEditor = (FrequencyEditorScript)GetComponent("FrequencyEditorScript");
		playerObject = GameObject.FindGameObjectWithTag("Player");

		mainFileBrowserScript = (FileBrowserGameObjectScript)GetComponent("FileBrowserGameObjectScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		HandleInputs();
	
	}

	void OnGUI() 
 	{
 		if(isActive)
 		{
    		
 			if(amplitudeEditor.isActive)
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "AMPLITUDE EDIT MODE" , guiSkin.label );
 			else if(frequencyEditor.isActive)
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "FREQUENCY EDIT MODE" , guiSkin.label );
 			else
 			{
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "GENERAL EDIT MODE" , guiSkin.label );
 				if( GUILayout.Button("Save Parameters File!", GUILayout.ExpandWidth(false)) ) 
        		{

        		}

 			}



    	}
    }

	void HandleInputs()
	{

		// check to toggle edit mode
		if(  Input.GetButtonDown("Toggle Edit Mode Button") == true )
		{
			if(isActive == false)
			{
				isActive = true;
				mainFileBrowserScript.isActive = false;
			}
			else
				isActive =  false;
		}


		if(isActive)
		{

			// Handle player position

			/// START

			Vector3 tempPosition = playerObject.transform.position;
			// handle player height
			// gamepad controls
			if( Input.GetAxis("LeftTrigger") > 0 )
				lerpValue -= Input.GetAxis("LeftTrigger") * playerUpDownSpeed * Time.deltaTime;
			if( Input.GetAxis("RightTrigger") < 0 )
				lerpValue -= Input.GetAxis("RightTrigger") * playerUpDownSpeed * Time.deltaTime;

			// keboard controls
			if( Input.GetKey("q") )
				lerpValue -=  playerUpDownSpeed * Time.deltaTime;
			if( Input.GetKey("e") )
				lerpValue +=  playerUpDownSpeed * Time.deltaTime;

			lerpValue = Mathf.Clamp( lerpValue, 0, 1.0f);
			tempPosition.y = Mathf.Lerp( playerMinHeight, playerMaxHeight, lerpValue);

			playerObject.transform.position = tempPosition;

			/// END


			// Handle edit modes toggles

			if( Input.GetButtonDown("Toggle Amplitude Edit") )
			{
				if( amplitudeEditor.isActive )
				{
					amplitudeEditor.isActive = false;
				}
				else
				{
					amplitudeEditor.isActive = true;
					frequencyEditor.isActive = false;
				}
			}

			if( Input.GetButtonDown("Toggle Frequency Edit") )
			{
				if( frequencyEditor.isActive )
				{
					frequencyEditor.isActive = false;
				}
				else
				{
					frequencyEditor.isActive = true;
					amplitudeEditor.isActive = false;
				}

			}


		}


	}



}
