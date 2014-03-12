using UnityEngine;
using System.Collections;

public class CreaturePart : MonoBehaviour 
{
	float lifetime = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifetime += Time.deltaTime;

		if(lifetime > 6.0f)
			Destroy(gameObject, 0.0f);
	}
}
