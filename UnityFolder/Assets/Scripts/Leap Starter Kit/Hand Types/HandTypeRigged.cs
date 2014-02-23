using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class HandTypeRigged : HandTypeBase 
{
    public RiggedHandTransforms riggedHandModel;
    private RiggedHandTransforms riggedHand;

    private Transform[] joints = new Transform[5];
    private Matrix handTransform;


    protected override void Awake()
    {
        base.Awake();
        riggedHand = (RiggedHandTransforms)Instantiate(riggedHandModel, transform.position, Quaternion.identity);
        riggedHand.transform.parent = transform;
        riggedHand.gameObject.SetActive(false);

        joints = riggedHand.joints;

        stateController.Initialize(this, new LeapGodHandState());
    }

    public override void UpdateHandType()
    {
        base.UpdateHandType();
        UpdateState();
    }

    public override void HandLost()
    {
        HideHand();
        base.HandLost();
    }

    
    public override void HideHand()
    {
        canBeVisible = false;
        riggedHand.gameObject.SetActive(false);
    }

    public override void ShowHand()
    {
        canBeVisible = true;
        riggedHand.gameObject.SetActive(true);
    }

    public override void HandFound()
    {
        ShowHand();
        base.HandFound();
    }


    void LateUpdate()
    {
        UpdateJoints();
    }

    /// <summary>
    /// Update joint position based on finger position
    /// </summary>
    void UpdateJoints()
    {
        Vector3 angle;
        float seekSpeed = Time.deltaTime * 20;
        float returnSpeed = Time.deltaTime * 4;
        Vector3 rotation;
        List<int> fingerIDs = new List<int>(unityHand.leapFingers.Keys);

        handTransform = unityHand.detectedFingers.handTransform;

        // Update Visible Fingers
        foreach (int i in fingerIDs)
        {
            Vector3 transformedPosition = handTransform.TransformPoint(unityHand.leapFingers[i].TipPosition).ToUnityScaled();
            Vector basePosition = -unityHand.leapFingers[i].Direction * unityHand.leapFingers[i].Length;
            basePosition += unityHand.leapFingers[i].TipPosition;

            //angle = Quaternion.FromToRotation(Vector3.forward + baseTransformedPosition, transformedPosition).eulerAngles;
            angle = Quaternion.FromToRotation(Vector3.forward, transformedPosition).eulerAngles;
            rotation = joints[i].localEulerAngles;

            rotation.x = Mathf.LerpAngle(rotation.x, angle.x, seekSpeed);
            rotation.y = Mathf.LerpAngle(rotation.y, angle.y, seekSpeed);
            rotation.z = Mathf.LerpAngle(rotation.z, angle.z, seekSpeed);
            joints[i].localEulerAngles = rotation;
        }

        // Update Missing Fingers
        for (int i = 0; i < 5; i++)
        {
            if (!unityHand.leapFingers.ContainsKey(i))
            {
                rotation = joints[i].localEulerAngles;
                rotation.x = Mathf.LerpAngle(rotation.x, 90, returnSpeed);
                rotation.y = Mathf.LerpAngle(rotation.y, 0, returnSpeed);
                rotation.z = Mathf.LerpAngle(rotation.z, 0, returnSpeed);
                joints[i].localEulerAngles = rotation;
            }
        }
    }


}
