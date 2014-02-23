using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Basic Leap Game Object
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class LeapThrowableObject : LeapBasicObject 
{
    private Vector3 prevPos;
    private Vector3 highestVel;

    // Used to determine the highest velocity within the last 0.2 seconds
    private float velTime;
    private float maxVelTime = 0.2f;
	
	protected override void Start()
	{
        base.Start();
		rigidbody.isKinematic = false;
	}

	public override LeapState Release(HandTypeBase h)
	{
        LeapState state = null;

		if (!isStatePersistent)
		{
            if (canGoThroughGeometry)
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
            }
            ThrowHeldObject();
            state = base.Release(h);
		}
		return state;
	}

    public override void UpdateTransform(HandTypeBase t)
    {
        base.UpdateTransform(t);

        Vector3 dir = transform.position - prevPos;
        dir = dir.normalized;
        dir = dir * (Vector3.Distance(transform.position, prevPos) * owner.unityHand.settings.throwingStrength);

        VelocityCheck(dir);

		if (!rigidbody.isKinematic)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;

			// Necessary to switch Hand Updates for collisions
			if (collisionOccurred)
			{
				owner.unityHand.runUpdate = false;
			}
			else
			{
				owner.unityHand.runUpdate = true;
			}
		}

        prevPos = transform.position;
        velTime += Time.deltaTime;
    }


    /// <summary>
    /// Keep track of highestVelocity, reset if maxVel exists for too long
    /// </summary>
    /// <param name="dir"></param>
    private void VelocityCheck(Vector3 dir)
    {
        float maxVelocitySpeed = owner.unityHand.settings.maxThrowingVelocity;
        if (dir.magnitude > highestVel.magnitude)
        {
            if (dir.magnitude > maxVelocitySpeed)
            {
                // Vector limited to max velocity
                dir.Normalize();
                dir = dir * maxVelocitySpeed;
            }

            highestVel = dir;
            velTime = 0;
        }
        else if (velTime > maxVelTime)
        {
            highestVel = dir;
            velTime = 0;
        }
    }

    private void ThrowHeldObject()
    {
        Vector3 previousAngle = LeapInputEx.Controller.Frame(10).Hand(owner.unityHand.hand.Id).PalmNormal.ToUnity();
        Vector3 currentAngle = owner.unityHand.hand.PalmNormal.ToUnity();
        float angle = Vector3.Angle(previousAngle, currentAngle);
        Vector3 angularVelocity = Vector3.Cross(previousAngle, currentAngle).normalized * angle * owner.unityHand.settings.angularStrength;
        
        // Throwing Velocity
        rigidbody.velocity = highestVel;

        //Angular Velocity
        if (rigidbody.maxAngularVelocity > owner.unityHand.settings.maxAngularThrowingVelocity)
        {
            rigidbody.maxAngularVelocity = owner.unityHand.settings.maxAngularThrowingVelocity;
        }
        rigidbody.angularVelocity = angularVelocity;

        Debug.Log("Object Thrown at Vel: " + rigidbody.velocity.magnitude);
        Debug.Log("Object Thrown at Angular Vel: " + rigidbody.angularVelocity);
    }
}
