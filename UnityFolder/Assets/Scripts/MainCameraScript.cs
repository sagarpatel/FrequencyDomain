using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour 
{
	GameObject meshFieldGenerator;
	public float screenCenter = 0;
	public int frequencyCount = 0;

	// Use this for initialization
	void Start () 
	{
		meshFieldGenerator = GameObject.Find("MainMeshField");
		frequencyCount = ((MeshFieldGeneratorScript)meshFieldGenerator.GetComponent("MeshFieldGeneratorScript")).verticesFrequencyDepthCount;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float frequencyScale = meshFieldGenerator.transform.localScale.z;
		screenCenter = (frequencyScale * (float)frequencyCount)/2.0f;

		transform.LookAt( new Vector3(0,-10,screenCenter), Vector3.up );
	}

}
