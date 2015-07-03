using UnityEngine;
using System.Collections;

public class RiderCamera_BarrelRollPhysics : MonoBehaviour 
{
	float m_barrelRollVel = 0;
	float m_barrelRollVelRange = 2000.0f;
	float m_barrelRollVelDecay = 3.3f;
	float m_currentBarrelRollAngle = 0;
	bool m_barrelRollVelDecayFlag = false;


	void Update()
	{
		m_currentBarrelRollAngle += m_barrelRollVel * Time.deltaTime;
		if(m_barrelRollVelDecayFlag == true)
			m_barrelRollVel -= m_barrelRollVel * m_barrelRollVelDecay * Time.deltaTime;

		Quaternion barrelRollRotation = Quaternion.Euler(new Vector3(0,0, -m_currentBarrelRollAngle));
		transform.localRotation = barrelRollRotation;
	}

	public void IncrementBarrelRollVelocity(float barrelRollInput)
	{
		m_barrelRollVel = Mathf.Clamp( m_barrelRollVel + barrelRollInput, -m_barrelRollVelRange, m_barrelRollVelRange );
		if(barrelRollInput == 0)
			m_barrelRollVelDecayFlag = true;
		else
			m_barrelRollVelDecayFlag = false;
	}

}
