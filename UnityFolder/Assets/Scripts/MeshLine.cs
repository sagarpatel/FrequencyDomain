using UnityEngine;
using System.Collections;

public class MeshLine : MonoBehaviour 
{

	public float maxLifeTime = 5.0f;
	float lifeTimeCounter = 0;

	Vector3 tempVector;

	Transform lineTransform;

	public Vector3[] meshlineVerticesArray;
	MeshLinesGenerator meshlinesGenerator;

	// Use this for initialization
	void Start () 
	{
		tempVector = new Vector3(0, 0, 0);
		lineTransform = transform.GetChild(0); // assuming only child
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
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
			meshlinesGenerator.RemoveMeshLineFromActiveList(gameObject);
		}
	
	}

	// 0 for relativesOnLine means left most part of line, value mus be between 0 and 1
	public float CalculateHeighOnLine(float relativeOnLine)
	{
		if(meshlineVerticesArray == null)
		{
			Debug.LogWarning("MeshLine component vertices not yet initialized, failed to get height value ");
			return 0;
		}

		relativeOnLine = 1.0f - relativeOnLine; // because mesh line is mirrored on x axis, ned to flip here

		float posOnLine = relativeOnLine * (float)(meshlineVerticesArray.Length -1);
		int floorIndex = Mathf.FloorToInt(posOnLine);
		int ceilIndex = Mathf.FloorToInt(posOnLine);

		float lerpStep = (float)ceilIndex - posOnLine;

		float heightValue = transform.localScale.y *  Mathf.Lerp(meshlineVerticesArray[floorIndex].y, meshlineVerticesArray[ceilIndex].y, lerpStep);
		return heightValue;

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
