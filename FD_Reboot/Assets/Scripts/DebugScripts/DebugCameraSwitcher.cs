using UnityEngine;
using System.Collections;

public class DebugCameraSwitcher : MonoBehaviour 
{
	bool m_isInit = false;

	GameObject m_riderCameraObject;
	GameObject m_conductorCameraObject;

	Camera m_riderCamera;
	Camera m_conductorCamera;

	// TODO: make read only pubilc accessor
	public Camera r_currentActiveCamera;

	void Init()
	{
		m_riderCameraObject = GameObject.FindGameObjectWithTag("RiderCamera");
		m_conductorCameraObject = GameObject.FindGameObjectWithTag("ConductorCamera");

		m_riderCamera = m_riderCameraObject.GetComponent<Camera>();
		m_conductorCamera = m_conductorCameraObject.GetComponent<Camera>();

		m_conductorCameraObject.SetActive(false);
		m_isInit = true;
		r_currentActiveCamera = m_riderCamera;
	}


	void Update () 
	{
		if(m_isInit == false)
		{
			Init();
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.C))
				ToggleCameraActive();
		}
	
	}

	void ToggleCameraActive()
	{
		m_riderCameraObject.SetActive(!m_riderCameraObject.activeSelf);
		m_conductorCameraObject.SetActive(!m_conductorCameraObject.activeSelf);

		if(m_riderCameraObject.activeSelf == true)
			r_currentActiveCamera = m_riderCamera;
		else
			r_currentActiveCamera = m_conductorCamera;
	}
}
