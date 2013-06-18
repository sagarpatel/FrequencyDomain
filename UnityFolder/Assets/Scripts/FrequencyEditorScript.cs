using UnityEngine;
using System.Collections;

public class FrequencyEditorScript : MonoBehaviour 
{

	int minIndex = 0;
	int maxIndex = 9;
	public int currentIndex;

	float inputCooldown = 0.2f;
	float cooldownCounter = 0;

	public GameObject rangeMarker;
	Vector3 rangeMarkerPosition = new Vector3();

	public GUISkin guiSkin;
	public bool isActive = false;

	int minSamples = 1;
	int maxSamples = 512;

	AudioDirectorScript audioDirector;


	// Use this for initialization
	void Start () 
	{
		currentIndex = minIndex;
		rangeMarker = (GameObject)Instantiate(rangeMarker, new Vector3(), Quaternion.identity);

		audioDirector =  (AudioDirectorScript)GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");

	}
	
	// Update is called once per frame
	void Update () 
	{

		if(isActive)
		{
			HandleInputs();

			foreach (Transform child in rangeMarker.transform)
				child.gameObject.renderer.enabled = true;
			
			rangeMarkerPosition = new Vector3(0,0, 40 * currentIndex );
			rangeMarker.transform.position = rangeMarkerPosition;
			AdjustRangeMarkerScale();
		}
		else
		{
			foreach (Transform child in rangeMarker.transform)
				child.gameObject.renderer.enabled = false;
		}


	
	}

	void OnGUI() 
 	{
 		if(isActive)
 		{
    		GUI.Label(new Rect(0.0f, 0.05f*Screen.height, Screen.width, 0.2f*Screen.height), "Current Frequency Range Index: " + currentIndex.ToString(), guiSkin.label );
    		GUI.Label(new Rect(0.0f, 0.1f*Screen.height, Screen.width, 0.2f*Screen.height), "Current Frequency Samples: " + audioDirector.samplesPerDecadeArray[currentIndex].ToString(), guiSkin.label );

    	}
    }


	void HandleInputs()
	{

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
			int tempSampleCount = audioDirector.samplesPerDecadeArray[currentIndex];
			
			if( Input.GetAxis("Editor Vertical") > 0)
				tempSampleCount += 1;
			else if( Input.GetAxis("Editor Vertical") < 0)
				tempSampleCount -= 1;

			if (tempSampleCount < minSamples)
				tempSampleCount = minSamples;
			else if(tempSampleCount > maxSamples)
				tempSampleCount = maxSamples;

			audioDirector.samplesPerDecadeArray[currentIndex] = tempSampleCount;
			

		}


	}

	void AdjustRangeMarkerScale()
	{


	}


}
