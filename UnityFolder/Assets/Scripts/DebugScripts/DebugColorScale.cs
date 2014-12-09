using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(AudioDirectorScript))]
public class DebugColorScale : MonoBehaviour 
{

	AudioDirectorScript audioDirector;
	bool isGUI;
	
	void Start () 
	{
		audioDirector = GetComponent<AudioDirectorScript>();
	}
	
	
	void Update () 
	{
		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;
		
		float scaleIncrement = 0;
		isGUI = false;
		scaleIncrement += inputDevice.DPadRight * 0.10f * Time.deltaTime;
		scaleIncrement -= inputDevice.DPadLeft * 0.10f * Time.deltaTime;
		
		if(Input.GetKey( KeyCode.RightArrow ))
			scaleIncrement += 0.10f * Time.deltaTime;
		
		if(Input.GetKey( KeyCode.LeftArrow ))
			scaleIncrement -= 0.10f * Time.deltaTime;
		
		if(scaleIncrement != 0)
			isGUI = true;
		
		float currentScale = audioDirector.rScale;
		currentScale += scaleIncrement;
		
		currentScale = Mathf.Clamp(currentScale, 0.001f, 10.0f);
		audioDirector.rScale = currentScale;
		audioDirector.gScale = currentScale;
		audioDirector.bScale = currentScale;
		
		
		
	}
	
	void OnGUI()
	{
		
		if(isGUI)
		{		
			GUI.Label(new Rect(600,80,200,20),"Color Scaler:" + audioDirector.rScale);	
		}
		
	}


}
