using UnityEngine;
using System.Collections;

public class AcrobaticsScript : MonoBehaviour 
{

	public float barrelRollSensitivity = 1.0f;
	public float barrelRollDegradation = 0.95f;
	public float barrelRollTriggerCounter;

	public Vector3 eulerRot;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

		// Handle barrel roll input
		// windows controls for triggers
		if( Input.GetAxis("LeftTrigger") > 0 )
			barrelRollTriggerCounter += Input.GetAxis("LeftTrigger");
		if( Input.GetAxis("RightTrigger") < 0 )
			barrelRollTriggerCounter += Input.GetAxis("RightTrigger");
		if( Mathf.Abs(Input.GetAxis("LeftTrigger")) < 0.1f )
			barrelRollTriggerCounter =  barrelRollTriggerCounter * barrelRollDegradation;



		

		eulerRot = transform.eulerAngles;
	
	}


}
