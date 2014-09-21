using UnityEngine;
using System.Collections;

public class LeapSteeringWheel : LeapGameObject
{
    public GameObject hand;
    public WheelForce wheelScript;
    public float wheelRadius = 1.2f;
    public float minClampRot = -180;
    public float maxClampRot = 180;

    private float steeringWheelResetSpeed = 4;
    private Quaternion defaultRotation;
    private float rotValue;
    private float currentRot;
    private float minRot = -1;
    private float maxRot = 1;

    #region Overridden Methods

    protected override void Start()
    {
        defaultRotation = transform.rotation;
        base.Start();
    }

    void FixedUpdate()
    {
        if (!isHeld)
        {
            ResetWheel();
        }

        UpdateRotValue();

        //Debug.Log(currentRot);
        wheelScript.wheelRotVal = currentRot;
    }

    public override LeapState Activate(HandTypeBase h)
    {
        owner = h;

        isHeld = true;

        return new LeapDrivingState(this);
    }

    public override LeapState Release(HandTypeBase h)
    {
        isHeld = false;
        return base.Release(h);
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        if (!owner)
        {
            t.SetActiveObject(this);
            owner = t;
        }
        else
        {
            RotateSteeringWheel();
            isHeld = true;
        }
    }

    #endregion

    #region User Defined Methods

    public GameObject PlaceHandOnSteeringWheel(GameObject handOnSteeringWheel)
    {
        Vector3 handPosition = CalculateHandOnSteeringWheelPosition();

        float rotationAmt = CalculateSteeringRotationAngle(Vector3.up * 100, owner.transform.position);

        if (isLeft(transform.position, Vector3.up, owner.transform.position))
        {
            rotationAmt = -rotationAmt;
        }

        handOnSteeringWheel = (GameObject)GameObject.Instantiate(hand, handPosition, Quaternion.Euler(new Vector3(0, 0, rotationAmt)));
        handOnSteeringWheel.transform.parent = transform;

        return handOnSteeringWheel;
    }

    private Vector3 CalculateHandOnSteeringWheelPosition()
    {
        Vector3 handGrabPosition = new Vector3();
        Vector3 handDirection = owner.transform.position - transform.position;

        handGrabPosition = transform.position;
        handDirection.Normalize();
        handDirection *= wheelRadius;

        handGrabPosition += handDirection;
        return (handGrabPosition);
    }

    private float CalculateSteeringRotationAngle(Vector3 p1, Vector3 p2)
    {
        Vector3 targetDir = p1 - transform.position;
        Vector3 targetDir2 = p2 - transform.position;

        targetDir = new Vector3(targetDir.x, targetDir.y, 0);
        targetDir2 = new Vector3(targetDir2.x, targetDir2.y, 0);

        return (Vector3.Angle(targetDir, targetDir2));
    }

    bool isLeft(Vector3 a, Vector3 b, Vector3 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }

    void RotateSteeringWheel()
    {
        GameObject handOnSteeringWheel = ((LeapDrivingState)owner.stateController.CurrentState).handOnSteeringWheel;

        float rotAngle = CalculateSteeringRotationAngle(handOnSteeringWheel.transform.position, owner.unityHand.transform.position);


        if (isLeft(transform.position, owner.unityHand.transform.position, handOnSteeringWheel.transform.position))
        {
            rotValue += rotAngle;
            transform.Rotate(Vector3.up, rotAngle);
        }
        else
        {
            rotValue -= rotAngle;
            transform.Rotate(Vector3.up, -rotAngle);
        }

        ClampRotations();
    }

    void ClampRotations()
    {
        float angleDiff;

        // Reset Rotation to Max Values
        if (rotValue > maxClampRot)
        {
            angleDiff = rotValue - maxClampRot;
            rotValue = maxClampRot;
            transform.Rotate(Vector3.up, -angleDiff);
        }
        else if (rotValue < minClampRot)
        {
            angleDiff = rotValue + maxClampRot;
            rotValue = minClampRot;
            transform.Rotate(Vector3.up, -angleDiff);
        }
    }

    private void ResetWheel()
    {
        float speed = steeringWheelResetSpeed;
        float range = steeringWheelResetSpeed / 2;

        if (rotValue > range)
        {
            rotValue -= speed;
        }
        else if (rotValue < -range)
        {
            rotValue += speed;
        }
        else if (rotValue < range && rotValue > -range)
        {
            rotValue = 0;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, defaultRotation, Time.deltaTime * speed);
    }

    // Convert from degrees to min/max values
    private void UpdateRotValue()
    {
        float lerpVal = Mathf.InverseLerp(minClampRot, maxClampRot, rotValue);
        currentRot = Mathf.Lerp(minRot, maxRot, lerpVal);
    }

    #endregion

}