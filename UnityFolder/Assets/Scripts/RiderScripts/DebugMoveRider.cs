using UnityEngine;
using System.Collections;

public class DebugMoveRider : MonoBehaviour 
{
	float forwardSpeedScale = 100.0f;
	float reverseSpeedScale = 400.0f;

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

		Vector3 after = transform.position;

		
		if(before.magnitude != after.magnitude)
		{
			Debug.Log("DIFFERENT");
			Debug.Log(before);
			Debug.Log(after);
		}
	}

}
