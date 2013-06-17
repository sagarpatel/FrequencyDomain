using UnityEngine;
using System.Collections;

public class GeneralEditorScript : MonoBehaviour 
{

	bool isActive = false;



	AmplitudeEditorScript amplitudeEditor;

	// Use this for initialization
	void Start () 
	{

		amplitudeEditor = (AmplitudeEditorScript)GetComponent("AmplitudeEditorScript");
	
	}
	
	// Update is called once per frame
	void Update () 
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
			amplitudeEditor.isActive = true;
		}
		else
		{
			amplitudeEditor.isActive = false;
		}

	
	}



}
