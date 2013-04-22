using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingCreatureScript : MonoBehaviour 
{
	enum CreatureStates
	{
		PreInitialize,
		AssemblingParts,
		CollectingPathData,
		FollowingPath,
		SelfDestructing
	};
	CreatureStates creatureState = CreatureStates.PreInitialize;

	GameObject[] creaturePartsArray;
	Vector3[] creaturePartsOriginalPositionArray;

	public List<Vector3> positionsRecordingsList;
	public List<Quaternion> rotationsRecordingsList;
	public float pathNodePlaybackInterval = 0.015f;
	float pathNodePlaybackIntervalCounter = 0;
	int pathNodeCounter = 0;

	public Vector3 playbackPositionDisplacement = new Vector3(-100,0,0);

	PlayerScript playerScript;
	CreatureManagerScript creatureManagerScript;


	// Use this for initialization
	void Start () 
	{
		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");
		creatureManagerScript = (CreatureManagerScript)GameObject.Find("CreatureManager").GetComponent("CreatureManagerScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(creatureState)
		{
			case CreatureStates.AssemblingParts:
				AssembleCreature();
				break;
			case CreatureStates.CollectingPathData:
				CollectPathData();
				break;
			case CreatureStates.FollowingPath:
				FollowPath();
				break;
			case CreatureStates.SelfDestructing:
				SelfDestruct();
				break;

		}
	
	}

	public void AquireCreatureParts(GameObject[] partsArray)
	{
		List<GameObject> creaturePartsList = new List<GameObject>();
		List<Vector3> creaturePartsOriginalPositionList = new List<Vector3>();
		for(int i = 0; i < partsArray.Length; i++ )
		{
			partsArray[i].transform.parent = transform;
			creaturePartsList.Add(partsArray[i]);
			creaturePartsOriginalPositionList.Add(partsArray[i].transform.position);
		}
		creaturePartsArray = creaturePartsList.ToArray();
		creaturePartsOriginalPositionArray = creaturePartsOriginalPositionList.ToArray();
		creatureState = CreatureStates.AssemblingParts;

	}

	void AssembleCreature()
	{
		float tempDistance;
		float currentDistance;
		float targetDistance;
		// player hits the ground., need to complete creature formation
		if(playerScript.oldVelocity.y < 0 && playerScript.velocity.y == 0)
		{
			Debug.Log("Hit Ground");
			for(int i = 0; i < creaturePartsArray.Length; i++)
			{
				creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, 1.0f);
			}
			creatureState = CreatureStates.CollectingPathData;
		}

		// ordinary update
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, playerScript.moveTowardsRatio);
		}
		//Debug.Log(playerScript.moveTowardsRatio);
	}

	void CollectPathData()
	{
		// get the data from the original list in the player
		positionsRecordingsList = new List<Vector3>( playerScript.positionRecordingList );
		rotationsRecordingsList = new List<Quaternion>( playerScript.rotationRecordingList );
		// wipe the original
		playerScript.positionRecordingList.Clear();
		playerScript.rotationRecordingList.Clear();
		// change state
		creatureState = CreatureStates.FollowingPath;

		Debug.Log(positionsRecordingsList.Count);
	}

	void FollowPath()
	{
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position =  Vector3.Lerp( creaturePartsArray[i].transform.position, positionsRecordingsList[pathNodeCounter], pathNodePlaybackIntervalCounter/pathNodePlaybackInterval);
			creaturePartsArray[i].transform.position += playbackPositionDisplacement;
			creaturePartsArray[i].transform.rotation = Quaternion.Slerp( creaturePartsArray[i].transform.rotation, rotationsRecordingsList[pathNodeCounter], pathNodePlaybackIntervalCounter/pathNodePlaybackInterval);

		}
		/*
		transform.position = Vector3.Lerp( transform.position, positionsRecordingsList[pathNodeCounter], pathNodePlaybackIntervalCounter/pathNodePlaybackInterval);
		transform.position += playbackPositionDisplacement;
		transform.rotation = Quaternion.Slerp( transform.rotation, rotationsRecordingsList[pathNodeCounter], pathNodePlaybackIntervalCounter/pathNodePlaybackInterval);
*/
		if(pathNodePlaybackIntervalCounter > pathNodePlaybackInterval)
		{
			pathNodePlaybackIntervalCounter -= pathNodePlaybackInterval;
			pathNodeCounter++;
		}
		pathNodePlaybackIntervalCounter += Time.deltaTime;

		if(pathNodeCounter > positionsRecordingsList.Count -1) // gone through the entire list, arrived at the end of the path
			creatureState = CreatureStates.SelfDestructing;

	}

	void SelfDestruct()
	{
		// relinquish all parts to the creature manager
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = new Vector3(0,0,0); //TOO: This is TEMPORARY until the dragonball dispersal system is implemented
			creaturePartsArray[i].transform.parent = creatureManagerScript.gameObject.transform;
		}
		// destroy
		Object.Destroy(this.gameObject);

	}


}
