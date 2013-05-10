using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmplitudeDispersalArrayScript : MonoBehaviour 
{

	public GameObject partPrefab;

	public int partsCount;
	public List<Vector3> positionsList = new List<Vector3>();
	List<Vector3> velocitiesList = new List<Vector3>();

	AudioDirectorScript audioDirector;

	public int frequencyMinIndex; // min 0
	public int frequencyMaxIndex; // max 99 (max final data points)

	public float audioHeightScaling = 1.0f;


	// Use this for initialization
	void Start () 
	{

		for(int i = 0; i < partsCount; i++)
		{
			positionsList.Add(new Vector3(30 * i, 0 ,0));
			velocitiesList.Add(new Vector3());
			GameObject tempGameObject = (GameObject)Instantiate(partPrefab, positionsList[i], Quaternion.identity);
			tempGameObject.transform.parent = transform;
			((CreaturePartsGeneralScript)tempGameObject.GetComponent("CreaturePartsGeneralScript")).arrayIndex = i;
			((CreaturePartsGeneralScript)tempGameObject.GetComponent("CreaturePartsGeneralScript")).ownerArrayScript = this;

		}

		audioDirector = (AudioDirectorScript) GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");

	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		float audioAverage = getAmplitudeAverageOverFrequencyRange();
		Debug.Log(audioAverage);

		Vector3 tempVelocity;
		tempVelocity = velocitiesList[0];
		tempVelocity.y += audioAverage * audioHeightScaling;
		velocitiesList[0] = tempVelocity;

		for(int i = 0; i < partsCount; i++)
			positionsList[i] += velocitiesList[i] * Time.deltaTime;

	}


	float getAmplitudeAverageOverFrequencyRange()
	{
		float tempSum = 0;
		for(int i = frequencyMinIndex; i < frequencyMaxIndex; i++)
			tempSum += audioDirector.pseudoLogArray[i];


		float average = tempSum/(float)(frequencyMaxIndex - frequencyMinIndex);
		return average;
	}



}
