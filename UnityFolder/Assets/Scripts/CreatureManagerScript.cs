using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureManagerScript : MonoBehaviour 
{
	// creatures are composed of 1 head part and 1+ body parts

	public GameObject creatureHeadPartPrefab;
	public GameObject creatureBodyPartPrefab;
	public int headPartsCount = 10;
	public int bodyPartsCount = 20;
	public float randomPositionSemiSphereRadius = 500.0f;
	public Vector3 randomPositionSemiSphereCenterOffset;
	GameObject[] creaturePartsArray;

	public GameObject flyingCreaturePrefab;

	public float playerMinimumJumpVelocity = 10.0f;

	public float creatureForwardSpeed = 1.0f;
	public float creaturePlaybackTimeScale = 1.0f;

	// Use this for initialization
	void Start () 
	{

		creaturePartsArray = new GameObject[headPartsCount + bodyPartsCount];
		int partsCounter = 0;
		for(int i = 0; i < headPartsCount ; i++)
		{
			creaturePartsArray[partsCounter] = (GameObject)Instantiate( creatureHeadPartPrefab,
																		GenerateRandomPointOnSemiSpehere(),
																		Quaternion.identity );
			creaturePartsArray[partsCounter].transform.parent = transform;
			partsCounter ++;
		}
		for(int i = 0; i < bodyPartsCount ; i++)
		{
			
			creaturePartsArray[partsCounter] = (GameObject)Instantiate( creatureBodyPartPrefab,
																		GenerateRandomPointOnSemiSpehere(),
																		Quaternion.identity );
			creaturePartsArray[partsCounter].transform.parent = transform;
			partsCounter ++;
		}

		

	}
	
	// Update is called once per frame
	void Update () 
	{

	

	}

	public void AttemptSpwanCreature(Vector3 playerPosition, float playerJumpVelocity)
	{
		if(playerJumpVelocity > playerMinimumJumpVelocity)
		{
			// check to see if a creature can be contructed (need 1 head part)
			// search for a free head part
			bool canBuildCreature = false;
			foreach (Transform child in transform) 
			{
				if(child.gameObject.tag == "CreatureHeadPart")
					canBuildCreature = true;
			}

			if(!canBuildCreature)
				return; //if no head found, can't build creature, exit function

			List<GameObject> creaturePartsList = new List<GameObject>();

			// search for a free head part
			foreach (Transform child in transform) 
			{
				if(child.gameObject.tag == "CreatureHeadPart")
				{
					creaturePartsList.Add(child.gameObject);
					child.transform.parent = null;
					break;
				}
			}

			// try to gather the appropriate number of body parts
			int bodyPartsDesiredCounter = (int)(playerJumpVelocity/5.0f);
			foreach (Transform child in transform) 
			{
				if(child.gameObject.tag == "CreatureBodyPart")
				{
					creaturePartsList.Add(child.gameObject);
					child.transform.parent = null;
					bodyPartsDesiredCounter --;
				}
				if(bodyPartsDesiredCounter == 0)
					break;
			}

			GameObject[] creaturePartsArray = creaturePartsList.ToArray();
			
			playerPosition += new Vector3(-120,0,0); // to make creature appear in fron of player so they can see it assemble and born
			
			GameObject newCreature = (GameObject)Instantiate( flyingCreaturePrefab, playerPosition, Quaternion.identity);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).AquireCreatureParts(creaturePartsArray);
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
