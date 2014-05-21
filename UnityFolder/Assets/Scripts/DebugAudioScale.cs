using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(AudioDirectorScript))]
public class DebugAudioScale : MonoBehaviour 
{
	AudioDirectorScript audioDirector;

	void Start () 
	{
		audioDirector = GetComponent<AudioDirectorScript>();
	}
	
	
	void Update () 
	{
		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;

		float scaleIncrement = 0;
		scaleIncrement += inputDevice.DPadUp * 8.0f * Time.deltaTime;
		scaleIncrement -= inputDevice.DPadDown * 8.0f * Time.deltaTime;

		audioDirector.overallAmplitudeScaler += scaleIncrement;
	}
}
