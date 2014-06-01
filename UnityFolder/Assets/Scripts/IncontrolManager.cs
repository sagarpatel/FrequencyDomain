using UnityEngine;
using System.Collections;
using InControl;

public class IncontrolManager : MonoBehaviour 
{

	
	// Use this for initialization
	void Start () 
	{
		InputManager.Setup();
		// Add a custom device profile.
		InputManager.AttachDevice( new UnityInputDevice( new InControlFDProfile() ) );

		Debug.Log( "InControl (version " + InputManager.Version + ")" );
	}
	
	// Update is called once per frame
	void Update () 
	{
		InputManager.Update();
	}


}
