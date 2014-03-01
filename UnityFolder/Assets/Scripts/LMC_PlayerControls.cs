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
		
		// using points translation
		/*
		Vector3 translationLMC = UnityVectorExtension.ToUnityScaled(currentFrame.Translation( controller.Frame(1) ) ) ;
		Debug.Log( translationLMC );
		transform.Translate(translationLMC, Space.Self);

		playerScript.HandleControls(translationLMC.x, translationLMC.y);
		*/

		// using palm / hand 

		Hand firstHand = currentFrame.Hands[0];
		if(firstHand.IsValid)
		{

			float LMCValue  = Quaternion.LookRotation(firstHand.Direction.ToUnity(), -firstHand.PalmNormal.ToUnity()).eulerAngles.z;

			Debug.Log(  firstHand.PalmNormal.Roll );
			Vector3 palmNormal = UnityVectorExtension.ToUnity( firstHand.Direction );
			Debug.DrawLine(transform.position, transform.position + new Vector3(firstHand.PalmNormal.Roll , 0, 0) * 2.150f, Color.green, 0, false);

			//Debug.DrawLine(transform.position, transform.position + firstHand.Direction.ToUnity() * 2.150f, Color.green, 0, false);
			//Debug.DrawLine(transform.position, transform.position + -firstHand.PalmNormal.ToUnity() * 2.150f, Color.red, 0, false);
		}

	}


}
