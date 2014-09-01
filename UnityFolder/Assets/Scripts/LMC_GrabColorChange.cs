using UnityEngine;
using System.Collections;
using Leap;

public class LMC_GrabColorChange : MonoBehaviour 
{
	Controller lmcController;

	public float viewGrab;

	void Start () 
	{

		lmcController = new Controller();
		if (lmcController == null)
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");
	
	}
	
	void Update () 
	{
		if (lmcController == null)
			return;

		Frame frame = lmcController.Frame();
		HandList hands = frame.Hands;
		Hand firstHand = hands[0];

		float grabStrength = Mathf.Pow(firstHand.GrabStrength, 2.0f);
		viewGrab = grabStrength;

		camera.backgroundColor = Color.Lerp(Color.black, Color.white, grabStrength);

	}
}
