using UnityEngine;
using System.Collections;

public class ButtonTrigger : TriggerableObject 
{

	public float pressSpeed = 4f;

	private Vector3 origin;
	private Vector3 triggerLoc;
	private float triggerHeight;
	private bool resetPeriod;
	private bool resetting;
	private bool newContact = true;

	void Start()
	{
		triggerLoc = origin = transform.position;
		triggerLoc.y -= transform.localScale.y;
		triggerHeight = triggerLoc.y + .1f;
	}

	void Update()
	{
		if (resetting)
		{
			transform.position = Vector3.Lerp(transform.position, origin, Time.fixedDeltaTime * pressSpeed);
			if (transform.position.y >= origin.y - .01f) // Close enough
				resetting = resetPeriod = false;
		}
	}

	private void OnTriggerStay(Collider c)
	{
		if (resetPeriod || !newContact)
			return;

		resetting = false;

		HandTypeBase h = c.GetComponent<HandTypeBase>();
		
		if (h)
		{
			Vector3 target = triggerLoc;
			target.y = Mathf.Clamp(c.transform.position.y - .6f, triggerLoc.y, origin.y);
			transform.position = Vector3.Lerp(transform.position, target, Time.fixedDeltaTime * pressSpeed);

			if (transform.position.y <= triggerHeight)
			{
				Invoke("EnableTrigger", 2f);
				resetPeriod = true;
				resetting = true;
				newContact = false;
				ExecuteAction();
			}
		}
	}

	private void OnTriggerExit(Collider c)
	{
		HandTypeBase h = c.GetComponent<HandTypeBase>();
		if (h && !resetPeriod)
			resetting = true;

		newContact = true;
	}

	private void EnableTrigger()
	{
		resetPeriod = false;
	}

}
