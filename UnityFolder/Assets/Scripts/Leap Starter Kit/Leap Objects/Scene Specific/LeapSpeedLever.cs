using UnityEngine;
using System.Collections;

public class LeapSpeedLever : LeapGameObject 
{
    public GameObject hand;
    public WheelForce wheelScript;
    public float minRotation = 35;
    public float maxRotation = 325;
    public float minSpeed = 0f;
    public float maxSpeed = 20f;
    public float rotationSpeed = 2f;


    private GameObject handOnLever;
    private float speedThrottle;
    private float currentSpeed;

    #region Overridden Methods

    void FixedUpdate()
    {
        if (!isHeld)
        {
            //ResetLever();
        }

        UpdateCurrentSpeed();

        wheelScript.leverValue = currentSpeed;
    }

    public override LeapState Activate(HandTypeBase h)
    {
        owner = h;

        handOnLever = (GameObject)GameObject.Instantiate(hand, grabCenterOffset.position, Quaternion.identity);
        isHeld = true;

        return new LeapDrivingLeverState(this);
    }

    public override LeapState Release(HandTypeBase h)
    {
        GameObject.Destroy(handOnLever);
        isHeld = false;
        return base.Release(h);
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        RotateLever();
        handOnLever.transform.position = grabCenterOffset.position;
        handOnLever.transform.position += new Vector3(0, 0.1f, -0.2f);
    }

    #endregion

    #region User Defined Methods

    private void UpdateCurrentSpeed()
    {
        float lerpVal = Mathf.InverseLerp(0, 120, speedThrottle);
        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, lerpVal);
    }

    private void RotateLever()
    {
        float offsetAmt = 0.1f;

        if (owner.unityHand.transform.position.y > handOnLever.transform.position.y + offsetAmt &&
            (transform.eulerAngles.x > maxRotation || transform.eulerAngles.x < 80f))
        {
            speedThrottle += rotationSpeed;
            transform.Rotate(-Vector3.right * rotationSpeed, Space.Self);
        }
        else if (owner.unityHand.transform.position.y < handOnLever.transform.position.y - offsetAmt &&
            (transform.eulerAngles.x < minRotation || transform.eulerAngles.x > 280f))
        {
            speedThrottle -= rotationSpeed;
            transform.Rotate(Vector3.right * rotationSpeed, Space.Self);
        }
    }

    private void ResetLever()
    {
        if (speedThrottle > 0)
        {
            speedThrottle -= 1;
            transform.Rotate(Vector3.right, Space.Self);
        }
        else if (speedThrottle < 0)
        {
            speedThrottle += 1;
            transform.Rotate(-Vector3.right, Space.Self);
        }
    }
    #endregion

}
