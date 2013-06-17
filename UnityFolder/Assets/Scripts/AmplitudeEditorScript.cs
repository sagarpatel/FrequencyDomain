using UnityEngine;
using System.Collections;

public class AmplitudeEditorScript : MonoBehaviour 
{

	int minIndex = 0;
	int maxIndex = 9;
	public int currentIndex;

	float inputCooldown = 0.2f;
	float cooldownCounter = 0;

	public GameObject rangeMarker;
	Vector3 rangeMarkerPosition = new Vector3();


	public bool isActive = false;

	// Use this for initialization
	void Start () 
	{
		currentIndex = minIndex;
		rangeMarker = (GameObject)Instantiate(rangeMarker, new Vector3(), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () 
	{


		if(isActive)
		{
			HandleInputs();

			foreach (Transform child in rangeMarker.transform)
				child.gameObject.renderer.enabled = true;
			
			rangeMarkerPosition = new Vector3(0,0, 40 * currentIndex );
			rangeMarker.transform.position = rangeMarkerPosition;
		}
		else
		{
			foreach (Transform child in rangeMarker.transform)
				child.gameObject.renderer.enabled = false;
		}



	
	}


	void HandleInputs()
	{


		if( Input.GetAxis("Editor Horizontal") != 0)
		{
			if( cooldownCounter > inputCooldown )
			{
				// actually apply input
				if( Input.GetAxis("Editor Horizontal") > 0)
					currentIndex += 1;
				else if ( Input.GetAxis("Editor Horizontal") < 0)
					currentIndex -= 1;

				cooldownCounter = 0;

			}
			else
				cooldownCounter += Time.deltaTime;

		}
		else
			cooldownCounter += Time.deltaTime;

		if( currentIndex < minIndex )
			currentIndex = minIndex;
		else if( currentIndex > maxIndex )
			currentIndex = maxIndex;			


	}


}
