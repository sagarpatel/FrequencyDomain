using UnityEngine;
using System.Collections;
using Leap;

public class LMC_PlayerControls : MonoBehaviour 
{

	PlayerScript playerScript;

	Controller controller;

	public float hMoveScale = 1.0f;
	public float vMoveScale = 1.0f;
	public float hLookScale = 1.0f;

	public float horizontalMove;
	public float verticalMove;
	public float horizontalLook;

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

		// using palm / hand 
		
		Hand firstHand = currentFrame.Hands[0];
		if(firstHand.IsValid)
		{
			horizontalMove =  hMoveScale * firstHand.PalmNormal.Roll / Mathf.PI ;
			verticalMove =  vMoveScale * firstHand.Direction.Pitch / Mathf.PI;
			horizontalLook = hLookScale * firstHand.Direction.Yaw / Mathf.PI;

			// debug visualization
			//Debug.DrawLine(transform.position, transform.position + new Vector3(firstHand.PalmNormal.Roll , 0, 0) * 2.150f, Color.green, 0, false);
			//Debug.DrawLine(transform.position, transform.position + new Vector3(0 , firstHand.Direction.Pitch, 0) * 2.150f, Color.blue, 0, false);
			//Debug.DrawLine(transform.position, transform.position + new Vector3(0 , 0, firstHand.Direction.Yaw) * 2.150f, Color.red, 0, false);
		}

		playerScript.HandleControls(-horizontalMove, -verticalMove);



	}


}
