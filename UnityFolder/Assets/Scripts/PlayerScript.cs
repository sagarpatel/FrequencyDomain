using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;

	public Vector3 velocity = new Vector3();
	public float friction = 0.0f;
	public float gravity = 0.0f;
	public float rampUpFactor = 1.0f;
	public float rampUpCounter = 0;

	MeshFieldGeneratorScript meshFieldGeneratorScript;

	// Use this for initialization
	void Start () 
	{
		meshFieldGeneratorScript = (MeshFieldGeneratorScript)GameObject.Find("MainMeshField").GetComponent("MeshFieldGeneratorScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 oldPosition = transform.position;

		float xTranslation = Input.GetAxis("Horizontal") * hControlSpeed;
		float yTranslation = Input.GetAxis("Vertical") * vControlSpeed;
		/*
		// cancel velocity in axis if changing direction (left/right only)
		if( xTranslation * velocity.z < 0) // if directions are oppsite
			velocity.z = 0;
	*/
		// apply new force to velocity
		velocity += new Vector3( -yTranslation, 0 , xTranslation);



		// only apply friction to translation, not gravity/height
		velocity.x -= velocity.x * friction;
		velocity.z -= velocity.z * friction;

		
		//Get New Height
		newHeight = meshFieldGeneratorScript.getHeightFromPosition(transform.position.x -1, transform.position.z);
		
		if( oldPosition.y < newHeight) // ramping up
		{
			rampUpCounter += (newHeight - oldPosition.y) * Time.deltaTime ; // keep track of of much height is gained
			velocity.y = 0;
			oldPosition.y = newHeight; // hug mesh
		}
		else if( oldPosition.y > newHeight) // flying in the air
		{
			if( rampUpCounter > 0 ) // the first moment in the air
			{
				velocity.y += rampUpCounter * rampUpFactor; // apply velocity gained from ramp
				//Debug.Log(velocity.y);
				rampUpCounter = 0; // reset it
			}
			else // in free fall
			{
				velocity.y -= gravity * Time.deltaTime; // apply gravity 
				Debug.Log(gravity * Time.deltaTime);
			}
		}

		// if oldPosition.y and newHeight are equal, oldPosition stays untouched.

		transform.position = oldPosition + velocity * Time.deltaTime;
	}


}
