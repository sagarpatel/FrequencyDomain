using UnityEngine;
using System.Collections;

public class LeapShootableState : LeapState {

    LeapShootableObject shootableObj;

    public LeapShootableState() { }


    public LeapShootableState(LeapGameObject obj)
	{
        activeObj = obj;
        shootableObj = (LeapShootableObject)obj;
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

            shootableObj.CheckFireBullet();

            CheckEscape();
        }
    }

    public override void Exit()
    {
        handController.ShowHand();
    }

}
