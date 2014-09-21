using UnityEngine;
using System.Collections;

public class TriggerableObject : MonoBehaviour 
{
	public int triggerID = -1; // Set this ID to a number, and set any objects that should receive its messages to the same number
	protected bool condition;

	
	void Update () 
	{
		if (condition)
			ExecuteAction();	
	}

	protected virtual void ExecuteAction()
	{
		Messenger.Broadcast<int>(SIG.TRIGGERACTIVATED.ToString(), triggerID);
	}


}
