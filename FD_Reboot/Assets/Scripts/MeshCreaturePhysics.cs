using UnityEngine;
using System.Collections;

public class MeshCreaturePhysics : MonoBehaviour 
{
	float m_speed = 200.0f;
	Vector3 m_rotVel;

	float m_speedMin = 100.0f;
	float m_speedMax = 1000.0f;
	float m_rotVelRange_x = 20.0f;
	float m_rotVelRange_y = 20.0f;
	float m_rotVelRange_z = 20.0f;

	float m_rotVelDecay_x = 0.0f;
	float m_rotVelDecay_y = 0.0f;
	float m_rotVelDecay_z = 0.0f;


	void Update()
	{
		m_rotVel.x -= m_rotVelDecay_x * m_rotVel.x * Time.deltaTime;
		m_rotVel.y -= m_rotVelDecay_y * m_rotVel.y * Time.deltaTime;
		m_rotVel.z -= m_rotVelDecay_z * m_rotVel.z * Time.deltaTime;


		transform.Rotate(m_rotVel * Time.deltaTime);
		transform.position += transform.forward * m_speed * Time.deltaTime;
	}

	public void IncrementCreatureRotationalVel(float increment_x, float increment_y, float increment_z)
	{
		m_rotVel.x = Mathf.Clamp( m_rotVel.x + increment_x, - m_rotVelRange_x, m_rotVelRange_x);
		m_rotVel.y =  Mathf.Clamp( m_rotVel.y + increment_y, -m_rotVelRange_y, m_rotVelRange_y);
		m_rotVel.z = Mathf.Clamp(m_rotVel.z + increment_z, -m_rotVelRange_z, m_rotVelRange_z);
	}


}
