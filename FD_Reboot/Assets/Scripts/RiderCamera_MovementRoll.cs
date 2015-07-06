using UnityEngine;
using System.Collections;

public class RiderCamera_MovementRoll : MonoBehaviour 
{
	RiderPhysics m_riderPhysics;
	public AnimationCurve m_rollProgressCurve;
	float m_rollRange = 25.0f;

	void Start()
	{
		m_riderPhysics = GetComponentInParent<RiderPhysics>();
	}

	void Update()
	{
		float currentWidthVelRatio = m_riderPhysics.CalculateVelocityRatio_Width();

		float rollStep = m_rollProgressCurve.Evaluate( Mathf.Abs(currentWidthVelRatio) );
		float rollAngle = -Mathf.Sign(currentWidthVelRatio) * Mathf.Lerp(0, m_rollRange, rollStep);
		Quaternion rollRotation = Quaternion.Euler(new Vector3(0, 0, rollAngle));
		transform.localRotation = rollRotation;
	}


}
