using UnityEngine;
using System.Collections;

public class LevelSelectDirectorScript : MonoBehaviour 
{
	public float levelLoadFOV = 179.0f;
	Camera mainCamera;
	public int nextLevelIndex;

	// Use this for initialization
	void Start () 
	{
		mainCamera = (Camera)GameObject.FindWithTag("MainCamera").GetComponent("Camera"); // read only, don't need to account for L+R cameras

		nextLevelIndex =  Application.loadedLevel + 1;
		if( nextLevelIndex > 5 )
			nextLevelIndex = 0;
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		float fov = mainCamera.fieldOfView;

		if( fov > levelLoadFOV )
			Application.LoadLevel(nextLevelIndex);

		if( Input.GetKey("0") )
			Application.LoadLevel(0);

		if( Input.GetKey("1") )
			Application.LoadLevel(1);

		if( Input.GetKey("2") )
			Application.LoadLevel(2);

		if( Input.GetKey("3") )
			Application.LoadLevel(3);

		if( Input.GetKey("4") )
			Application.LoadLevel(4);

		if( Input.GetKey("5") )
			Application.LoadLevel(5);
/*
		if( Input.GetKey("6") )
			Application.LoadLevel(6);

		if( Input.GetKey("7") )
			Application.LoadLevel(7);

		if( Input.GetKey("8") )
			Application.LoadLevel(8);

*/
	
	}


}
