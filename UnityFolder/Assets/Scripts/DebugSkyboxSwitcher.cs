using UnityEngine;
using System.Collections;

public class DebugSkyboxSwitcher : MonoBehaviour 
{
	public Material skybox1;
	public Material skybox2;
	public Material skybox3;
	public Material skybox4;
	public Material skybox5;
	public Material skybox6;
	public Material skybox7;
	public Material skybox8;
	public Material skybox9;


	Skybox skybox;
	Camera camera;
	AudioDirectorScript audioDirector;

	bool isLiveColor = false;

	void Start()
	{
		skybox = GetComponent<Skybox>();
		camera = GetComponent<Camera>();
		audioDirector = FindObjectOfType<AudioDirectorScript>();
	}


	void Update()
	{

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log("PRESSED");
			skybox.material = skybox1;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			skybox.material = skybox2;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			skybox.material = skybox3;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			skybox.material = skybox4;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			skybox.material = skybox5;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			skybox.material = skybox6;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			skybox.material = skybox7;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			skybox.material = skybox8;
			skybox.enabled = true;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Alpha9))
		{
			skybox.material = skybox9;
			skybox.enabled = true;
			isLiveColor = false;
		}


		if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			skybox.enabled = false;
			camera.backgroundColor = Color.black;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.Minus))
		{
			skybox.enabled = false;
			camera.backgroundColor = Color.white;
			isLiveColor = false;
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			skybox.enabled = false;
			isLiveColor = true;
		}


		if(isLiveColor)
		{
			camera.backgroundColor = audioDirector.calculatedRGB;
		}

	}

}
