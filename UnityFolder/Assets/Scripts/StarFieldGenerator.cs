using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarFieldGenerator : MonoBehaviour 
{

	public GameObject startModel;

	public float starCount = 1000;

	[Range(100,10000)]
	public float xRange = 100.0f;	
	[Range(100,10000)]
	public float yRange = 100.0f;	
	[Range(100,10000)]
	public float zRange = 100.0f;	

	List<GameObject> starsList = new List<GameObject>();
	public float modelScale = 1.0f;

	public int randomSeed = 1;

	public void GenerateStarField()
	{
		Random.seed = randomSeed;
		randomSeed ++;

		Vector3 spawnPosition;
		float randX;
		float randY;
		float randZ;
		for(int i = 0; i < starCount; i ++)
		{
			randX = Random.Range(-xRange, xRange);
			randY = Random.Range(-yRange, yRange);
			randZ = Random.Range(-zRange, zRange);

			spawnPosition = new Vector3(randX, randY, randZ);

			starsList.Add( (GameObject) Instantiate(startModel, spawnPosition, Quaternion.identity) );
			starsList[i].transform.localScale = Vector3.one * modelScale;
		}
	}


	public void DeleteAllStarsInScene()
	{
		GameObject[] allStars = GameObject.FindGameObjectsWithTag("Star");
		for(int i = 0; i < allStars.Length; i++)
		{
			DestroyImmediate(allStars[i]);
		}
	}
}
