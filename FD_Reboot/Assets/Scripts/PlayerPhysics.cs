using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour 
{

	void Update()
	{
		RaycastHit hitInfo =  new RaycastHit();
		if(Physics.Raycast(transform.position, -transform.up, out hitInfo))
		{
			Debug.Log("RAYCAST HIT! " + hitInfo.transform.name + " : " + hitInfo.distance);

		}

	}



}
