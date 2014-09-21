using UnityEngine;
using System.Collections;

public class LeapHandleState : LeapState
{

    private LeapHandleObject handleObj;

public LeapHandleState() { }


    public LeapHandleState(LeapGameObject obj)
	{
        activeObj = obj;
        handleObj = (LeapHandleObject)obj;
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
            if (IsGrabbing())
            {
                // Update active object
                activeObj.UpdateTransform(handController);
            }
            else
            {
                if (handleObj.canAccelerate)
                {
                    handleObj.brakes.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }

                handleObj.OpenHandUpdate();

                // Leave State
                if (Vector3.Distance(handleObj.releasePosition, handController.unityHand.transform.localPosition) > handleObj.releaseDistance)
                {
                    // Attempt to leave state
                    if (!handController.activeObj.isStatePersistent)
                    {
                        if (handController.activeObj.canRelease)
                        {
                            handController.ChangeState(handController.activeObj.Release(handController));
                        }
                    }
                }
            }

            CheckEscape();
        }
    }

    public override void Exit()
    {
        handController.ShowHand();
    }
}
