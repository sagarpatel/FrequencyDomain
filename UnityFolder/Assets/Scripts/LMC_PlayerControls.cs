using UnityEngine;
using System.Collections;
using Leap;

public class LMC_PlayerControls : MonoBehaviour 
{

	PlayerScript playerScript;
	AcrobaticsScript acrobaticsScript;

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
	public float sphereRadiusRollingAverage = 100;

	float ballRadius = 0;

	// Use this for initialization
	void Start () 
	{
		playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
		acrobaticsScript = GameObject.FindWithTag("CameraHolder").GetComponent<AcrobaticsScript>();
		
		// Leap stuff
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		controller.Config.SetFloat("Gesture.Circle.MinArc", Mathf.PI );
		controller.Config.Save();

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
		HandleBarrelRoll(currentFrame);
	}

	void HandleMovement(Hand currentFrameHand)
	{
		horizontalMove = 0;
		verticalMove = 0;
		horizontalLook = 0;

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
		if(Mathf.Abs(horizontalMove) < 5)
		{
			acrobaticsScript.RotateToInitialRotation();
		}
		else
		{
			playerScript.HandleControls(-horizontalMove, -verticalMove);
		}

		//Debug.Log(horizontalMove);
	}

	void HandleWarp(Hand currentFrameHand)
	{
		ballRadius = 0;

		if(currentFrameHand.IsValid)
		{
			ballRadius = currentFrameHand.SphereRadius;

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
				//playerScript.isLMCWarping = true;
				//playerScript.energyCounter += 0.25f * Time.deltaTime * playerScript.currentBoostFactor;
				//Debug.Log("WARPING");
			}
		}
	}

	void HandleBarrelRoll(Frame frame)
	{
		// circle drawing style
		/*
		if(!frame.Gestures().IsEmpty)
		{
			Gesture gesture = frame.Gestures()[0];
			CircleGesture circleGesture = new CircleGesture(gesture);

			float direction = 1.0f;
			if (circleGesture.Pointable.Direction.AngleTo(circleGesture.Normal) <= Mathf.PI/2) 
				direction *= 1.0f;
			else
				direction *= -1.0f;


			//Debug.Log(circleGesture.Progress);
			//Debug.Log(gesture.IsValid);

			float circleCircumference = 2.0f * Mathf.PI * circleGesture.Radius;
			float distanceCovered = circleCircumference * circleGesture.Progress;

			float averageSpeed = distanceCovered / gesture.Duration;
			averageSpeed *= 2000.0f;
			averageSpeed *= direction;
		
			acrobaticsScript.barrelRollTriggerCounter += Time.deltaTime * averageSpeed;
		}
		*/

		// ball squeeze style
		if(sphereRadiusRollingAverage < 70)
		{
			Debug.Log(sphereRadiusRollingAverage);
			if( Mathf.Abs(horizontalMove) > 10.0f )
			{
				acrobaticsScript.barrelRollTriggerCounter += -Mathf.Sign(horizontalMove) * Time.deltaTime * 5.0f;
			}


		}

	}


}
