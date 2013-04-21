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
	Vector3[] creaturePartsOriginalPositionArray;

	PlayerScript playerScript;

	float previousRatio = 0;

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
		float currentRatio = playerScript.moveTowardsRatio;
		if(playerScript.oldVelocity.y < 0 && playerScript.velocity.y == 0)
		{
			currentRatio = previousRatio;
			Debug.Log("Hit Ground");
		}
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp(creaturePartsOriginalPositionArray[i], transform.position, playerScript.moveTowardsRatio );
		}
		//Debug.Log(playerScript.moveTowardsRatio);
		previousRatio = currentRatio;
	}


}
