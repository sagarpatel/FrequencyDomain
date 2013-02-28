using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hControlSpeed = 1.0f;
	public float vControlSpeed = 1.0f;

	public float newHeight = 0;
	public float oldHeight = 0;


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
	
		Vector3 tempVec = transform.position;
		tempVec.y = newHeight;
		transform.position = tempVec;
	}


}
