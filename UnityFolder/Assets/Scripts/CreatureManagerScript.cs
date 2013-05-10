using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureManagerScript : MonoBehaviour 
{
	// creatures are composed of 1 head part and 1+ body parts

	public int headPartsCount = 10;
	public int bodyPartsCount = 20;
	public float randomPositionSemiSphereRadius = 500.0f;
	public Vector3 randomPositionSemiSphereCenterOffset;

	public GameObject flyingCreaturePrefab;

	public float playerMinimumJumpVelocity = 10.0f;

	public float creatureForwardSpeed = 1.0f;
	public float creaturePlaybackTimeScale = 1.0f;

	GameObject[] creatureHeadPartsArray;
	GameObject[] creatureBodyPartsArray;

	bool isInitialized = false;

	// Use this for initialization
	void Start () 
	{


	}
	
	// Update is called once per frame
	void Update () 
	{

		if(isInitialized == false)
		{
			creatureHeadPartsArray = GameObject.FindGameObjectsWithTag("CreatureHeadPart");
			creatureBodyPartsArray = GameObject.FindGameObjectsWithTag("CreatureBodyPart");

			isInitialized = true;
		}

	

	}

	public void AttemptSpwanCreature(Vector3 playerPosition, float playerJumpVelocity)
	{
		if(playerJumpVelocity > playerMinimumJumpVelocity)
		{
			// check to see if a creature can be contructed (need 1 head part)
			// search for a free head part
			bool canBuildCreature = false;
			for(int i = 0; i < creatureHeadPartsArray.Length; i++) 
			{
				CreaturePartsGeneralScript creaturePartGeneralScript = (CreaturePartsGeneralScript)creatureHeadPartsArray[i].GetComponent("CreaturePartsGeneralScript");
				if( creaturePartGeneralScript.isPartOfCreature == false )
					canBuildCreature = true;
			}

			if(!canBuildCreature)
				return; //if no head found, can't build creature, exit function

			// now commited to spawning a creature
			List<GameObject> partsForNewCreatureList = new List<GameObject>();

			// search for a free head part
			for(int i = 0; i < creatureHeadPartsArray.Length; i++)  
			{
				CreaturePartsGeneralScript creaturePartGeneralScript = (CreaturePartsGeneralScript)creatureHeadPartsArray[i].GetComponent("CreaturePartsGeneralScript");
				if( creaturePartGeneralScript.isPartOfCreature == false )
				{
					partsForNewCreatureList.Add(creatureHeadPartsArray[i]);
					creaturePartGeneralScript.isPartOfCreature = true;
					creatureHeadPartsArray[i].transform.parent = null;
					break;
				}
			}

			// try to gather the appropriate number of body parts
			int bodyPartsDesiredCounter = (int)(playerJumpVelocity/5.0f);
			for(int i = 0; i < creatureBodyPartsArray.Length; i++)  
			{
				CreaturePartsGeneralScript creaturePartGeneralScript = (CreaturePartsGeneralScript)creatureBodyPartsArray[i].GetComponent("CreaturePartsGeneralScript");
				if( creaturePartGeneralScript.isPartOfCreature == false )
				{
					partsForNewCreatureList.Add(creatureBodyPartsArray[i]);
					creaturePartGeneralScript.isPartOfCreature = true;
					creatureBodyPartsArray[i].transform.parent = null;

					bodyPartsDesiredCounter --;
				}
				if(bodyPartsDesiredCounter == 0)
					break;
			}

			GameObject[] partsForNewCreatureArray = partsForNewCreatureList.ToArray();
			
			playerPosition += new Vector3(-120,0,0); // to make creature appear in fron of player so they can see it assemble and born
			
			GameObject newCreature = (GameObject)Instantiate( flyingCreaturePrefab, playerPosition, Quaternion.identity);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).AquireCreatureParts(partsForNewCreatureArray);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).forwardSpeed = creatureForwardSpeed;
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).plabackTimeScale = creaturePlaybackTimeScale;
		}


	}

	public Vector3 GenerateRandomPointOnSemiSpehere()
	{
		Vector3 randomPosition = Random.onUnitSphere * randomPositionSemiSphereRadius;

		while(randomPosition.y < 0) //make sure that point is on the top half of the sphere only
			randomPosition = Random.onUnitSphere * randomPositionSemiSphereRadius;

		// adjust with offset
		randomPosition += randomPositionSemiSphereCenterOffset;
		
		return randomPosition;
	}



}
