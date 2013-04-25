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
		AnimatingDeath_GatheringParts,
		AnimatingDeath_SendingoffParts,
		RelinquishingParts
	};
	CreatureStates creatureState = CreatureStates.PreInitialize;

	GameObject[] creaturePartsArray;
	Vector3[] creaturePartsOriginalPositionArray;

	public List<Vector3> positionsRecordingsList;
	public List<Quaternion> rotationsRecordingsList;
	public List<Color> colorsRecordingsList;
	public float pathNodePlaybackInterval = 0.015f;
	public float plabackTimeScale = 1.0f;
	float originalPathTimeLength;
	float currentPathTimeCounter = 0;

	public float forwardSpeed;
	Vector3 positionDisplacement;
	Vector3 initialPositionOffset;

	List<Vector3> partsDeathPositionsList = new List<Vector3>();

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
			case CreatureStates.AnimatingDeath_GatheringParts:
				AnimateDeath_GatherParts();
				break;
			case CreatureStates.AnimatingDeath_SendingoffParts:
				AnimateDeath_SendoffParts();
				break;
			case CreatureStates.RelinquishingParts:
				RelinquishParts();
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
		colorsRecordingsList = new List<Color>( playerScript.colorRecordingList );
		//set color transparency, only works if shader in material is of Transparent/ type
		for(int i = 0; i < colorsRecordingsList.Count; i++)
		{
			Color tempColor = colorsRecordingsList[i];
			tempColor.a = 0.9f;
			colorsRecordingsList[i] = tempColor;
		}
		// get original flight duration
		originalPathTimeLength = playerScript.recordingLength;
		// get spawn offset
		initialPositionOffset = transform.position - positionsRecordingsList[0];
		// wipe the original
		playerScript.positionRecordingList.Clear();
		playerScript.rotationRecordingList.Clear();
		playerScript.colorRecordingList.Clear();
		// change state
		creatureState = CreatureStates.FollowingPath;	

		// set initial color
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].renderer.material.color = colorsRecordingsList[0];
		}	
	}

	void FollowPath()
	{
		float pathProgressionRatio = currentPathTimeCounter / originalPathTimeLength;
		int currentPathNodeIndex = Mathf.FloorToInt(pathProgressionRatio * (float)positionsRecordingsList.Count);
		if(currentPathNodeIndex > positionsRecordingsList.Count -2) // gone through the entire list, arrived at the end of the path
		{
			creatureState = CreatureStates.AnimatingDeath_GatheringParts;
			return;
		}

		float ratioToNextPathNode = (pathProgressionRatio * (float)positionsRecordingsList.Count) - (float)currentPathNodeIndex;
		//updating head part
		creaturePartsArray[0].transform.position = Vector3.Lerp( positionsRecordingsList[currentPathNodeIndex], positionsRecordingsList[currentPathNodeIndex + 1], ratioToNextPathNode);
		creaturePartsArray[0].transform.rotation = Quaternion.Slerp( rotationsRecordingsList[currentPathNodeIndex], rotationsRecordingsList[currentPathNodeIndex + 1], ratioToNextPathNode);
		creaturePartsArray[0].renderer.material.color = Color.Lerp( colorsRecordingsList[currentPathNodeIndex], colorsRecordingsList[currentPathNodeIndex +1], ratioToNextPathNode );

		//update the position displacement
		positionDisplacement += new Vector3(-forwardSpeed * Time.deltaTime * plabackTimeScale,0,0);
		//displace the head with the speed/position displacement vector
		creaturePartsArray[0].transform.position += positionDisplacement + initialPositionOffset;

		// make body parts follow the one in front
		for(int i = 1; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp( creaturePartsArray[i].transform.position, creaturePartsArray[i-1].transform.position, 7 * plabackTimeScale * Time.deltaTime );
			creaturePartsArray[i].transform.rotation = Quaternion.Slerp( creaturePartsArray[i].transform.rotation, creaturePartsArray[i-1].transform.rotation, 7 * plabackTimeScale * Time.deltaTime);
			creaturePartsArray[i].renderer.material.color = Color.Lerp( creaturePartsArray[i].renderer.material.color, creaturePartsArray[i-1].renderer.material.color, 25 * plabackTimeScale * Time.deltaTime);
		}
		currentPathTimeCounter += Time.deltaTime * plabackTimeScale;
	}


	void AnimateDeath_GatherParts()
	{
		Vector3 headPostion = creaturePartsArray[0].transform.position;
		Vector3 lastPartPosition = creaturePartsArray[creaturePartsArray.Length -1].transform.position;
		if( Mathf.Abs(Vector3.Distance(headPostion, lastPartPosition)) > 5) // conitnue body animation until the last piece has caught up sufficiently to the head
		{
			for(int i = 1; i < creaturePartsArray.Length; i++)
			{
				creaturePartsArray[i].transform.position = Vector3.Lerp( creaturePartsArray[i].transform.position, creaturePartsArray[i-1].transform.position, 21 * plabackTimeScale * Time.deltaTime );
				creaturePartsArray[i].transform.rotation = Quaternion.Slerp( creaturePartsArray[i].transform.rotation, creaturePartsArray[i-1].transform.rotation, 21 * plabackTimeScale * Time.deltaTime);
				creaturePartsArray[i].renderer.material.color = Color.Lerp( creaturePartsArray[i].renderer.material.color, creaturePartsArray[i-1].renderer.material.color, 25 * plabackTimeScale * Time.deltaTime);
			}
		}
		else
		{
			// set final posions of parts (back into the pool)
			for(int i = 0; i < creaturePartsArray.Length; i++)
			{
				partsDeathPositionsList.Add(creatureManagerScript.GenerateRandomPointOnSemiSpehere());
				/*
				creaturePartsArray[i].AddComponent("TrailRenderer");
				Material tempMaterial = new Material(Shader.Find("Diffuse"));
				Color tempColor = creaturePartsArray[i].renderer.material.color;
				tempColor.a = 0.1f;
				tempMaterial.SetColor("_Color", tempColor);
				((TrailRenderer)creaturePartsArray[i].GetComponent("TrailRenderer")).material = tempMaterial ;
				((TrailRenderer)creaturePartsArray[i].GetComponent("TrailRenderer")).startWidth = 10;
				((TrailRenderer)creaturePartsArray[i].GetComponent("TrailRenderer")).endWidth = 10;
				*/
			}
			creatureState = CreatureStates.AnimatingDeath_SendingoffParts;
		}

	}

	void AnimateDeath_SendoffParts()
	{
		float deltaCounter = 0;
		Color targetColor = new Color(1,1,1,1);
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.position = Vector3.Lerp( creaturePartsArray[i].transform.position, partsDeathPositionsList[i], 1 * Time.deltaTime );
			deltaCounter += Vector3.Distance( creaturePartsArray[i].transform.position, partsDeathPositionsList[i]);

			creaturePartsArray[i].renderer.material.color = Color.Lerp( creaturePartsArray[i].renderer.material.color, targetColor , 0.2f * Time.deltaTime);
		}
		float deltaAverage = deltaCounter/(float)creaturePartsArray.Length;
		if(deltaAverage < 5)
			creatureState = CreatureStates.RelinquishingParts;
	}

	void RelinquishParts()
	{
		// relinquish all parts to the creature manager
		for(int i = 0; i < creaturePartsArray.Length; i++)
		{
			creaturePartsArray[i].transform.parent = creatureManagerScript.gameObject.transform;
			creaturePartsArray[i].renderer.material.color = new Color(1,1,1,1);
			//Destroy(creaturePartsArray[i].GetComponent("TrailRenderer"));
		}
		// destroy
		Object.Destroy(this.gameObject);
	}


}
