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

	float[] sphereRadiusRollingAverageArray;
	int sphereRadiusRollingAverageLength = 10;	
	int sphereRadusRollingAverageCurrentIndex = 0;
	public float sphereRadiusRollingAverage = 0;

	// Use this for initialization
	void Start () 
	{
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
		// Leap stuff
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);

		sphereRadiusRollingAverageArray = new float[sphereRadiusRollingAverageLength];

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

			sphereRadiusRollingAverageArray[sphereRadusRollingAverageCurrentIndex] = ballRadius;
			sphereRadusRollingAverageCurrentIndex += 1;
			if(sphereRadusRollingAverageCurrentIndex >= sphereRadiusRollingAverageLength)
				sphereRadusRollingAverageCurrentIndex = 0;

			for(int i =0; i < sphereRadiusRollingAverageArray.Length; i++ )
			{
				sphereRadiusRollingAverage += sphereRadiusRollingAverageArray[i]; 
			}
			sphereRadiusRollingAverage = sphereRadiusRollingAverage / sphereRadiusRollingAverageArray.Length;

			//Debug.Log( sphereRadiusRollingAverage < 70 );
			if(sphereRadiusRollingAverage < 70)
			{
				playerScript.isLMCWarping = true;
				playerScript.energyCounter += 0.25f * Time.deltaTime * playerScript.currentBoostFactor;
				Debug.Log("WARPING");
			}
		}
	}


}
