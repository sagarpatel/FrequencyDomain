using UnityEngine;
using System.Collections;

public class FlyingCreatureScript : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void AssembleCreature(GameObject[] partsArray)
	{
		
		for(int i = 0; i < partsArray.Length; i++ )
		{
			partsArray[i].transform.parent = transform;
			//Debug.Log(i);
		}

	}


}
