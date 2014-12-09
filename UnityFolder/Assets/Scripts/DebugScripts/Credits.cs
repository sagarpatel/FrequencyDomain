using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{

	public GameObject creditsModelPrefab;

	public Vector3 scale;

	GameObject creditsModel;

	bool visibleState = false;

	AudioDirectorScript audioDirector;

	// Use this for initialization
	void Start () 
	{
		creditsModel = (GameObject)Instantiate(creditsModelPrefab, transform.position, transform.rotation);
		creditsModel.transform.localScale = scale;
		creditsModel.GetComponent<MeshRenderer>().enabled = false;


		audioDirector =  (AudioDirectorScript)GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if( (Input.GetKeyDown("b")) || (Input.GetKeyDown("v")) || (Input.GetKeyDown("n") ) )
		{
			visibleState = !visibleState;
			creditsModel.GetComponent<MeshRenderer>().enabled = visibleState;
		}

		if(visibleState)
		{
			creditsModel.renderer.material.color = 10.0f * audioDirector.calculatedRGB;
		}

	}



}
