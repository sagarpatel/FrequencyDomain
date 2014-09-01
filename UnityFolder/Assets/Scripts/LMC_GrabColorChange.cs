using UnityEngine;
using System.Collections;
using Leap;

public class LMC_GrabColorChange : MonoBehaviour 
{
	Controller lmcController;

	public float viewGrab;

	float[] grabBuffer;
	int currentIndex = 0;

	public float currentAverage = 0;

	void Start () 
	{

		lmcController = new Controller();
		if (lmcController == null)
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");

		grabBuffer = new float[30];
	}
	
	// using normal update would means rolling average could move at varaible spped --> bad
	void FixedUpdate () 
	{
		if (lmcController == null)
			return;

		Frame frame = lmcController.Frame();
		HandList hands = frame.Hands;
		Hand firstHand = hands[0];

		float grabStrength = Mathf.Pow(firstHand.PinchStrength, 2.0f);//GrabStrength, 2.0f);
		viewGrab = grabStrength;

		currentIndex++;
		if (currentIndex > grabBuffer.Length - 1)
			currentIndex = 0;
		grabBuffer[currentIndex] = grabStrength;

		float temp = 0;
		for (int i = 0; i < grabBuffer.Length; i++)
		{
			temp += grabBuffer[i];
		}
		currentAverage = temp / (float)grabBuffer.Length;

		camera.backgroundColor = Color.Lerp(Color.black, Color.white, currentAverage);
	}
}
