using UnityEngine;
using System.Collections;

public class CreaturePartsGeneralScript : MonoBehaviour 
{
	public bool isPartOfCreature = false;
	public Transform originalArrayTransform;
	public int arrayIndex =0;

	public AmplitudeDispersalArrayScript ownerArrayScript;

	public Vector3 calculatedWorldPosition;

	// Use this for initialization
	void Start () 
	{
		// the part should have been spawned by a array, need to return there once creature is dead
		originalArrayTransform = transform.parent;

	}
	
	// Update is called once per frame
	void Update () 
	{

		// when in array with, act accordingly
		if(isPartOfCreature == false)
		{
			transform.parent = originalArrayTransform;
			transform.localPosition = ownerArrayScript.positionsList[arrayIndex]; 
		}
	
	}


}
