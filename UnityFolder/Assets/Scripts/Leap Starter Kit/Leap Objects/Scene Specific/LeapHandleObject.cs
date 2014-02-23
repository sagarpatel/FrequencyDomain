using UnityEngine;
using System.Collections;
using Leap;

public class LeapHandleObject : LeapGameObject
{
    public GameObject hand;
    public HandleScript rotationObject;
    public LeapGameObject brakes;
    public bool canAccelerate = true;

    [HideInInspector]
    public Vector3 releasePosition;
    [HideInInspector]
    public float releaseDistance = 0.5f;

    private GameObject handOnHandle;
    private float multiplyWristRotation = 1.2f;
    private bool pressedBrakes = false;

    // Positions translated from World to Local Space relative to LeapPosOffset
    private Vector3 unityHandPOS;
    private Vector3 heldObjectPOS;
    private Vector3 handOnHandlePOS;
    private Vector3 rotationObjectPOS;

    #region Overridden Methods

    public override LeapState Activate(HandTypeBase h)
    {
        owner = h;
        isHeld = true;

        return new LeapHandleState(this);
    }

    public override LeapState Release(HandTypeBase h)
    {
        isHeld = false;

        if (handOnHandle)
        {
            GameObject.Destroy(handOnHandle);
        }

        if (canAccelerate)
        {
            UpdateBrakes();
        }

        return base.Release(h);
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        UpdateLocalPositions();

        GrabbingUpdate();
    }

    #endregion


    #region User Defined Methods

    private void UpdateLocalPositions()
    {
        unityHandPOS = owner.unityHand.settings.leapPosOffset.InverseTransformPoint(owner.transform.position);
        heldObjectPOS = owner.unityHand.settings.leapPosOffset.InverseTransformPoint(transform.position);

        if (handOnHandle)
            handOnHandlePOS = owner.unityHand.settings.leapPosOffset.InverseTransformPoint(handOnHandle.transform.position);

        rotationObjectPOS = owner.unityHand.settings.leapPosOffset.InverseTransformPoint(rotationObject.transform.position);
    }

    /// <summary>
    /// If Grabbing, check if brakes are pressed and run either brakes code or speed code
    /// </summary>
    private void GrabbingUpdate()
    {
        // Spawn closed hand model if it doesn't exist
        if (!handOnHandle)
        {
            handOnHandle = (GameObject)GameObject.Instantiate(hand, transform.position, rotationObject.transform.rotation);

            handOnHandle.transform.parent = transform;

            if (owner.unityHand.isRightHand)
            {
                handOnHandle.transform.localEulerAngles += new Vector3(0, 25, 0);
                handOnHandle.transform.localPosition += new Vector3(0, 0.02f, 0);
            }
            else
            {
                handOnHandle.transform.localEulerAngles += new Vector3(0, 40, 0);
                handOnHandle.transform.localPosition += new Vector3(0, -0.01f, 0);
            }

            CheckIfBrakesPressed();
            TurnOffBrake();

            HighlightObject(false);
            brakes.HighlightObject(false);

            owner.HideHand();
        }

        // Rotate Steering System
        RotateBasedOnHandPosition();

        if (pressedBrakes)
        {
            if (canAccelerate)
            {
                UpdateBrakes();
                brakes.transform.localRotation = Quaternion.Euler(new Vector3(0, 30, 0));
            }
        }
        else
        {
            if (canAccelerate)
            {
                brakes.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                UpdateSpeed();
            }
        }
    }

    private void UpdateBrakes()
    {
        rotationObject.speedThrottle = 0;
    }

    private void UpdateSpeed()
    {
        float rotAngle = CalculateHandRotation();
        float mVal = multiplyWristRotation;

        //Debug.Log(rotAngle);

        //rotationObject.speedThrottle = rotationObject.maxClampSpeed - rotAngle;
        rotationObject.speedThrottle = rotAngle;

        Vector3 tempAngle = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(new Vector3(rotAngle * mVal, tempAngle.y, tempAngle.z));
        
        //Debug.Log(rotationObject.speedThrottle);
    }
    
