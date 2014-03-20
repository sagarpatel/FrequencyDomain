using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerScript : MonoBehaviour 
{
	public GameObject mainCameraModulePrefab;
	public GameObject ovrCameraControllerModulePrefab;

	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;

	public Vector3 velocity = new Vector3();
	public Vector3 oldVelocity = new Vector3();
	public Vector3 oldPosition = new Vector3();
	public float friction = 0.0f;
	public float gravity = 0.0f;
	public float rampUpFactor = 1.0f;
	public float rampUpCounter = 0;

	public float currentBoostFactor;
	public float[] boostFactorArray = new float[3];
	public float[] boostTreasholdArray = new float[2];
	public int boostStage = 0;
	public float energyCounter = 0;
	public float originalFieldOfView = 90;
	public float currentFieldOfView;

	public float orignalBloomIntensityValue = 2.0f;
	public float originalLightsRange = 250.0f;

	float bloomBurstValue;

	public float bloomBurstScale = 1.0f;
	public float meshLightsScale = 1.0f;

	public float bloomBurstMinimumHeight = 10;

	public float jumpHeight;
	public float hangtimeCounter;
	public float[] bloomBurstValueArray = new float[5];

	public float hangTimeScale = 1;
	public float degradationTimeScale = 1.0f;

	public int activeCoroutineCounter = 0;

	public float moveTowardsRatio = 0; 
	bool isFlyingUpward = false;
	Vector3 jumpPosition;
	float jumpApexHeight;
	float jumpVelocity;

	public List<Vector3> positionRecordingList = new List<Vector3>();
	public List<Quaternion> rotationRecordingList = new List<Quaternion>();
	public List<Color> colorRecordingList = new List<Color>();
	public float recordingUpdateInterval = 0.015f;
	float recordingUpdateIntervalCounter = 0;
	bool isRecording = false;
	public float recordingLength = 0;


	public bool isOVR = false;
	bool wasOVR = false;
	public float ovrHorizontalSpeedScale = 0.4f;
	public float ovrVerticalSpeedScale = 0.03f;
	OVRCameraController ovrCameraController;


	public GameObject mainCameraGameObject;
	MeshFieldGeneratorScript meshFieldGeneratorScript;
	List<Camera> mainCameraComponentList = new List<Camera>();
	List<Bloom> bloomScriptList = new List<Bloom>();
	List<Light> meshLightsList = new List<Light>();
	CreatureManagerEmittedParts creatureManagerEmittedParts;

	GeneralEditorScript editor;

	public bool isLMCWarping = false;

	// Use this for initialization
	void Start () 
	{
		AttachCorrectCameraModule();
		
		meshFieldGeneratorScript = (MeshFieldGeneratorScript)GameObject.Find("MainMeshField").GetComponent("MeshFieldGeneratorScript");
		
		GameObject[] tempCameraObjectArray =  GameObject.FindGameObjectsWithTag("MainCamera");
		for(int i = 0; i < tempCameraObjectArray.Length; i++)
			mainCameraComponentList.Add( (Camera)tempCameraObjectArray[i].GetComponent("Camera") );

		//mainCameraComponent = (Camera)GameObject.Find("Main Camera").GetComponent("Camera");
		
		for(int i = 0; i < tempCameraObjectArray.Length; i++)
			bloomScriptList.Add( (Bloom)tempCameraObjectArray[i].GetComponent("Bloom") );

		GameObject[] meshLightsObjectsArray =  GameObject.FindGameObjectsWithTag("MeshLight");
		
		for(int i =0; i < meshLightsObjectsArray.Length; i++)
		{
			meshLightsList.Add( (Light)(meshLightsObjectsArray[i]).GetComponent("Light") );
		}

		creatureManagerEmittedParts = (CreatureManagerEmittedParts)GameObject.FindWithTag("CreatureManager").GetComponent("CreatureManagerEmittedParts");

		editor = (GeneralEditorScript)GameObject.FindWithTag("Editor").GetComponent("GeneralEditorScript");

		Screen.showCursor = false;
		Screen.lockCursor = true;
	}	
	
	// Update is called once per frame
	void Update () 
	{

		// Check for camera type change



		if( isOVR != wasOVR)
			AttachCorrectCameraModule();

		wasOVR = isOVR;


		oldPosition = transform.position;
		oldVelocity = velocity;

		float xTranslation = Input.GetAxis("Horizontal") * hControlSpeed * Time.deltaTime;
		float yTranslation = Input.GetAxis("Vertical") * vControlSpeed * Time.deltaTime;

		HandleControls( xTranslation, yTranslation );

		Mathf.Clamp(velocity.y, -100.0f, 20.0f); // hacked clamp

		if(editor.isActive == false)
		{

			//Get New Height
			newHeight = meshFieldGeneratorScript.getHeightFromPosition(transform.position.x - 0, transform.position.z);
			newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -1, transform.position.z) )/4.0f;
			newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -2, transform.position.z) )/8.0f;
			//newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -3, transform.position.z) )/8.0f;

			// add a bit from either side
			newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -1, transform.position.z-1) )/5.0f;
			newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -1, transform.position.z+1) )/5.0f;
			
			if( oldPosition.y < newHeight) // ramping up
			{
				rampUpCounter += (newHeight - oldPosition.y) * Time.deltaTime ; // keep track of of much height is gained
				velocity.y = 0;
				oldPosition.y = newHeight; // hug mesh
				isRecording  = false;
			}
			else if( oldPosition.y > newHeight) // flying in the air
			{
				if( rampUpCounter > 0 ) // the first moment in the air, should not have consecutive enters
				{
					jumpHeight = transform.position.y;
					velocity.y += rampUpCounter * rampUpFactor; // apply velocity gained from ramp
					rampUpCounter = 0; // reset it
					moveTowardsRatio = 0;
					jumpPosition = transform.position;
					jumpApexHeight = 0;
					jumpVelocity = velocity.y;
					isFlyingUpward = true;
					if( jumpVelocity > creatureManagerEmittedParts.playerMinimumJumpVelocity)
					{
						isRecording = true;
						recordingLength = 0;
						StartCoroutine(HandlePlayerMovementRotationRecording()); // start logging position
						
						//Debug.Log( "Recording length:" + positionRecordingList.Count.ToString() );
						//Debug.Log("Framecount: " + Time.frameCount.ToString() );

						creatureManagerEmittedParts.AttemptSpwanCreature(jumpPosition, jumpVelocity); // create creature, does not assemble instantly
					}
				}
				else // in free fall
				{
					velocity.y -= gravity * Time.deltaTime; // apply gravity 	
					hangtimeCounter += Time.deltaTime;
					if( velocity.y < 0 )
					{
						if(isFlyingUpward == true) 
						{
							isFlyingUpward = false;
							jumpApexHeight = transform.position.y;
							jumpVelocity = 0;
						}
						else
							moveTowardsRatio = (jumpApexHeight - oldPosition.y )/jumpApexHeight; // 0 means at the top, 1 means touching ground
					}
				}
			}

		}

		// if oldPosition.y and newHeight are equal, oldPosition stays untouched.
		transform.position = oldPosition + velocity * Time.deltaTime;

		HandleBoost();
		HandleBloomBurst();
		HandleMeshLights();
		
	}

	public void AttachCorrectCameraModule()
	{
		GameObject cameraHolder = GameObject.FindWithTag("CameraHolder");
		// Cleanup up previous camera (if it exists)
		if( cameraHolder.transform.childCount > 0)
			Destroy(cameraHolder.transform.GetChild(0).gameObject, 0.0f);

		
		// Add correct camera module
		if(isOVR)
		{
			GameObject ovrModule = (GameObject)Instantiate(ovrCameraControllerModulePrefab, new Vector3(), Quaternion.identity);
			//ovrModule.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
			ovrModule.transform.parent = cameraHolder.transform;
			ovrModule.transform.localPosition = new Vector3();
			ovrCameraController = (OVRCameraController)GameObject.FindWithTag("OVRCameraController").GetComponent("OVRCameraController");
			mainCameraGameObject =  GameObject.FindWithTag("MainCamera");
			
			GameObject[] tempCameraObjectArray =  GameObject.FindGameObjectsWithTag("MainCamera");
			mainCameraComponentList.Clear();
			for(int i = 0; i < tempCameraObjectArray.Length; i++)
				mainCameraComponentList.Add( (Camera)tempCameraObjectArray[i].GetComponent("Camera") );

		}
		else
		{
			GameObject normalCameraModule = (GameObject)Instantiate(mainCameraModulePrefab, new Vector3(), Quaternion.identity);
			//normalCameraModule.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
			normalCameraModule.transform.parent = cameraHolder.transform;
			normalCameraModule.transform.localPosition = new Vector3();
			mainCameraGameObject =  GameObject.FindWithTag("MainCamera"); //GameObject.Find("Main Camera");
		}

	}

	void HandleBoost()
	{
		// hack fix by pasting object find before every use
		// shouldn't need to do this since new one gets assigned in AttachCorrectCameraModule, isn't the function atomic?
		GameObject[] tempCameraObjectArray =  GameObject.FindGameObjectsWithTag("MainCamera");
		mainCameraComponentList.Clear();
		for(int i = 0; i < tempCameraObjectArray.Length; i++)
			mainCameraComponentList.Add( (Camera)tempCameraObjectArray[i].GetComponent("Camera") );

		foreach(Camera mainCameraComponent in mainCameraComponentList)
		{
			currentFieldOfView = mainCameraComponent.fieldOfView;

			if( currentFieldOfView < boostTreasholdArray[0] )
				boostStage = 0;
			else if( currentFieldOfView < boostTreasholdArray[1] )
				boostStage = 1;
			else
				boostStage = 2;

			float boostFactor = 0;
			boostFactor = boostFactorArray[boostStage];
			currentBoostFactor = boostFactor;

			if(isOVR)
			{
				boostFactor = boostFactor/3.0f;
			}

			if( Input.GetButton("Warp") || isLMCWarping == true )
			{
				energyCounter += Time.deltaTime * boostFactor;
			}
			else
			{
				if( boostStage == 0 )
					energyCounter -= Time.deltaTime * 1.5f * boostFactor;
				else if( boostStage == 1)
					energyCounter -= Time.deltaTime * boostFactor;
				else if(boostStage == 2)
					energyCounter -= Time.deltaTime * 2.0f * boostFactor;


				if(energyCounter < 0)
					energyCounter = 0;
			}

			float rgbValue =  energyCounter/(181 - originalFieldOfView) ;

			//Debug.Log(energyCounter);

			if( originalFieldOfView + energyCounter < 180 )
			{
				mainCameraComponent.fieldOfView = originalFieldOfView + energyCounter;
				mainCameraComponent.backgroundColor = new Color(rgbValue,rgbValue,rgbValue,rgbValue);

				if(isOVR)
				{
					ovrCameraController.SetVerticalFOV(1.5f*mainCameraComponent.fieldOfView);
					ovrCameraController.BackgroundColor = 1.5f * mainCameraComponent.backgroundColor;
				}
			}
		}
	}

	void HandleBloomBurst()
	{
		if(oldVelocity.y < 0 && velocity.y == 0 && jumpHeight > bloomBurstMinimumHeight) // moment of impact with ground
		{	
			Debug.Log("Player HIT GROUND");
			//bloomBurstValue = -oldVelocity.y * bloomBurstScale;
			bloomBurstValue = hangtimeCounter * bloomBurstScale;
			activeCoroutineCounter++;
			StartCoroutine(BloomBurstDegradeCoroutine(hangtimeCounter, bloomBurstValue));
			hangtimeCounter = 0;
		}
		float bloomBurstSum = 0;
		for(int i = 0; i < bloomBurstValueArray.Length; i++)
			bloomBurstSum += bloomBurstValueArray[i];
		
		foreach(Bloom bloomScript in bloomScriptList)
			bloomScript.bloomIntensity = 15.0f * (orignalBloomIntensityValue + bloomBurstSum) * Time.deltaTime ;

	}

	IEnumerator BloomBurstDegradeCoroutine(float hangTime, float initialBloomBurst)
	{	
		for(int i =0; i < bloomBurstValueArray.Length; i++)
		{
			// find an empty stack slot
			if(bloomBurstValueArray[i] == 0)
			{
				float timeCounter = 0;
				hangTime = hangTime * hangTimeScale;
				bloomBurstValueArray[i] = initialBloomBurst;
				while( timeCounter < hangTime )
				{
					bloomBurstValueArray[i] = Mathf.Lerp(bloomBurstValueArray[i], 0, timeCounter/hangTime);
					timeCounter += degradationTimeScale * Time.deltaTime;
					if(bloomBurstValueArray[i] < 0.00001 ) // kill the co-routine if value is too low to be noticeable (this seems to fix inconsistencies bug)
						break;
					yield return null;
				}
				bloomBurstValueArray[i] = 0;
				activeCoroutineCounter --;
				break;
			}
		}
	}

	void HandleMeshLights()
	{
		float bloomBurstSum = 0;
		for(int i = 0; i < bloomBurstValueArray.Length; i++)
			bloomBurstSum += bloomBurstValueArray[i]/bloomBurstScale; //undo bloombusrt scale


		for(int i = 0; i < meshLightsList.Count; i++ )
			meshLightsList[i].range = originalLightsRange + bloomBurstSum * meshLightsScale;
	}

	public void HandleControls( float xTranslation, float yTranslation )
	{
		// check if input would put player out of bounds
		Vector3 predecitedPosition =  oldPosition + velocity * Time.deltaTime;
		float fdc = meshFieldGeneratorScript.verticesFrequencyDepthCount;
		float tdc = meshFieldGeneratorScript.verticesTimeDepthCount;
		float xscale = meshFieldGeneratorScript.xScale;
		float zscale = meshFieldGeneratorScript.zScale;

		Vector3 frictionScaling = new Vector3(1,1,1);

		if( predecitedPosition.x < 0.5f*tdc*xscale && yTranslation > 0)
		{
			yTranslation = 0;
			frictionScaling.x = 2.0f;
		}

		if( predecitedPosition.x > 0.8*tdc*xscale && yTranslation < 0)
		{
			yTranslation = 0;
			frictionScaling.x = 2.0f;
		}

		if( predecitedPosition.z < -10 && xTranslation < 0)
		{
			xTranslation = 0;
			frictionScaling.z = 2.0f;
		}

		if( predecitedPosition.z > 1.0f*fdc*zscale && xTranslation >0 )
		{
			xTranslation = 0;
			frictionScaling.z = 2.0f;
		}

		// apply new force to velocity
		velocity += new Vector3( -yTranslation, 0 , xTranslation);

		//in Oculus Rift mode, use head tilting to add to velocity
		if(isOVR)
		{
			if( yTranslation == 0 && xTranslation == 0 )
			{
				// hack fix by pasting object find before every use
				// shouldn't need to do this since new one gets assigned in AttachCorrectCameraModule, isn't the function atomic?
				mainCameraGameObject =  GameObject.FindWithTag("MainCamera");
				
				float zRotationAngle = mainCameraGameObject.transform.localEulerAngles.z;
				float zVelocityOVR = 0;
				if( zRotationAngle < 180)
					zVelocityOVR = -zRotationAngle * ovrHorizontalSpeedScale;
				else
					zVelocityOVR = (360 - zRotationAngle) * ovrHorizontalSpeedScale;

				float xRotationAngle = mainCameraGameObject.transform.localEulerAngles.x;
				float xVelocityOVR = 0;
				if( xRotationAngle < 180)
					xVelocityOVR = xRotationAngle * ovrVerticalSpeedScale;
				else
					xVelocityOVR = -(360 - xRotationAngle) * ovrVerticalSpeedScale;

				velocity += new Vector3( -xVelocityOVR, 0, zVelocityOVR);

				// hacked up bounding, blargh
				if(oldPosition.x < 0.42f * tdc * xscale)
					oldPosition.x =  0.42f * tdc * xscale;
				else if(oldPosition.x > 0.9 * tdc * xscale)
					oldPosition.x = 0.5f * tdc * xscale;
				else if(oldPosition.z < 0)
					oldPosition.z = 0;
				else if(oldPosition.z > fdc * zscale)
					oldPosition.z = fdc * zscale;
			}
		}

		// only apply friction to translation, not gravity/velocity
		velocity.x -= velocity.x * friction * frictionScaling.x * Time.deltaTime;
		velocity.z -= velocity.z * friction * frictionScaling.z * Time.deltaTime;

	}

	IEnumerator HandlePlayerMovementRotationRecording()
	{

		while(isRecording == true)
		{
			//Debug.Log("Framecount: " + Time.frameCount.ToString() );

			//Debug.Log("Counter: " + recordingUpdateIntervalCounter.ToString() );
			//Debug.Log("Interval: " + recordingUpdateInterval.ToString() );

			// hack fix by pasting object find before every use
			// shouldn't need to do this since new one gets assigned in AttachCorrectCameraModule, isn't the function atomic?
			mainCameraGameObject =  GameObject.FindWithTag("MainCamera");

			if(recordingUpdateIntervalCounter > recordingUpdateInterval)
			{
				recordingUpdateIntervalCounter -= recordingUpdateInterval;
				positionRecordingList.Add(mainCameraGameObject.transform.position);
				rotationRecordingList.Add(mainCameraGameObject.transform.rotation);
				colorRecordingList.Add(meshFieldGeneratorScript.currentColor);

				//Debug.Log(positionRecordingList.Count);
			}
			recordingUpdateIntervalCounter += Time.deltaTime;
			recordingLength += Time.deltaTime;
			yield return null;
		}

	}



}
