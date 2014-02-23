using UnityEngine;
using System.Collections;
using Leap;

public class LMC_PlayerControls : MonoBehaviour 
{

	PlayerScript playerScript;

	Controller controller;

	// Use this for initialization
	void Start () 
	{
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
		// Leap stuff
		controller = new Controller();

	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Frame currentFrame = controller.Frame();
		Vector3 translationLMC = UnityVectorExtension.ToUnityScaled(currentFrame.Translation( controller.Frame(1) ) ) ;
		Debug.Log( translationLMC );
		transform.Translate(translationLMC, Space.Self);

	}


}
