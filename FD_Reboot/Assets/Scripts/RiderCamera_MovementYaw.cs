using UnityEngine;
using System.Collections;

public class RiderCamera_MovementYaw : MonoBehaviour 
{
	RiderPhysics m_riderPhysics;
	public AnimationCurve m_yawProgressCurve;
	float m_yawRange = 25.0f;

	void Start()
	{
		m_riderPhysics = GetComponentInParent<RiderPhysics>();
	}

	void Update()
	{
		float currentWidthVelRatio = m_riderPhysics.CalculateVelocityRatio_Width();

		float yawStep = m_yawProgressCurve.Evaluate( Mathf.Abs(currentWidthVelRatio) );
		float yawAngle = Mathf.Sign(currentWidthVelRatio) * Mathf.Lerp(0, m_yawRange, yawStep);
		Quaternion yawRotation = Quaternion.Euler( new Vector3(0, yawAngle, 0));
		transform.localRotation = yawRotation;
	}
}
