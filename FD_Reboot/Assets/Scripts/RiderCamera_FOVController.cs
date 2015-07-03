using UnityEngine;
using System.Collections;

public class RiderCamera_FOVController : MonoBehaviour 
{
	Camera m_riderCamera;
	float m_currentRiderFOV = 0;
	float m_riderCameraFOV_Min = 100.0f;
	float m_riderCameraFOV_Max = 179.0f;
	public AnimationCurve m_riderCameraFOVCurve;
	float m_currentStep_Linear = 0;
	float m_currentStep_Curve = 0;
	float m_inputTimeCounter = 0;
	float m_timeToMaxFOV = 3.0f;
	float m_timeCounterDecayScale = 4.0f;
	bool m_fovDecayFlag = false;

	void Start()
	{
		m_riderCamera = GetComponent<Camera>();
		m_currentRiderFOV = m_riderCameraFOV_Min;
		m_riderCamera.fieldOfView = m_currentRiderFOV;
	}

	void Update()
	{
		if(m_fovDecayFlag == true)
		{
			m_inputTimeCounter = Mathf.Clamp( m_inputTimeCounter - m_timeCounterDecayScale * Time.deltaTime, 0, m_riderCameraFOV_Max);
		}

		m_currentStep_Linear = Mathf.InverseLerp(0, m_timeToMaxFOV, m_inputTimeCounter);
		m_currentStep_Curve = m_riderCameraFOVCurve.Evaluate(m_currentStep_Linear);

		m_currentRiderFOV = Mathf.Lerp(m_riderCameraFOV_Min, m_riderCameraFOV_Max, m_currentStep_Curve);
		m_riderCamera.fieldOfView = m_currentRiderFOV;
	}

	// assuming digital input (on/off)
	public void RiderCameraFOVInput(float controlInput)
	{
		if(controlInput > 0)
		{
			m_inputTimeCounter = Mathf.Clamp( m_inputTimeCounter + Time.deltaTime, 0, m_timeToMaxFOV);
			m_fovDecayFlag = false;
		}
		else
		{
			m_fovDecayFlag = true;
		}
	}
	
}
