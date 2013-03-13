using UnityEngine;
using System.Collections;

public class LevelSelectDirectorScript : MonoBehaviour 
{
	public float levelLoadFOV = 179.0f;
	Camera mainCamera;
	// Use this for initialization
	void Start () 
	{
		mainCamera = (Camera)GameObject.Find("Main Camera").GetComponent("Camera");
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		float fov = mainCamera.fieldOfView;

		if( fov > levelLoadFOV )
			Application.LoadLevel("Aliceeffekt_T2");
	
	}


}
