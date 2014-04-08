using UnityEngine;
using System.Collections;

public class MeshLine : MonoBehaviour 
{

	public float maxLifeTime = 5.0f;
	float lifeTimeCounter = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		lifeTimeCounter += Time.deltaTime;

		if(lifeTimeCounter > maxLifeTime)
			Destroy(gameObject, 0.0f);
	
	}


}
