using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;
	public float oldHeight = 0;

	public Vector3 velocity = new Vector3();
	public float friction = 0.0f;
	public float gravity = 0.0f;


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
		//transform.Translate( -yTranslation, 0 , xTranslation);
		velocity += new Vector3( -yTranslation, 0 , xTranslation);
		// only apply friction to translation, not gravity/height
		velocity.x -= velocity.x * friction;
		velocity.z -= velocity.z * friction;

		
		//Get New Height
		oldHeight = newHeight;
		newHeight = meshFieldGeneratorScript.getHeightFromPosition(transform.position.x, transform.position.z);

		// if ramping up
		if( newHeight > oldHeight)
		{
			velocity.y += newHeight - oldHeight;
		}

		// if up in the air
		if( oldPosition.y > newHeight)
		{
			velocity.y -= gravity ;
		}
		else 
		{
			// if not in the air, hug surface
			velocity.y = 0;
			oldPosition.y = newHeight;
		}

		transform.position = oldPosition + velocity * Time.deltaTime;
	}


}
