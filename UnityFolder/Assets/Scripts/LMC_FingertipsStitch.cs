using UnityEngine;
using System.Collections;
using Leap;

public class LMC_FingertipsStitch : MonoBehaviour 
{
	Controller lmcController;
	AudioDirectorScript audioDirector;
	MeshLinesGenerator meshlinesGenerator;

	Vector3[] fingertipsPosArray;
	float posScale = 7.0f;
	GameObject[] debugPosObjects;
	Vector3[] stitchPosArray;
	public bool isValidData = false;

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
			debugPosObjects[i].name = "Debug finger: " + i.ToString();
			debugPosObjects[i].transform.parent = transform;
			debugPosObjects[i].renderer.enabled = false;
		}

		meshlinesGenerator = GetComponent<MeshLinesGenerator>();
		stitchPosArray = new Vector3[meshlinesGenerator.verticesFrequencyDepthCount];
	}

	void Update()
	{
		isValidData = false;
		if (lmcController == null)
			return;

		Frame frame = lmcController.Frame();
		HandList hands = frame.Hands;
		Hand firstHand = hands[0];

		if (firstHand.IsValid == false)
		{
			print("Invalid hand");
			return;
		}


		for (int i = 0; i < fingertipsPosArray.Length; i++)
		{
			Vector3 leapFingerPos = firstHand.Fingers[i].StabilizedTipPosition.ToUnityScaled();
			// flipping x and z to account for parent transform facing the wrong way
			leapFingerPos.x = -leapFingerPos.x * 2.0f;
			leapFingerPos.z = -leapFingerPos.z * 4.0f ;
			fingertipsPosArray[i] = posScale * audioDirector.overallAmplitudeScaler * leapFingerPos;
			debugPosObjects[i].transform.localPosition = fingertipsPosArray[i];
		}

		// reverse order of data if right hand
		if (firstHand.IsLeft == false)
		{
			print("RIGHT HAND DAWG");
			System.Array.Reverse(debugPosObjects);
		}

		int fingerIndex = 0;
		int fingerCount = debugPosObjects.Length;
		int stitchesPerFinger = stitchPosArray.Length / fingerCount;

		for (int i = 0; i < stitchPosArray.Length; i++)
		{
			stitchPosArray[i] = debugPosObjects[fingerIndex].transform.position;
			if ((i + 1) % stitchesPerFinger == 0)
				fingerIndex++;
		}
		// send data over
		meshlinesGenerator.stitchOriginPosArray = stitchPosArray;
		isValidData = true;
	}
		
	
	

}
