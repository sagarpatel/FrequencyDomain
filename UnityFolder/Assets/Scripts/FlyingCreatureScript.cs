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
	public float plabackTimeScale = 1.0f;
	float originalPathTimeLength;
	float currentPathTimeCounter = 0;

	public float forwardSpeed;
	Vector3 positionDisplacement;

	Vector3 initialPositionOffset;

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
		// player hits the ground., need to complete creature formation
		if(playerScript.oldVelocity.y < 0 && playerScript.velocity.y == 0)
		{
			//Debug.Log("Hit Ground");
			for(int i = 0; i < creaturePartsArray.Length; i++)
			{
				creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, 1.0f);
			}
			creatureState = CreatureStates.CollectingPathData;
		}

		// ordinary update
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, Mathf.SmoothStep(0f,1f,playerScript.moveTowardsRatio));
		}
	}

	void CollectPathData()
	{
		// get the data from the original list in the player
		positionsRecordingsList = new List<Vector3>( playerScript.positionRecordingList );
		rotationsRecordingsList = new List<Quaternion>( playerScript.rotationRecordingList );
		// get original flight duration
		originalPathTimeLength = playerScript.recordingLength;
		// get spawn offset
		initialPositionOffset = transform.position - positionsRecordingsList[0];
		// wipe the original
		playerScript.positionRecordingList.Clear();
		playerScript.rotationRecordingList.Clear();
		// change state
		creatureState = CreatureStates.FollowingPath;		
	}

	void FollowPath()
	{
		float pathProgressionRatio = currentPathTimeCounter / originalPathTimeLength;
		int currentPathNodeIndex = Mathf.FloorToInt(pathProgressionRatio * (float)positionsRecordingsList.Count);
		if(currentPathNodeIndex > positionsRecordingsList.Count -2) // gone through the entire list, arrived at the end of the path
		{
			creatureState = CreatureStates.SelfDestructing;
			return;
		}

		float ratioToNextPathNode = (pathProgressionRatio * (float)positionsRecordingsList.Count) - (float)currentPathNodeIndex;
		//updating head part
		creaturePartsArray[0].transform.position = Vector3.Lerp( positionsRecordingsList[currentPathNodeIndex], positionsRecordingsList[currentPathNodeIndex + 1], ratioToNextPathNode);
		creaturePartsArray[0].transform.rotation = Quaternion.Slerp( rotationsRecordingsList[currentPathNodeIndex], rotationsRecordingsList[currentPathNodeIndex + 1], ratioToNextPathNode);
		
		//update the position displacement
		positionDisplacement += new Vector3(-forwardSpeed * Time.deltaTime * plabackTimeScale,0,0);
		//displace the head with the speed/position displacement vector
		creaturePartsArray[0].transform.position += positionDisplacement + initialPositionOffset;

		// make body parts follow the one in front
		for(int i = 1; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp( creaturePartsArray[i].transform.position, creaturePartsArray[i-1].transform.position, 7 * plabackTimeScale * Time.deltaTime );
			creaturePartsArray[i].transform.rotation = Quaternion.Slerp( creaturePartsArray[i].transform.rotation, creaturePartsArray[i-1].transform.rotation, 7 * plabackTimeScale * Time.deltaTime);
		}
		currentPathTimeCounter += Time.deltaTime * plabackTimeScale;
	}

	void SelfDestruct()
	{
		// relinquish all parts to the creature manager
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = new Vector3( Random.Range(-200.0f, -50.0f), Random.Range(-10.0f,200.0f), Random.Range(-100.0f, 600.0f) ); //TOO: This is TEMPORARY until the dragonball dispersal system is implemented
			creaturePartsArray[i].transform.parent = creatureManagerScript.gameObject.transform;
		}
		// destroy
		Object.Destroy(this.gameObject);
	}


}
