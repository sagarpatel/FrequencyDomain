using UnityEngine;
using System.Collections;

public class DebugCameraSwitcher : MonoBehaviour 
{
	bool m_isInit = false;

	GameObject m_riderCameraObject;
	GameObject m_conductorCameraObject;
	

	void Init()
	{
		m_riderCameraObject = GameObject.FindGameObjectWithTag("RiderCamera");
		m_conductorCameraObject = GameObject.FindGameObjectWithTag("ConductorCamera");

		m_conductorCameraObject.SetActive(false);
		m_isInit = true;
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
	}
}
