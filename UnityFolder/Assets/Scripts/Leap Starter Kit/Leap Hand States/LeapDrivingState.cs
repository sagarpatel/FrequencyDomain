using UnityEngine;
using System.Collections;

public class LeapDrivingState : LeapState 
{
    public GameObject handOnSteeringWheel;

    public LeapDrivingState() { }


    public LeapDrivingState(LeapGameObject obj)
	{
        activeObj = obj;
        handOnSteeringWheel = ((LeapSteeringWheel)activeObj).PlaceHandOnSteeringWheel(handOnSteeringWheel);
	}

    public override void Enter(HandTypeBase o)
    {
        handController = o;
        if (activeObj)
            handController.SetActiveObject(activeObj);

        handController.HideHand();
    }

    public override void Execute()
    {
        if (handController.unityHand == null)
            return;

        if (handController.activeObj)
        {
            // Update active object
            activeObj.UpdateTransform(handController);

            // Attempt to leave state
            if (!IsGrabbing() && !handController.activeObj.isStatePersistent)
            {
                if (handController.activeObj.canRelease)
                {
                    handController.ChangeState(handController.activeObj.Release(handController));
                }
            }

            CheckEscape();
        }
    }

    public override void Exit()
    {
        GameObject.Destroy(handOnSteeringWheel);
        handController.ShowHand();
    }

}
