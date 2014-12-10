using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class GeneralEditorScript : MonoBehaviour 
{

	public bool isActive = false;

	int minIndex = 0;
	int maxIndex = 2;
	public int currentIndex;

	float inputCooldown = 0.2f;
	float cooldownCounter = 0;

	float incrementCooldown = 0.2f;
	float incrementCooldownCounter = 0;
	float incrementHeldDownDurationCounter = 0;

	float colorScaleIncrement = 0.005f;
	float minColorScale = 0.01f;
	float maxColorScale = 1.0f;

	float playerMinHeight = 0;
	float playerMaxHeight = 400;
	float playerUpDownSpeed = 0.7f;
	float lerpValue = 0;

	public GUISkin guiSkin;

	AmplitudeEditorScript amplitudeEditor;
	FrequencyEditorScript frequencyEditor;
	GameObject playerObject;

	public ParametersFilesImportScript mainFileBrowserScript;

	AudioDirectorScript audioDirector;

	public bool isInMenu = false;
	GameObject fpsDisplayObject;

	// Use this for initialization
	void Start () 
	{

		amplitudeEditor = (AmplitudeEditorScript)GetComponent("AmplitudeEditorScript");
		frequencyEditor = (FrequencyEditorScript)GetComponent("FrequencyEditorScript");
		playerObject = GameObject.FindWithTag("Player");

		mainFileBrowserScript = (ParametersFilesImportScript)GetComponent("ParametersFilesImportScript");

		audioDirector =  (AudioDirectorScript)GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		fpsDisplayObject = GameObject.FindWithTag("FPSDisplay");
	}
	
	// Update is called once per frame
	void Update () 
	{

		HandleInputs();

		// This is really stupid, can't be bothered to make a parent class
		isInMenu = false;
		isInMenu |= isActive;
		isInMenu |= amplitudeEditor.isActive;
		isInMenu |= frequencyEditor.isActive;
		isInMenu |= mainFileBrowserScript.isActive;	
		if(fpsDisplayObject != null)
			fpsDisplayObject.SetActive(isInMenu);
	
	}

	void OnGUI() 
 	{
 		if(isActive)
 		{
    		
 			if(amplitudeEditor.isActive)
 			{
 				GUI.color = Color.red;
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "AMPLITUDE EDIT MODE" , guiSkin.label );
 			}
 			else if(frequencyEditor.isActive)
 			{
 				GUI.color = Color.green;
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "FREQUENCY EDIT MODE" , guiSkin.label );
 			}
 			else
 			{
 				GUI.Label(new Rect(0.0f, 0.02f*Screen.height, Screen.width, 0.2f*Screen.height), "GENERAL EDIT MODE" , guiSkin.label );
 				

 				if(currentIndex == 0)
 				{
 					GUI.color = Color.red;
					GUI.Label(new Rect(0.0f, 0.05f*Screen.height, Screen.width, 0.2f*Screen.height), "Red Scale Factor: " + audioDirector.rScale.ToString() , guiSkin.label );
				}
				else if(currentIndex == 1)
				{
					GUI.color = Color.green;
					GUI.Label(new Rect(0.0f, 0.05f*Screen.height, Screen.width, 0.2f*Screen.height), "Green Scale Factor: " + audioDirector.gScale.ToString() , guiSkin.label );
				}
				else if(currentIndex == 2)
				{
					GUI.color = Color.blue  + Color.green * 0.4f;
					GUI.Label(new Rect(0.0f, 0.05f*Screen.height, Screen.width, 0.2f*Screen.height), "Blue Scale Factor: " + audioDirector.bScale.ToString() , guiSkin.label );
				}

				
 			}

 			GUI.color = Color.white;


 			
			if( GUILayout.Button("Save Parameters File!", GUILayout.ExpandWidth(false)) ) 
    		{
    			string dataDirectory = Application.dataPath;
    			System.IO.File.WriteAllText( dataDirectory + "/Parameters_for_" + audioDirector.currentlyPlayingFileName.Split('.')[0] + ".txt", GenerateParametersFileString());
    			Debug.Log(dataDirectory);
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
				Screen.showCursor = true;
				Screen.lockCursor = false;
			}
			else
			{
				isActive =  false;
				Screen.showCursor = false;
				Screen.lockCursor = true;
			}
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

			if(amplitudeEditor.isActive == false && frequencyEditor.isActive == false)
				HandleGeneralParametersInputs();


			

		}


	}

	void HandleGeneralParametersInputs()
	{
		// Handle parameters controls
		// handle range selection
		if( Input.GetAxis("Editor Horizontal") != 0)
		{
			if( cooldownCounter > inputCooldown )
			{
				// actually apply input
				if( Input.GetAxis("Editor Horizontal") > 0)
					currentIndex += 1;
				else if ( Input.GetAxis("Editor Horizontal") < 0)
					currentIndex -= 1;

				cooldownCounter = 0;

			}
			else
				cooldownCounter += Time.deltaTime;

		}
		else
			cooldownCounter += Time.deltaTime;


		if( currentIndex < minIndex )
			currentIndex = minIndex;
		else if( currentIndex > maxIndex )
			currentIndex = maxIndex;			


		// handle incrementing
		if( Input.GetAxis("Editor Vertical") != 0)
		{
			if( incrementCooldownCounter > (incrementCooldown - incrementHeldDownDurationCounter/10.0f) )
			{
				float tempColorScale = 0;
				if(currentIndex == 0)
					tempColorScale = audioDirector.rScale;
				else if(currentIndex == 1)
					tempColorScale = audioDirector.gScale;
				else if(currentIndex == 2)
					tempColorScale = audioDirector.bScale;
				
				if( Input.GetAxis("Editor Vertical") > 0)
					tempColorScale += colorScaleIncrement;
				else if( Input.GetAxis("Editor Vertical") < 0)
					tempColorScale -= colorScaleIncrement;

				if (tempColorScale < minColorScale)
					tempColorScale = minColorScale;
				else if(tempColorScale > maxColorScale)
					tempColorScale = maxColorScale;

				// WILD RUMPUS HACK
				if(currentIndex == 0)
				{

					audioDirector.rScale = tempColorScale;
					audioDirector.gScale = tempColorScale;
					audioDirector.bScale = tempColorScale;
				}
				else if(currentIndex == 1)
					audioDirector.gScale = tempColorScale;
				else if(currentIndex == 2)
					audioDirector.bScale = tempColorScale;
	

				incrementCooldownCounter = 0;
			}
			else
				incrementCooldownCounter += Time.deltaTime;

			incrementHeldDownDurationCounter += Time.deltaTime;
		}
		else
		{
			incrementCooldownCounter += Time.deltaTime;
			incrementHeldDownDurationCounter = 0;
		}

	}


	string GenerateParametersFileString()
	{

		string tempString = null;

		tempString += "Frequency Domain v0.5\n";
		tempString += "by Sagar Patel\n\n";
		tempString += "This parameters file was generated on" + DateTime.Now + "\n";
		tempString += "Music file being played at the time: " + audioDirector.currentlyPlayingFileName + "\n\n";

		tempString += "The numbers below represent the parameters list. They are listed in the following order:\n";
		tempString += "<Amplitude scales distribution>\n";
		tempString += "<Frequency start sample index>\n";
		tempString += "<Frequency samples distribution>\n";
		tempString += "<RGB Color scale factors>\n\n";


		tempString += "|";
		for(int i =0; i < audioDirector.scalingPerDecadeArray.Length; i++)
		{
			tempString +=  audioDirector.scalingPerDecadeArray[i].ToString() + ",";
		}
		tempString += "\n";

		tempString += "|";
		tempString += audioDirector.sampleStartIndex.ToString();
		tempString += "\n";

		tempString += "|";
		for(int i =0; i < audioDirector.samplesPerDecadeArray.Length; i++)
		{
			tempString +=  audioDirector.samplesPerDecadeArray[i].ToString() + ",";
		}
		tempString += "\n";

		tempString += "|";
		tempString += audioDirector.rScale.ToString() + ",";
		tempString += audioDirector.gScale.ToString() + ",";
		tempString += audioDirector.bScale.ToString() + ",";


		tempString = tempString.Replace("\n", Environment.NewLine);

		return tempString;

	}



}
