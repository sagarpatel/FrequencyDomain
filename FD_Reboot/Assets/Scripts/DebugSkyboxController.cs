using UnityEngine;
using System.Collections;

public class DebugSkyboxController : MonoBehaviour 
{
	Skybox m_riderCameraSkybox;
	Camera m_riderCamera;

	public Material[] m_skyboxMaterialsArray;
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
			SetToSkybox_Normal(0);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetToSkybox_Normal(1);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			SetToSkybox_Normal(2);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			SetToSkybox_Normal(3);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			SetToSkybox_Normal(4);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			SetToSkybox_Normal(5);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha9))
		{
			SetToSkybox_Normal(6);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			SetToSkybox_RenderTexture();
		}
	}

	void SetToColor(Color bkgColor)
	{
		m_riderCameraSkybox.enabled = false;
		m_riderCamera.backgroundColor = bkgColor;
	}

	void SetToSkybox_Normal(int skyboxIndex)
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_skyboxMaterialsArray[skyboxIndex];
	}

	void SetToSkybox_RenderTexture()
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_renderTextureSkybox;
	}

}
