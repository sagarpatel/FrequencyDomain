using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(AudioDirectorScript))]
public class DebugAudioScale : MonoBehaviour 
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
		scaleIncrement += inputDevice.DPadUp * 8.0f * Time.deltaTime;
		scaleIncrement -= inputDevice.DPadDown * 8.0f * Time.deltaTime;

		if(scaleIncrement != 0)
			isGUI = true;

		audioDirector.overallAmplitudeScaler += scaleIncrement;

	}

	void OnGUI()
	{

		if(isGUI)
		{		
	    	GUI.Label(new Rect(600,40,200,20),"Amplitude Scaler:" + audioDirector.overallAmplitudeScaler);	
	    }

	}

}
