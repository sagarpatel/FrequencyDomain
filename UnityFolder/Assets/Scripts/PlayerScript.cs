using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;
	public float oldHeight = 0;

	public float heightPosition = 0;
	public float dropRatio = 0.98f;

	MeshFieldGeneratorScript meshFieldGeneratorScript;

	// Use this for initialization
	void Start () 
	{
		meshFieldGeneratorScript = (MeshFieldGeneratorScript)GameObject.Find("MainMeshField").GetComponent("MeshFieldGeneratorScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float xTranslation = Input.GetAxis("Horizontal") * hControlSpeed;
		float yTranslation = Input.GetAxis("Vertical") * vControlSpeed;
		transform.Translate( -yTranslation, 0 , xTranslation);
		
		//Get New Height
		newHeight = meshFieldGeneratorScript.getHeightFromPosition(transform.position.x, transform.position.z);
		/*
		//Calculate height velocity, only if going higher
		if( newHeight > oldHeight )
		{
			heightPosition += newHeight - oldHeight;
		}
		heightPosition = heightPosition * dropRatio;

		// make sure you stay above the mesh
		if( heightPosition < newHeight )
		{
			heightPosition = newHeight;
		}
	
		//Set Height
		Vector3 tempVec = transform.position;
		tempVec.y = heightPosition + 0.5f;
		transform.position = tempVec;




		oldHeight = newHeight;
		*/
		Vector3 tempVec = transform.position;
		tempVec.y = newHeight;
		transform.position = tempVec;
	}


}
