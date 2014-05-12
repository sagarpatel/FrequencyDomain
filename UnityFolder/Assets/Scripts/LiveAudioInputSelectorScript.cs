using UnityEngine;
using System.Collections;
using InControl;

public class LiveAudioInputSelectorScript : MonoBehaviour 
{
	public string[] devicesArray;
	public int currentlySelectedDeviceIndex = 0;

	public bool isActive = false;
	float textVerticalOffset = 100.0f;

	AudioDirectorScript audioDirector;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		devicesArray = Microphone.devices;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isActive)
		{
			if(InputManager.ActiveDevice.GetControl( InputControlType.Action1 ))
			{
				if(currentlySelectedDeviceIndex >= devicesArray.Length -1)
						currentlySelectedDeviceIndex = 0;
					else
						currentlySelectedDeviceIndex += 1;

				// Call Audiodirector function with new device
				audioDirector.HandleLiveInputSwitch(devicesArray[currentlySelectedDeviceIndex]);
			}
		}
	
	}


	void OnGUI()
	{


		if(isActive)
		{
			devicesArray = Microphone.devices;

	    	GUI.Label(new Rect(0,textVerticalOffset+0,200,20),"List of available devices:");
	    	GUI.Label(new Rect(200,textVerticalOffset+0,200,20),"Tap the Warp button to switch devices");

			float yPos = 0;
	    	float offset = 10.0f;
			foreach(string device in devicesArray)
	    	{
	    		GUI.Label(new Rect(10,textVerticalOffset+ 20 + yPos, 500, 20), device);
	    		yPos += offset;
	    	}

			GUI.Label(new Rect(0,textVerticalOffset+ 40 + yPos,200,20),"Currently selected device:");
			GUI.Label(new Rect(10,textVerticalOffset+ 60 + yPos,500,20), devicesArray[currentlySelectedDeviceIndex]);
		}

	}
}
