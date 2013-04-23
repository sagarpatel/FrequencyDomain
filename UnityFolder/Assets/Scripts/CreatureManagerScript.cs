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
																		new Vector3( Random.Range(-400.0f, -50.0f), Random.Range(-100.0f,100.0f), Random.Range(-100.0f, 600.0f) ),
																		Quaternion.identity );
			creaturePartsArray[partsCounter].transform.parent = transform;
			partsCounter ++;
		}
		for(int i = 0; i < bodyPartsCount ; i++)
		{
			
			creaturePartsArray[partsCounter] = (GameObject)Instantiate( creatureBodyPartPrefab,
																		new Vector3( Random.Range(-400.0f, -50.0f), Random.Range(-100.0f,100.0f),Random.Range(-100.0f, 600.0f) ),
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
			int bodyPartsDesiredCounter = (int)(playerJumpVelocity/10.0f);
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
			
			GameObject newCreature = (GameObject)Instantiate( flyingCreaturePrefab, playerPosition, Quaternion.identity);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).AquireCreatureParts(creaturePartsArray);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).forwardSpeed = creatureForwardSpeed;
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).plabackTimeScale = creaturePlaybackTimeScale;
		}


	}



}
