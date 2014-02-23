using UnityEngine;

public class LeapState : FSMState<HandTypeBase> 
{
	public HandTypeBase handController;
	public LeapGameObject activeObj;

    public KeyCode escape = KeyCode.Escape;

	public override void Enter(HandTypeBase o) 
	{ 
		handController = o; 
	}

	public override void Execute() 
	{

	}
	public override void OnTriggerStay(Collider c) 
	{

	}
	public override void OnTriggerEnter(Collider c) 
	{

	}
	public override void OnTriggerExit(Collider c) 
	{

	}
	public override void OnCollisionEnter(Collision c) 
	{
	
	}
	public override void OnCollisionStay(Collision c) 
	{
	
	}
	public override void OnCollisionExit(Collision c) 
	{
	
	}
	public override void Exit() 
	{
	
	}

    public bool IsGrabbing()
    {
        return handController.unityHand.isHandDetermined && handController.unityHand.hand.Fingers.Count < 2;
    }

    public void CheckEscape()
    {
        if (Input.GetKeyDown(escape))
        {
            if (!handController.activeObj.isStatePersistent)
            {
                handController.ChangeState(handController.activeObj.Release(handController));
            }
        }
    }
}