using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandTypeMultiple : HandTypeBase 
{
	public List<HandModel> models;
	private HANDSTATE handModelState;

	protected override void Awake()
	{
		base.Awake();

		foreach (HandModel model in models)
			model.Initialize(transform);
		
		stateController.Initialize(this, new LeapGodHandState());
	}

	public override void UpdateHandType()
	{
		base.UpdateHandType();
		UpdateHandModel();
		UpdateState();
	}

	private void UpdateHandModel()
	{
        if (canBeVisible)
        {
            if (unityHand.isHandDetermined)
                handModelState = unityHand.hand.Fingers.Count < 2 ? HANDSTATE.CLOSED : HANDSTATE.OPEN;
            else
                handModelState = HANDSTATE.UNKNOWN;
        }

		// Enable Hand Models
		foreach (HandModel model in models)
			model.SetActive(handModelState);
	}


	public override void HandLost()
	{
        HideHand();
		base.HandLost();
	}

    public override void HandFound()
    {
        ShowHand();
        base.HandFound();
    }

    public override void HideHand()
    {
        canBeVisible = false;
        handModelState = HANDSTATE.INACTIVE;
        foreach (HandModel model in models)
            model.SetActive(handModelState);
    }
}
