using UnityEngine;
using System.Collections;

public class DebugSkyboxController : MonoBehaviour 
{
	Skybox m_riderCameraSkybox;
	Skybox m_conductorCameraSkybox;
	Camera m_riderCamera;
	Camera m_conductorCamera;

	public Material[] m_skyboxMaterialsArray;
	public Material m_renderTextureSkybox;

	float m_prevInput = 0;
	int m_currentIndex = 0;

	void Start()
	{
		m_riderCameraSkybox = GameObject.FindGameObjectWithTag("RiderCamera").GetComponent<Skybox>();
		m_riderCamera = GameObject.FindGameObjectWithTag("RiderCamera").GetComponent<Camera>();
		m_conductorCameraSkybox = GameObject.FindGameObjectWithTag("ConductorCamera").GetComponent<Skybox>();
		m_conductorCamera = GameObject.FindGameObjectWithTag("ConductorCamera").GetComponent<Camera>();
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

		m_conductorCameraSkybox.enabled = false;
		m_conductorCamera.backgroundColor = bkgColor;
	}

	void SetToSkybox_Normal(int skyboxIndex)
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_skyboxMaterialsArray[skyboxIndex];

		m_conductorCameraSkybox.enabled = true;
		m_conductorCameraSkybox.material = m_skyboxMaterialsArray[skyboxIndex];
	}

	void SetToSkybox_RenderTexture()
	{
		m_riderCameraSkybox.enabled = true;
		m_riderCameraSkybox.material = m_renderTextureSkybox;

		m_conductorCameraSkybox.enabled = true;
		m_conductorCameraSkybox.material = m_renderTextureSkybox;
	}

	public void SkyboxChange(int input)
	{
		if(m_prevInput != input)
		{

			m_currentIndex = (m_currentIndex + input) % 10;


			if( m_currentIndex == 0)
			{
				SetToColor(Color.black);
			}
			else if ( m_currentIndex == 1)
			{
				SetToColor(Color.white);
			}
			else if( m_currentIndex == 2)
			{
				SetToSkybox_Normal(0);
			}
			else if( m_currentIndex == 3)
			{
				SetToSkybox_Normal(1);
			}
			else if( m_currentIndex == 4)
			{		
				SetToSkybox_Normal(2);
			}
			else if( m_currentIndex == 5)
			{
				SetToSkybox_Normal(3);
			}
			else if( m_currentIndex == 6)
			{
				SetToSkybox_Normal(4);
			}
			else if( m_currentIndex == 7)
			{		
				SetToSkybox_Normal(5);
			}
			else if( m_currentIndex == 8)
			{
				SetToSkybox_Normal(6);
			}
			else if( m_currentIndex == 9)
			{
				SetToSkybox_RenderTexture();
			}
		}

		m_prevInput = input;

	}

}
