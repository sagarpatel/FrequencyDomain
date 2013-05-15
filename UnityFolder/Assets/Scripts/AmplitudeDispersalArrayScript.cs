using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmplitudeDispersalArrayScript : MonoBehaviour 
{

	public GameObject partPrefab;

	public int partsCount;
	public List<Vector3> positionsList = new List<Vector3>();
	public List<Quaternion> rotationsList = new List<Quaternion>();
	List<Vector3> velocitiesList = new List<Vector3>();

	AudioDirectorScript audioDirector;

	public int frequencyMinIndex; // min 0
	public int frequencyMaxIndex; // max 99 (max final data points)

	public float audioHeightScaling = 1.0f;

	public float dispersalUpdateMinimum = 0.02f;
	float dispersalUpdateCounter = 0;

	float oldHeight;
	public float gravity = 1.0f;

	// Use this for initialization
	void Start () 
	{

		for(int i = 0; i < partsCount; i++)
		{
			positionsList.Add(new Vector3( 400.0f * Mathf.Cos( (Mathf.PI/2) + (Mathf.PI/2) * ((float)i/(float)partsCount) ), 0 , -800.0f * Mathf.Sin( (Mathf.PI/2) + (Mathf.PI/2) * ((float)i/(float)partsCount) ) ) );
			rotationsList.Add(Quaternion.identity);
			//Debug.Log(positionsList[i]);
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

		dispersalUpdateCounter += Time.deltaTime;
		if(dispersalUpdateCounter > dispersalUpdateMinimum)
		{
			dispersalUpdateCounter -= dispersalUpdateMinimum;

		}
		
		float audioAverage = getAmplitudeAverageOverFrequencyRange();
		//Debug.Log(audioAverage);

		Vector3 tempPosition = positionsList[0];
		if( audioAverage * audioHeightScaling > oldHeight )
			tempPosition.y  =  audioAverage * audioHeightScaling; //Mathf.Lerp( tempPosition.y, audioAverage * audioHeightScaling, 3.0f * Time.deltaTime);
		else
		{
			tempPosition.y -= gravity * Time.deltaTime; //tempPosition.y  = Mathf.Lerp( tempPosition.y, audioAverage * audioHeightScaling, 1.0f * Time.deltaTime);
			if(tempPosition.y <0)
				tempPosition.y = 0;
		}
		positionsList[0] = tempPosition;

		oldHeight = positionsList[0].y;

		// disperse vaules

		for(int i = partsCount -1; i > 0 ; i--)
		{
			float newHeight = Mathf.Lerp( positionsList[i].y, positionsList[i-1].y, dispersalUpdateCounter/dispersalUpdateMinimum);
			tempPosition = positionsList[i];
			tempPosition.y = newHeight;
			positionsList[i] = tempPosition;
		}

		//update roations list
		for(int i = 0; i < rotationsList.Count; i++)
		{
			Quaternion tempRotation = Quaternion.AngleAxis(positionsList[i].y * Time.deltaTime * 3.0f, Vector3.forward);
			rotationsList[i] *= tempRotation;
		}


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
