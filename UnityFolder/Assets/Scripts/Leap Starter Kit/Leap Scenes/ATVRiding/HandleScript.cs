using UnityEngine;
using System.Collections;

public class HandleScript : MonoBehaviour {

    public WheelForce wheelScript;

    [HideInInspector]
    public float currentRot;
    [HideInInspector]
    public float rotValue;
    [HideInInspector]
    public float currentSpeed;
    [HideInInspector]
    public float speedThrottle;

    public float minClampRot = -45;
    public float maxClampRot = 45;
    private float minRot = -1;
    private float maxRot = 1;
    public float minSpeed = 0f;
    public float maxSpeed = 1f;
    public float minClampSpeed = 0;
    public float maxClampSpeed = 20;

	// Update is called once per frame
	void FixedUpdate () {

        UpdateRotValue();
        UpdateCurrentSpeed();

        wheelScript.wheelRotVal = currentRot;
        wheelScript.leverValue = currentSpeed;
        //Debug.Log("CURRENT SPEED: " + currentSpeed);
	}

    // Convert from degrees to min/max values
    private void UpdateRotValue()
    {
        float lerpVal = Mathf.InverseLerp(minClampRot, maxClampRot, rotValue);
        currentRot = Mathf.Lerp(minRot, maxRot, lerpVal);
    }

    private void UpdateCurrentSpeed()
    {
        float lerpVal = Mathf.InverseLerp(minClampSpeed, maxClampSpeed, speedThrottle);
        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, lerpVal);
    }

}
