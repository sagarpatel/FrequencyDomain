using UnityEngine;
using System.Collections;

public class CreaturePart : MonoBehaviour 
{
	public float lifetime = 0;
	public bool isPartOfCreature = false;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifetime += Time.deltaTime;

		if(lifetime > 4.0f)
		{
			if(isPartOfCreature == false)
				Destroy(gameObject, 0.0f);
		}
	}
}
