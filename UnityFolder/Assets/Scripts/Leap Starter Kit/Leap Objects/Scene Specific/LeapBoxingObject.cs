using UnityEngine;
using System.Collections;

public class LeapBoxingObject : LeapBasicObject {

    [HideInInspector]
    public Vector3 maxVelocity;

    private Vector3 prevPos;
    //private Transform prevTransform;
    private Vector3 highestVel;

    private float maxVelTime = 0.2f;
    private float velTime;

    #region Overridden Methods

    protected override void Start()
    {
        //prevTransform = transform;
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        base.UpdateTransform(t);

        UpdatePunchingVelocity();
    }

    private void UpdatePunchingVelocity()
    {
        //prevTransform = transform;
        maxVelocity = highestVel;

        Vector3 dir = owner.transform.position - prevPos;
        dir = dir.normalized;
        dir = dir * (Vector3.Distance(owner.transform.position, prevPos) * owner.unityHand.settings.throwingStrength);

        // Keep track of highestVelocity, reset if maxVel exists for too long
        VelocityCheck(dir);

        prevPos = owner.transform.position;
        velTime += Time.deltaTime;
    }

    #endregion

    #region User Defined Methods

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

    #endregion
}
