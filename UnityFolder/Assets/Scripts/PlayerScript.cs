using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerScript : MonoBehaviour 
{
	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;

	public Vector3 velocity = new Vector3();
	public Vector3 oldVelocity = new Vector3();
	Vector3 oldPosition = new Vector3();
	public float friction = 0.0f;
	public float gravity = 0.0f;
	public float rampUpFactor = 1.0f;
	public float rampUpCounter = 0;

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
	public float ovrHorizontalSpeedScale = 0.4f;
	public float ovrVerticalSpeedScale = 0.05f;
	OVRCameraController ovrCameraController;


	GameObject mainCameraGameObject;
	MeshFieldGeneratorScript meshFieldGeneratorScript;
	List<Camera> mainCameraComponentList = new List<Camera>();
	List<Bloom> bloomScriptList = new List<Bloom>();
	List<Light> meshLightsList = new List<Light>();
	CreatureManagerScript creatureManagerScript;

	// Use this for initialization
	void Start () 
	{
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

		creatureManagerScript = (CreatureManagerScript)GameObject.Find("CreatureManager").GetComponent("CreatureManagerScript");
		mainCameraGameObject =  GameObject.FindWithTag("MainCamera"); //GameObject.Find("Main Camera");

		if(isOVR)
			ovrCameraController = (OVRCameraController)GameObject.Find("OVRCameraController").GetComponent("OVRCameraController");
	}	
	
	// Update is called once per frame
	void Update () 
	{
		oldPosition = transform.position;
		oldVelocity = velocity;

		HandleControls();

		

		//Get New Height
		newHeight = meshFieldGeneratorScript.getHeightFromPosition(transform.position.x - 0, transform.position.z);
		newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -1, transform.position.z) )/4.0f;
		newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -2, transform.position.z) )/8.0f;
		//newHeight += ( meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -3, transform.position.z) )/8.0f;
		
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
				if( jumpVelocity > creatureManagerScript.playerMinimumJumpVelocity)
				{
					isRecording = true;
					recordingLength = 0;
					StartCoroutine(HandlePlayerMovementRotationRecording()); // start logging position
					
					creatureManagerScript.AttemptSpwanCreature(jumpPosition, jumpVelocity); // create creature, does not assemble instantly
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

		// if oldPosition.y and newHeight are equal, oldPosition stays untouched.
		transform.position = oldPosition + velocity * Time.deltaTime;

		HandleBoost();
		HandleBloomBurst();
		HandleMeshLights();
		
	}

	void HandleBoost()
	{

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

			if( Input.GetButton("Fire1") || Input.GetButton("Jump") )
			{
				energyCounter += Time.deltaTime * boostFactor;
			}
			else
			{
				energyCounter -= Time.deltaTime * boostFactor;
				if(energyCounter < 0)
					energyCounter = 0;
			}

			float rgbValue =  energyCounter/(181 - originalFieldOfView) ;

			if( originalFieldOfView + energyCounter < 180 )
			{
				mainCameraComponent.fieldOfView = originalFieldOfView + energyCounter;
				mainCameraComponent.backgroundColor = new Color(rgbValue,rgbValue,rgbValue,rgbValue);

				if(isOVR)
				{
					ovrCameraController.SetVerticalFOV(mainCameraComponent.fieldOfView);

				}


			}

		}

	}

	void HandleBloomBurst()
	{
		if(oldVelocity.y < 0 && velocity.y == 0 && jumpHeight > bloomBurstMinimumHeight) // moment of impact with ground
		{				
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
			bloomScript.bloomIntensity = orignalBloomIntensityValue + bloomBurstSum;

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

	void HandleControls()
	{
		// check if input would put player out of bounds
		Vector3 predecitedPosition =  oldPosition + velocity * Time.deltaTime;
		float fdc = meshFieldGeneratorScript.verticesFrequencyDepthCount;
		float tdc = meshFieldGeneratorScript.verticesTimeDepthCount;
		float xscale = meshFieldGeneratorScript.xScale;
		float zscale = meshFieldGeneratorScript.zScale;


		float xTranslation = Input.GetAxis("Horizontal") * hControlSpeed * Time.deltaTime;
		float yTranslation = Input.GetAxis("Vertical") * vControlSpeed * Time.deltaTime;

		Vector3 frictionScaling = new Vector3(1,1,1);

		if( predecitedPosition.x < 0.5f*tdc*xscale && yTranslation > 0)
		{
			yTranslation = 0;
			frictionScaling.x = 2.0f;
		}

		if( predecitedPosition.x > tdc*xscale && yTranslation < 0)
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

			Debug.Log(new Vector3( -xVelocityOVR, 0, zVelocityOVR));

		}

		// only apply friction to translation, not gravity/velocity
		velocity.x -= velocity.x * friction * frictionScaling.x * Time.deltaTime;
		velocity.z -= velocity.z * friction * frictionScaling.z * Time.deltaTime;

	}

	IEnumerator HandlePlayerMovementRotationRecording()
	{

		while(isRecording == true)
		{
			if(recordingUpdateIntervalCounter > recordingUpdateInterval)
			{
				recordingUpdateIntervalCounter -= recordingUpdateInterval;
				positionRecordingList.Add(mainCameraGameObject.transform.position);
				rotationRecordingList.Add(mainCameraGameObject.transform.rotation);
				colorRecordingList.Add(meshFieldGeneratorScript.currentColor);
			}
			recordingUpdateIntervalCounter += Time.deltaTime;
			recordingLength += Time.deltaTime;
			yield return null;
		}

	}



}
