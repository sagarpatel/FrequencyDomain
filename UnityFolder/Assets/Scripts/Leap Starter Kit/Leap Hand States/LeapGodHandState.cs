using UnityEngine;

public class LeapGodHandState : LeapState
{
	private LeapGameObject highlightObj;

	public LeapGodHandState() { }

	  	
	public LeapGodHandState(LeapGameObject obj)
	{
		activeObj = obj;
	} 
		

	public override void Enter(HandTypeBase o) 
	{ 
		handController = o;
        if (activeObj)
        {
            handController.SetActiveObject(activeObj);

            if (!activeObj.handIsVisible)
                handController.HideHand();
        }
	}
	
	public override void Execute() 
	{
		if (handController.unityHand == null)
			return;

		if (handController.activeObj)
		{
            // Update active object
            handController.activeObj.UpdateTransform(handController);

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

	public override void OnTriggerEnter(Collider c)
	{
		LeapGameObject obj = c.GetComponent<LeapGameObject>();
		if (!handController.activeObj && obj)	//if we're not already holding an object, and a new object was collided with, select object
		{
			HighlightClosest(obj);
		}
	}

	public override void OnTriggerStay(Collider c) 
	{
		LeapGameObject obj = c.GetComponent<LeapGameObject>();
		if (!handController.activeObj && obj)	//if we're not already holding an object, continue checking closest object to select
		{
			HighlightClosest(obj);
			if (highlightObj != null && obj != highlightObj) { obj.DeSelect(); return; }  // Deselect and exit function if this is not the highlighted object
			
			if (IsGrabbing() && (highlightObj.owner == null || highlightObj.canUseBothHands))
			{
				highlightObj.DeSelect();
				handController.ChangeState(highlightObj.Activate(handController));
			}
		}
	}
	
	public override void OnTriggerExit(Collider c) 
	{
		LeapGameObject obj = c.GetComponent<LeapGameObject>();
		if (obj)
		{
			if (obj == highlightObj)
				highlightObj = null;
			obj.DeSelect();
		}
	}

	public override void OnCollisionEnter(Collision c)
	{
		// Currently Not Implemented
	}

	public override void OnCollisionStay(Collision c)
	{
		// Currently Not Implemented
	}

	public override void OnCollisionExit(Collision c)
	{
		// Currently Not Implemented
	}
	
	public override void Exit() 
	{
        handController.unityHand.runUpdate = true;

        if (activeObj && !activeObj.handIsVisible)
            handController.ShowHand();
	}

	private void HighlightClosest(LeapGameObject obj)
	{
		if (highlightObj)
			highlightObj = Vector3.Distance(obj.transform.position, handController.transform.position) < Vector3.Distance(highlightObj.transform.position, handController.transform.position) ? obj : highlightObj;
		else
			highlightObj = obj;
		
		highlightObj.Select();
	}
}
