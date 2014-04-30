using UnityEngine;
using System.Collections;

public class MeshLine : MonoBehaviour 
{

	public float maxLifeTime = 5.0f;
	float lifeTimeCounter = 0;

	Vector3 tempVector;

	Transform lineTransform;

	// Use this for initialization
	void Start () 
	{
		tempVector = new Vector3(0, 0, 0);
		lineTransform = transform.GetChild(0); // assuming only child
	}
	/*
	void Awake()
	{
		lineTransform = transform.GetChild(0); // assuming only child	
	}
	*/

	// Update is called once per frame
	void Update () 
	{

		lifeTimeCounter += Time.deltaTime;

		if(lifeTimeCounter > maxLifeTime)
		{
			lifeTimeCounter = 0;
			gameObject.SetActive(false);
		}
	
	}
	/*
	public void UpdateCenter(float verticesFrequencyDepthCount, float verticesSpread)
	{
		float xOffset = -0.5f  * verticesFrequencyDepthCount * verticesSpread;
		tempVector = lineTransform.localPosition;
		tempVector.x = xOffset;

		lineTransform.localPosition = tempVector;
	}
	*/


}
