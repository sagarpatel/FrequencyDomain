using UnityEngine;
using System.Collections;

public class DebugSkyboxController : MonoBehaviour 
{
	Skybox m_riderCameraSkybox;
	Camera m_riderCamera;

	public Material m_normalSkybox;
	public Material m_renderTextureSkybox;

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
			SetToSkybox_Normal();
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetToSkybox_RenderTexture();
		}
	}

	void SetToColor(Color bkgColor)
	{
		m_riderCameraSkybox.enabled = false;
		m_riderCamera.backgroundColor = bkgColor;
	}

	void SetToSkybox_Normal()
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_normalSkybox;
	}

	void SetToSkybox_RenderTexture()
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_renderTextureSkybox;
	}

}
