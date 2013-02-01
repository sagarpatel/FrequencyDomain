using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public float hSpeed = 1.0f;
	public float vSpeed = 1.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float xTranslation = Input.GetAxis("Horizontal");
		float yTranslation = Input.GetAxis("Vertical");

		transform.Translate( -yTranslation, 0 , xTranslation);
	
	}


}
