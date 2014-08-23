using UnityEngine;
using System.Collections;
using Leap;

public class LMC_FingertipsStitch : MonoBehaviour 
{
	Controller lmcController;
	AudioDirectorScript audioDirector;
	MeshLinesGenerator meshlinesGenerator;

	Vector3[] fingertipsPosArray;
	float posScale = 0.01f;
	GameObject[] debugPosObjects;

	public bool isValidData = false;
	int jointsPerFinger = 5; // finger tip is the 5th joint (index 0)
	public Vector3[][] fingerJointsArrayStitchesPosArray;

	Vector3[][] fingersArrayJointsPositionsPosArray;

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
		//stitchPosArray = new Vector3[meshlinesGenerator.verticesFrequencyDepthCount];
		fingerJointsArrayStitchesPosArray = new Vector3[jointsPerFinger][];
		for (int i = 0; i < fingerJointsArrayStitchesPosArray.Length; i++)
		{
			fingerJointsArrayStitchesPosArray[i] = new Vector3[meshlinesGenerator.verticesFrequencyDepthCount];
		}

		// internal array to store just joint positions
		fingersArrayJointsPositionsPosArray = new Vector3[5][]; // 5 because there are 5 fingers
		for (int i = 0; i < fingersArrayJointsPositionsPosArray.Length; i++)
		{
			fingersArrayJointsPositionsPosArray[i] = new Vector3[jointsPerFinger]; 
		}
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

		
		//for every finger
		for (int i = 0; i < fingersArrayJointsPositionsPosArray.Length; i++)
		{
			// for every joint in finger
			// do bone joints (except fingertips)
			for (int j = 1; j < jointsPerFinger; j++)
			{
				// enum mappings -->  https://developer.leapmotion.com/documentation/skeletal/csharp/api/Leap.Bone.html#csharpclass_leap_1_1_bone_1ad1607a6b2f5cceb9194ad7d7f88d4b07

				Vector3 jointPos = firstHand.Fingers[i].Bone((Bone.BoneType)(jointsPerFinger -1 - j)).PrevJoint.ToUnity();
				// flipping x and z to account for parent transform facing the wrong way
				jointPos.x *= -2.0f;
				jointPos.z *= -4.0f;
				jointPos *= posScale * audioDirector.overallAmplitudeScaler;
				debugPosObjects[i].transform.localPosition = jointPos;
				fingersArrayJointsPositionsPosArray[i][j] = debugPosObjects[i].transform.position; ;
			}
			
		}
		// do finger tips (get stabilized value0
		for (int i = 0; i < fingertipsPosArray.Length; i++)
		{
			Vector3 leapFingerPos = firstHand.Fingers[i].StabilizedTipPosition.ToUnity();
			// flipping x and z to account for parent transform facing the wrong way
			leapFingerPos.x = -leapFingerPos.x * 2.0f;
			leapFingerPos.z = -leapFingerPos.z * 4.0f ;
			fingertipsPosArray[i] = posScale * audioDirector.overallAmplitudeScaler * leapFingerPos;
			debugPosObjects[i].transform.localPosition = fingertipsPosArray[i];
			fingersArrayJointsPositionsPosArray[i][0] = debugPosObjects[i].transform.position;
		}


		// reverse order of data if right hand
		if (firstHand.IsLeft == false)
		{
			print("RIGHT HAND DAWG");
			//System.Array.Reverse(debugPosObjects);
			System.Array.Reverse(fingersArrayJointsPositionsPosArray);
		}

		int fingerIndex = 0;
		int fingerCount = debugPosObjects.Length;
		int stitchesCount = fingerJointsArrayStitchesPosArray[0].Length;
		int stitchesPerFinger = stitchesCount / fingerCount; //stitchPosArray.Length / fingerCount;

		// going through each collumn
		for (int i = 0; i < stitchesCount; i++)
		{
			//stitchPosArray[i] = debugPosObjects[fingerIndex].transform.position;

			for (int j = 0; j < jointsPerFinger; j++)
			{
				fingerJointsArrayStitchesPosArray[j][i] = fingersArrayJointsPositionsPosArray[fingerIndex][j];
			}


			if ((i + 1) % stitchesPerFinger == 0)
				fingerIndex++;
		}
		// send data over
		meshlinesGenerator.fingerJointsArrayStitchesPosArray = fingerJointsArrayStitchesPosArray;
		isValidData = true;
	}
		
	
	

}
