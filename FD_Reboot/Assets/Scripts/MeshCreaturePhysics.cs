using UnityEngine;
using System.Collections;

public class MeshCreaturePhysics : MonoBehaviour 
{
	float m_baseSpeed = 200.0f;
	float m_currentSpeed = 0;

	Vector3 m_rotVel;

	float m_rotVelRange_x = 20.0f;
	float m_rotVelRange_y = 20.0f;
	float m_rotVelRange_z = 20.0f;

	float m_rotVelDecay_x = 1.0f;
	float m_rotVelDecay_y = 1.0f;
	float m_rotVelDecay_z = 1.0f;

	bool m_decayFlag_x = false;
	bool m_decayFlag_y = false;
	bool m_decayFlag_z = false;

	float m_extraSpeed = 0;
	float m_extraSpeedProgress = 0;
	float m_extraSpeedMax = 500.0f;

	public AnimationCurve m_extraSpeedCurve;
	float m_extraSpeedRampUpDuration = 5.0f;
	bool m_extraSpeedDecayFlag = false;

	void FixedUpdate()
	{
		if(m_decayFlag_x == true)
			m_rotVel.x -= m_rotVelDecay_x * m_rotVel.x * Time.deltaTime;
		if(m_decayFlag_y == true)
			m_rotVel.y -= m_rotVelDecay_y * m_rotVel.y * Time.deltaTime;
		if(m_decayFlag_z == true)
			m_rotVel.z -= m_rotVelDecay_z * m_rotVel.z * Time.deltaTime;

		transform.Rotate(m_rotVel * Time.deltaTime);

		float step = m_extraSpeedCurve.Evaluate(m_extraSpeedProgress);
		if(m_extraSpeedDecayFlag == true)
			m_extraSpeedProgress = Mathf.Clamp( m_extraSpeedProgress - Time.deltaTime/m_extraSpeedRampUpDuration, 0, 1);

		m_extraSpeed = Mathf.Lerp(0, m_extraSpeedMax, step);
		m_currentSpeed = m_baseSpeed + m_extraSpeed;
		transform.position += transform.forward * m_currentSpeed * Time.deltaTime;
	}

	public void IncrementCreatureRotationalVel(float increment_x, float increment_y, float increment_z)
	{
		m_rotVel.x = Mathf.Clamp( m_rotVel.x + increment_x, - m_rotVelRange_x, m_rotVelRange_x);
		m_rotVel.y =  Mathf.Clamp( m_rotVel.y + increment_y, -m_rotVelRange_y, m_rotVelRange_y);
		m_rotVel.z = Mathf.Clamp(m_rotVel.z + increment_z, -m_rotVelRange_z, m_rotVelRange_z);

		if(increment_x == 0)
			m_decayFlag_x = true;
		else
			m_decayFlag_x = false;

		if(increment_y == 0)
			m_decayFlag_y = true;
		else
			m_decayFlag_y = false;

		if(increment_z == 0)
			m_decayFlag_z = true;
		else
			m_decayFlag_z = false;
	}

	public void IncrementExtraSpeedStep(float stepIncrement)
	{
		m_extraSpeedProgress = Mathf.Clamp(m_extraSpeedProgress + stepIncrement/m_extraSpeedRampUpDuration, 0, 1.0f);

		if(stepIncrement == 0)
			m_extraSpeedDecayFlag = true;
		else
			m_extraSpeedDecayFlag = false;
	}

}
