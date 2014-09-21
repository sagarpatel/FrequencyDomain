using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{

	public GameObject creditsModelPrefab;

	GameObject creditsModel;

	// Use this for initialization
	void Start () 
	{
		creditsModel = (GameObject)Instantiate(creditsModelPrefab, transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}



}
