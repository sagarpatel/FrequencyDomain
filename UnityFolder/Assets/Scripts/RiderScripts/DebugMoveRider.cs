using UnityEngine;
using System.Collections;

public class DebugMoveRider : MonoBehaviour 
{
	float forwardSpeedScale = 500.0f;
	float reverseSpeedScale = 400.0f;

	float sideSpeedScale = 100.0f;

	public Transform meshHeadObject;
	RiderPosRot riderPosRot;

	void Start()
	{
		riderPosRot = GetComponent<RiderPosRot>();
	}

	void Update()
	{
		Vector3 before = transform.position;

		if(Input.GetKey(KeyCode.Y) == true)
		{
			transform.position += transform.forward * forwardSpeedScale * Time.deltaTime;
			//Debug.Log(transform.position);
		}
		if(Input.GetKey(KeyCode.H) == true)
		{
			transform.position += -transform.forward * reverseSpeedScale * Time.deltaTime;
		}

		if(Input.GetKey(KeyCode.J) == true)
		{
			riderPosRot.widthOffset += sideSpeedScale * Time.deltaTime;
		}

		if(Input.GetKey(KeyCode.G) == true)
		{
			riderPosRot.widthOffset -= sideSpeedScale * Time.deltaTime;
		}

		Vector3 after = transform.position;

		
		if(before.magnitude != after.magnitude)
		{
			//Debug.Log("DIFFERENT");
			//Debug.Log(before);
			//Debug.Log(after);
		}

		//Debug.Log("diff: " + Vector3.Distance(transform.position, meshHeadObject.position));


	}

}