    /// <summary>
    /// Calculates Forward X-axis rotation of hand
    /// </summary>
    /// <returns></returns>
    private float CalculateHandRotation()
    {
        // Find Angle
        Vector3 temp = owner.unityHand.hand.Direction.ToUnity();
        temp = new Vector3(0, temp.y, temp.z);
        float rotAngle = Vector3.Angle(temp, Vector3.forward);

        // Find Sign
        Vector3 referenceRight = Vector3.Cross(Vector3.forward, temp);
        float sign = (Vector3.Dot(referenceRight, Vector3.right) > 0.0f) ? -1.0f : 1.0f;


        rotAngle = rotAngle * sign;

        return rotAngle;
    }

    private void CheckIfBrakesPressed()
    {
        BrakeDetection brake = brakes.GetComponent<BrakeDetection>();

        if (brake)
        {
            if (brake.brakeCollision) //isBrakeColliding
            {
                pressedBrakes = true;
            }
            else
            {
                pressedBrakes = false;
            }
        }
    }

    private void TurnOffBrake()
    {
        BrakeDetection brake = brakes.GetComponent<BrakeDetection>();

        if (brake)
        {
            brake.brakeCollision = false;
        }
    }

    void RotateBasedOnHandPosition()
    {
        //float rotAngle = CalculateSteeringRotationAngle(handOnHandle.transform.position, unityHand.transform.position);
        float rotAngle = CalculateSteeringRotationAngle(handOnHandlePOS, unityHandPOS);

        Debug.DrawLine(Vector3.zero, unityHandPOS, Color.red);
        Debug.DrawLine(Vector3.zero, heldObjectPOS, Color.red);
        Debug.DrawLine(Vector3.zero, handOnHandlePOS, Color.red);
        Debug.DrawLine(Vector3.zero, rotationObjectPOS, Color.red);

        //if (isLeft(rotationObject.transform.position, unityHand.transform.position, handOnHandle.transform.position))
        if (isLeft(rotationObjectPOS, unityHandPOS, handOnHandlePOS))
        {
            rotationObject.rotValue += rotAngle;
            rotationObject.transform.Rotate(Vector3.up, rotAngle);
        }
        else
        {
            rotationObject.rotValue -= rotAngle;
            rotationObject.transform.Rotate(Vector3.up, -rotAngle);
        }

        ClampRotations();
    }

    private float CalculateSteeringRotationAngle(Vector3 p1, Vector3 p2)
    {
        Vector3 targetDir = p1 - rotationObjectPOS;
        Vector3 targetDir2 = p2 - rotationObjectPOS;

        targetDir = new Vector3(targetDir.x, 0, targetDir.z);
        targetDir2 = new Vector3(targetDir2.x, 0, targetDir2.z);

        return (Vector3.Angle(targetDir, targetDir2));
    }

    void ClampRotations()
    {
        float angleDiff;

        // Reset Rotation to Max Values
        if (rotationObject.rotValue > rotationObject.maxClampRot)
        {
            angleDiff = rotationObject.rotValue - rotationObject.maxClampRot;
            rotationObject.rotValue = rotationObject.maxClampRot;
            rotationObject.transform.Rotate(Vector3.up, -angleDiff);
        }
        else if (rotationObject.rotValue < rotationObject.minClampRot)
        {
            angleDiff = rotationObject.rotValue + rotationObject.maxClampRot;
            rotationObject.rotValue = rotationObject.minClampRot;
            rotationObject.transform.Rotate(Vector3.up, -angleDiff);
        }
    }

    bool isLeft(Vector3 a, Vector3 b, Vector3 c)
    {
        return ((b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x)) > 0;
    }

    public void OpenHandUpdate()
    {
        // Only happens once when hand was previously grabbing
        if (handOnHandle)
        {
            releasePosition = owner.unityHand.transform.localPosition;

            owner.ShowHand();

            GameObject.Destroy(handOnHandle);
        }

        owner.transform.position = transform.position;
        owner.transform.position += rotationObject.transform.up * 0.1f;


        CheckIfBrakesPressed();

        if (pressedBrakes)
        {
            HighlightObject(true);
            brakes.HighlightObject(true);
        }
        else
        {
            HighlightObject(true);
            brakes.HighlightObject(false);
        }
    }

    #endregion
}
