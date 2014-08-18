using UnityEngine;
using System.Collections;
using Leap;

public class LMC_FingertipsStitch : MonoBehaviour 
{
	Controller lmcController;
	AudioDirectorScript audioDirector;
	public Vector3[] fingertipsPosArray;
	float posScale = 3.0f;

	GameObject[] debugPosObjects;

	void Start () 
	{
		audioDirector = GameObject.FindGameObjectWithTag("AudioDirector").GetComponent<AudioDirectorScript>();
		lmcController = new Controller();
		if (lmcController == null)
			Debug.LogWarning("Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");

		fingertipsPosArray = new Vector3[5];
		debugPosObjects = new GameObject[fingertipsPosArray.Length];
		for (int i = 0; i < debugPosObjects.Length; i++)
		{
			debugPosObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			debugPosObjects[i].transform.parent = transform;
		}
	}
	
	void Update () 
	{

		if (lmcController != null)
		{
			Frame frame = lmcController.Frame();
			HandList hands = frame.Hands;
			Hand firstHand = hands[0];

			for (int i = 0; i < fingertipsPosArray.Length; i++)
			{
				fingertipsPosArray[i] =  posScale * audioDirector.overallAmplitudeScaler * firstHand.Fingers[i].StabilizedTipPosition.ToUnityScaled();
				debugPosObjects[i].transform.localPosition = fingertipsPosArray[i];
			}
			
		}
	
	}

}
