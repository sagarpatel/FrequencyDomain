using UnityEngine;
using System.Collections;

public class CameraTypeSelectorScript : MonoBehaviour 
{
	public bool isActive = false;
	float textVerticalOffset = 50.0f;
	float textHorizontalOffset = 400.0f;

	PlayerScript playerScript;

	// Use this for initialization
	void Start () 
	{
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isActive)
		{
			if(Input.GetButtonDown("Toggle Camera Type"))
			{
				playerScript.isOVR = !playerScript.isOVR;
			}
		}
	}


	void OnGUI()
	{
		if(isActive)
		{
	
	    	GUI.Label(new Rect( textHorizontalOffset,textVerticalOffset+0,400,20),"Oculus Rift mode toggle: 'o' key or the B button");
		}

	}

}
