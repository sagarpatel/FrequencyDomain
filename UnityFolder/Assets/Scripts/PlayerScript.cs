using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hSpeed = 1.0f;
	public float vSpeed = 1.0f;

	public float currentHeight = 0;

	MeshFieldGeneratorScript meshFieldGenerator;

	// Use this for initialization
	void Start () 
	{
		meshFieldGenerator = (MeshFieldGeneratorScript)GameObject.Find("MainMeshField").GetComponent("MeshFieldGeneratorScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float xTranslation = Input.GetAxis("Horizontal");
		float yTranslation = Input.GetAxis("Vertical");
		transform.Translate( -yTranslation, 0 , xTranslation);

		
		//Get Height
		currentHeight = meshFieldGenerator.getHeightFromPosition(transform.position.x, transform.position.z);
		//Set Height
		Vector3 tempVec = transform.position;
		tempVec.y = currentHeight + 0.5f;
		transform.position = tempVec;
	
	}


}
