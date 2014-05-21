using UnityEngine;
using System.Collections;

public class DebugPointLight : MonoBehaviour 
{
	Light light;

	// Use this for initialization
	void Start () 
	{
		light = gameObject.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetKey("o"))
		{
			light.range -= 100.0f * Time.deltaTime;
		}
		if(Input.GetKey("p"))
		{
			light.range += 100.0f * Time.deltaTime;
		}
		if(Input.GetKey("k"))
		{
			light.intensity -= 5.0f * Time.deltaTime;
		}
		if(Input.GetKey("l"))
		{
			light.intensity += 5.0f * Time.deltaTime;
		}
	
	}
	/*
	void OnGUI()
	{

		GUI.Label(new Rect(0.0f, 0.05f*Screen.height, Screen.width, 0.2f*Screen.height), "Light Range: " + light.range.ToString());
		GUI.Label(new Rect(0.0f, 0.1f*Screen.height, Screen.width, 0.2f*Screen.height), "Light Intensity: " + light.intensity.ToString());
	}
	*/
}
