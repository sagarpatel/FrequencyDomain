using UnityEngine;
using System.Collections;

public class CreaturePartsAudioScalingScript : MonoBehaviour 
{

	Vector3 originalPrefabScale;
	AudioDirectorScript audioDirector;

	float previousRMS = 0;

	public float rmsScalingFactor = 1.0f;
	public float scalingDecayFactor = 1.0f;

	float currentRMSScalingValue = 1.0f;

	// Use this for initialization
	void Start () 
	{
		originalPrefabScale = transform.localScale;
		
		audioDirector = (AudioDirectorScript) GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*
		float newRMS = audioDirector.rmsValue * rmsScalingFactor;

		if( newRMS > currentRMSScalingValue )
		{
			currentRMSScalingValue = newRMS;
		}
		else
		{
		   	currentRMSScalingValue = Mathf.Lerp( currentRMSScalingValue, 1.0f, scalingDecayFactor * Time.deltaTime);
		}

		transform.localScale = originalPrefabScale + (currentRMSScalingValue * originalPrefabScale);
	*/
	}

}
