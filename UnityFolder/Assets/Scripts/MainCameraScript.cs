using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour 
{
	GameObject meshFieldGenerator;
	public float distanceAhead = 0;
	Vector3 cameraTarget = new Vector3();

	// Use this for initialization
	void Start () 
	{
		meshFieldGenerator = GameObject.Find("MainMeshField");
	}
	
	// Update is called once per frame
	void Update () 
	{
		cameraTarget = transform.parent.position;
		cameraTarget.x -= distanceAhead;

		transform.LookAt( cameraTarget, Vector3.up );
	}

}
