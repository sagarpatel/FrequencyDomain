using UnityEngine;
using System.Collections;

public class DebugSkyboxController : MonoBehaviour 
{
	Skybox m_riderCameraSkybox;
	Camera m_riderCamera;

	void Start()
	{
		m_riderCameraSkybox = FindObjectOfType<Skybox>();
		m_riderCamera = FindObjectOfType<Camera>();
		SetToColor(Color.black);
	}

	void Update()
	{

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetToColor(Color.black);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetToColor(Color.white);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetToSkybox();
		}

	}

	void SetToColor(Color bkgColor)
	{
		m_riderCameraSkybox.enabled = false;
		m_riderCamera.backgroundColor = bkgColor;
	}

	void SetToSkybox()
	{
		m_riderCameraSkybox.enabled = true;
	}


}
