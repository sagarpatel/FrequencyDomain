using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingCreatureScript : MonoBehaviour 
{
	enum CreatureStates
	{
		PreInitialize,
		AssemblingParts,
		Alive,
		Dead
	};

	CreatureStates creatureState = CreatureStates.PreInitialize;

	GameObject[] creaturePartsArray;

	PlayerScript playerScript;

	// Use this for initialization
	void Start () 
	{
		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");
	
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(creatureState)
		{
			case CreatureStates.AssemblingParts:
				AssembleCreature();
				break;
		}
	
	}

	public void AquireCreatureParts(GameObject[] partsArray)
	{
		List<GameObject> creaturePartsList = new List<GameObject>();
		for(int i = 0; i < partsArray.Length; i++ )
		{
			partsArray[i].transform.parent = transform;
			creaturePartsList.Add(partsArray[i]);
		}
		creaturePartsArray = creaturePartsList.ToArray();
		creatureState = CreatureStates.AssemblingParts;

	}

	void AssembleCreature()
	{
		float tempDistance;
		float currentDistance;
		float targetDistance;
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			currentDistance = Vector3.Distance(creaturePartsArray[i].transform.position, transform.position);
			targetDistance = currentDistance * playerScript.moveTowardsRatio;
			tempDistance = currentDistance - targetDistance;
			creaturePartsArray[i].transform.position = Vector3.MoveTowards(creaturePartsArray[i].transform.position, transform.position, tempDistance);

		}
		Debug.Log(playerScript.moveTowardsRatio);

	}


}
