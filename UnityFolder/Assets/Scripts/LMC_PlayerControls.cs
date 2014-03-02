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
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);

	}
	
	// Update is called once per frame
	void Update () 
	{
		
		Frame currentFrame = controller.Frame();

		// using palm / hand 		
		// movement and looking
		Hand firstHand = currentFrame.Hands[0];
		HandleMovement( firstHand );


		// warping
		//Hand secondHand = currentFrame.Hands[1]
		playerScript.isLMCWarping = false;
		HandleWarp( firstHand );

	}

	void HandleMovement(Hand currentFrameHand)
	{
		if(currentFrameHand.IsValid)
		{
			horizontalMove =  hMoveScale * currentFrameHand.PalmNormal.Roll / Mathf.PI ;
			verticalMove =  vMoveScale * currentFrameHand.Direction.Pitch / Mathf.PI;
			horizontalLook = hLookScale * currentFrameHand.Direction.Yaw / Mathf.PI;

			// debug visualization
			//Debug.DrawLine(transform.position, transform.position + new Vector3(currentFrameHand.PalmNormal.Roll , 0, 0) * 2.150f, Color.green, 0, false);
			//Debug.DrawLine(transform.position, transform.position + new Vector3(0 , currentFrameHand.Direction.Pitch, 0) * 2.150f, Color.blue, 0, false);
			//Debug.DrawLine(transform.position, transform.position + new Vector3(0 , 0, currentFrameHand.Direction.Yaw) * 2.150f, Color.red, 0, false);
		}
		playerScript.HandleControls(-horizontalMove, -verticalMove);
	}

	void HandleWarp(Hand currentFrameHand)
	{

		if(currentFrameHand.IsValid)
		{
			float ballRadius = currentFrameHand.SphereRadius;
			Debug.Log( ballRadius );
			if(ballRadius < 80)
			{
				playerScript.isLMCWarping = true;
				playerScript.energyCounter += Time.deltaTime * playerScript.currentBoostFactor;
				Debug.Log("WARPING");
			}
		}
	}


}
