using UnityEngine;
using System.Collections;

public class PunchingBag : MonoBehaviour
{

	public void OnCollisionEnter(Collision collision)
	{
        LeapBoxingObject leapObj = collision.gameObject.GetComponent<LeapBoxingObject>();

		if (leapObj)
		{
			Debug.Log(leapObj.maxVelocity.magnitude);
			rigidbody.AddForceAtPosition(leapObj.maxVelocity * 800, leapObj.transform.position);
		}
	}
}
