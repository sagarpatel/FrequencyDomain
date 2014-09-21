using UnityEngine;
using System.Collections;

public class WheelForce : MonoBehaviour {

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    public Transform wheelFLTrans;
    public Transform wheelFRTrans;
    public Transform wheelRLTrans;
    public Transform wheelRRTrans;

    public float maxTorque = 5;

    public float currentSpeed;
    public float topSpeed = 150;
    public float maxReverseSpeed = 50;
    public float lowestSteerAtSpeed = 50;
    public float lowSpeedSteerAngel = 10;
    public float highSpeedSteerAngel = 1;
    public float decellarationSpeed = 30;
    public float wheelRotVal { get; set; }
    public float leverValue { get; set; }
    private bool braked = false;
    private float myForwardFriction;
    private float mySidewayFriction;
    private float slipForwardFriction;
    private float slipSidewayFriction;

	// Use this for initialization
	void Start () {
        rigidbody.centerOfMass += new Vector3(0,-3f,0);

        SetValues();
	}

    private void SetValues()
    {
        myForwardFriction = wheelRR.forwardFriction.stiffness;

        mySidewayFriction = wheelRR.sidewaysFriction.stiffness;

        slipForwardFriction = 0.05f;

        slipSidewayFriction = 0.085f;
    }

    void Update()
    {
        wheelFLTrans.Rotate(wheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        wheelFRTrans.Rotate(wheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        wheelRLTrans.Rotate(wheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        wheelRRTrans.Rotate(wheelRR.rpm / 60 * 360 * Time.deltaTime, 0, 0);

        Vector3 wheelAngleFL = wheelFLTrans.localEulerAngles;
        Vector3 wheelAngleFR = wheelFRTrans.localEulerAngles;

        wheelFLTrans.localEulerAngles = new Vector3(wheelAngleFL.x, wheelFL.steerAngle - wheelAngleFL.z, wheelAngleFL.z);
        wheelFRTrans.localEulerAngles = new Vector3(wheelAngleFR.x, wheelFR.steerAngle - wheelAngleFR.z, wheelAngleFR.z);
        
        ReverseSlip();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        Control();

        HandBrake();
	}

    private void ReverseSlip()
    {

        if (currentSpeed < 0)
        {

            SetFrontSlip(slipForwardFriction, slipSidewayFriction);

        }

        else
        {

            SetFrontSlip(myForwardFriction, mySidewayFriction);

        }

    }

    private void Control()
    {
        float maxVal = Mathf.Lerp(0, topSpeed, leverValue);

        currentSpeed = 2 * 22 / 7 * wheelRL.radius * wheelRL.rpm * 60 / 1000;

        currentSpeed = Mathf.Round(currentSpeed);

        //if (currentSpeed < maxVal && currentSpeed > -maxReverseSpeed && !braked)
        if (currentSpeed < maxVal &&  !braked)
        {

            wheelRR.motorTorque = maxTorque * leverValue;
            wheelRL.motorTorque = maxTorque * leverValue;
            wheelRR.brakeTorque = 0;
            wheelRL.brakeTorque = 0;

        }
        else if((currentSpeed - maxVal) > 5)
        {
            wheelRR.motorTorque = 0;
            wheelRL.motorTorque = 0;
            wheelRR.brakeTorque = decellarationSpeed;
            wheelRL.brakeTorque = decellarationSpeed;

        }
        else
        {
            wheelRR.motorTorque = 0;
            wheelRL.motorTorque = 0;
        }

        var speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;

        var currentSteerAngel = Mathf.Lerp(lowSpeedSteerAngel, highSpeedSteerAngel, speedFactor);

        currentSteerAngel *= wheelRotVal;

        wheelFL.steerAngle = currentSteerAngel;

        wheelFR.steerAngle = currentSteerAngel;

    }

    void HandBrake()
    {
        wheelFR.brakeTorque = 0;
        wheelFL.brakeTorque = 0;

        SetRearSlip(myForwardFriction, mySidewayFriction); 
    }

    void SetRearSlip(float currentForwardFriction, float currentSidewayFriction)
    {
        WheelFrictionCurve ff;
        WheelFrictionCurve sf;

        ff = wheelRR.forwardFriction;
        sf = wheelRR.sidewaysFriction;

        ff.stiffness = currentForwardFriction;
        sf.stiffness = currentSidewayFriction;

        ff = wheelRL.forwardFriction;
        sf = wheelRL.sidewaysFriction;

        ff.stiffness = currentForwardFriction;
        sf.stiffness = currentSidewayFriction;
    }

    void SetFrontSlip(float currentForwardFriction, float currentSidewayFriction)
    {
        WheelFrictionCurve ff;
        WheelFrictionCurve sf;

        ff = wheelFR.forwardFriction;
        sf = wheelFR.sidewaysFriction;

        ff.stiffness = currentForwardFriction;
        sf.stiffness = currentSidewayFriction;

        ff = wheelFL.forwardFriction;
        sf = wheelFL.sidewaysFriction;

        ff.stiffness = currentForwardFriction;
        sf.stiffness = currentSidewayFriction;
    }
}
