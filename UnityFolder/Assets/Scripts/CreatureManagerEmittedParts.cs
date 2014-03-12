using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureManagerEmittedParts : MonoBehaviour 
{
	// creatures are composed of 1 Head part and 1+ body parts

	public float randomPositionSemiSphereRadius = 500.0f;
	public Vector3 randomPositionSemiSphereCenterOffset;

	public GameObject flyingCreaturePrefab;

	public float playerMinimumJumpVelocity = 10.0f;

	public float creatureForwardSpeedStart = 100.0f;
	public float creatureForwardSpeedEnd = 100.0f;

	public float creaturePlaybackTimeScale = 1.0f;

	public Vector3 creatureSpawnPositionOffset = new Vector3(-150,0,0);

	GameObject[] creatureBodyPartsArray;

	// Use this for initialization
	void Start () 
	{


	}
	
	// Update is called once per frame
	void Update () 
	{

		creatureBodyPartsArray = GameObject.FindGameObjectsWithTag("CreatureBodyPart");

		if(creatureBodyPartsArray.Length != 0)
		{
			// sort parts
			sortBodyParts();
		}
	
	}

	public void AttemptSpwanCreature(Vector3 playerPosition, float playerJumpVelocity)
	{
		if(playerJumpVelocity > playerMinimumJumpVelocity)
		{
			// check to see if a creature can be contructed (need 1 Head part)
			// search for a free body part
			bool canBuildCreature = false;
			for(int i = 0; i < creatureBodyPartsArray.Length; i++) 
			{
				CreaturePartsGeneralScript creaturePartGeneralScript = (CreaturePartsGeneralScript)creatureBodyPartsArray[i].GetComponent("CreaturePartsGeneralScript");
				if( creaturePartGeneralScript.isPartOfCreature == false )
					canBuildCreature = true;
			}

			if(!canBuildCreature)
				return; //if no Head found, can't build creature, exit function

			// now commited to spawning a creature
			List<GameObject> partsForNewCreatureList = new List<GameObject>();

			// randommly order array to ensure not always the first ones get picked,
			//creatureHeadPartsArray.Shuffle();

		
			// try to gather the appropriate number of body parts
			int bodyPartsDesiredCounter = (int)(playerJumpVelocity/5.0f);
			//creatureBodyPartsArray.Shuffle();

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
			
			playerPosition += creatureSpawnPositionOffset;//new Vector3(-100,0,0); // to make creature appear in fron of player so they can see it assemble and born
			//playerPosition += new Vector3(0, - playerPosition.y * 0.2f, 0); // reduce height
			
			GameObject newCreature = (GameObject)Instantiate( flyingCreaturePrefab, playerPosition, Quaternion.identity);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).AquireCreatureParts(partsForNewCreatureArray);
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).forwardSpeedStart = creatureForwardSpeedStart;
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).forwardSpeedEnd = creatureForwardSpeedEnd;
			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).plabackTimeScale = creaturePlaybackTimeScale;

			((FlyingCreatureScript)newCreature.GetComponent("FlyingCreatureScript")).spawnPlayerJumpVelocity = playerJumpVelocity;
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


	void sortBodyParts()
	{
		List<GameObject> tempBodyList = new List<GameObject>();
		tempBodyList.Add(creatureBodyPartsArray[0]);
		for(int i = 1; i < creatureBodyPartsArray.Length; i++)
		{
			int currentPartIndex = ((CreaturePartsGeneralScript)creatureBodyPartsArray[i].GetComponent("CreaturePartsGeneralScript")).arrayIndex;
			
			int oldListCount = tempBodyList.Count;

			foreach(GameObject part in tempBodyList)
			{
				int comparisonPartIndex = ((CreaturePartsGeneralScript)part.GetComponent("CreaturePartsGeneralScript")).arrayIndex;
				if( currentPartIndex >= comparisonPartIndex ) // highest index at top
				{
					tempBodyList.Insert(tempBodyList.IndexOf(part), creatureBodyPartsArray[i]);
					break;
				}

			}
			int newListCount = tempBodyList.Count;
			if( newListCount == oldListCount ) // if not insterted above anything else
				tempBodyList.Add(creatureBodyPartsArray[i]);
		}

		creatureBodyPartsArray = tempBodyList.ToArray();

	}


}

/*
// shuffling list randomly, using Fisher Yates, from --> http://www.bytechaser.com/en/functions/p6sv9tve9v/randomly-shuffle-contents-of-any-list-in-c-sharp.aspx
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var randomNumber = new System.Random(Time.frameCount);
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = randomNumber.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

*/