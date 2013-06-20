using UnityEngine;
using System.Collections;

public class AcrobaticsScript : MonoBehaviour 
{

	public float barrelRollSensitivity = 1.0f;
	public float barrelRollDegradation = 0.95f;
	public float barrelRollTriggerCounter;

	public Vector3 eulerRot;

	GeneralEditorScript editor;
	PlayerScript playerScript;

	// Use this for initialization
	void Start () 
	{
		editor = (GeneralEditorScript)GameObject.Find("Editor_Importer").GetComponent("GeneralEditorScript");
		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(editor.isActive == false)
		{
			if(playerScript.isOVR == false)
			{

				// Handle barrel roll input
				// windows controls for triggers
				if( Input.GetAxis("LeftTrigger") > 0 )
				{
					if( barrelRollTriggerCounter > 0)
						barrelRollTriggerCounter += Input.GetAxis("LeftTrigger") * Time.deltaTime;
					else
						barrelRollTriggerCounter += Input.GetAxis("LeftTrigger") * Time.deltaTime * 3.0f ; //reverse faster
				}
				if( Input.GetAxis("RightTrigger") < 0 )
				{
					if( barrelRollTriggerCounter < 0)
						barrelRollTriggerCounter += Input.GetAxis("RightTrigger") * Time.deltaTime;
					else
						barrelRollTriggerCounter += Input.GetAxis("RightTrigger") * Time.deltaTime * 3.0f; //reverse faster
				}

				//handle keyboard input
				if( Input.GetKey("q") )
				{
					if( barrelRollTriggerCounter > 0)
						barrelRollTriggerCounter += 1.5f * Time.deltaTime;
					else
						barrelRollTriggerCounter +=  Time.deltaTime * 3.0f ; //reverse faster
				}
				if( Input.GetKey("e") )
				{
					if( barrelRollTriggerCounter < 0)
						barrelRollTriggerCounter -= 1.5f * Time.deltaTime;
					else
						barrelRollTriggerCounter -=  Time.deltaTime * 3.0f; //reverse faster
				}

			}

		}

		if(playerScript.isOVR == false)
		{
			
			if( Mathf.Abs(Input.GetAxis("LeftTrigger")) < 0.1f )
				barrelRollTriggerCounter =  barrelRollTriggerCounter * barrelRollDegradation;

			transform.Rotate( -barrelRollTriggerCounter * barrelRollSensitivity, 0, 0);

			eulerRot = transform.eulerAngles;
		}
	
	}


}
