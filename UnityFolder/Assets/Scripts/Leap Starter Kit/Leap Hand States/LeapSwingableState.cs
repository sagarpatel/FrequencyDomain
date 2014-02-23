using UnityEngine;
using System.Collections;

public class LeapSwingableState : LeapState {

    LeapSwingableObject swingableObj;

    public LeapSwingableState() { }


    public LeapSwingableState(LeapGameObject obj)
	{
        activeObj = obj;
        swingableObj = (LeapSwingableObject)obj;
	}

    public override void Enter(HandTypeBase o)
    {
        handController = o;
        if (activeObj)
            handController.SetActiveObject(activeObj);

        swingableObj.isHeld = true;
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

            swingableObj.CheckSwipe();

            CheckEscape();
        }
    }

    public override void Exit()
    {
        swingableObj.isHeld = false;
        handController.ShowHand();
    }

}
