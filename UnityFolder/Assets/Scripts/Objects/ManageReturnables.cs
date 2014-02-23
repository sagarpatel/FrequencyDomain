using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManageReturnables : MonoBehaviour {
	

	#region vars
	public Returnable[] pieces; 
	private int count = 0;
	public bool reset;
	public int triggerID = -1;
	#endregion
	
	#region unity methods
	void OnEnable()
	{
		foreach (Transform c in transform)
		{
			count += c.childCount;
			if (c.childCount < 1)
				count++;
		}

		pieces = new Returnable[count];
		count = 0;
		SearchChildren(transform);

		if (triggerID >= 0) //if no trigger ID is set, do not register for trigger actions
			Messenger.AddListener<int>(SIG.TRIGGERACTIVATED.ToString(), TriggerAction);
	}

	void Update()
	{
		if (reset)
		{
			Reset();
			reset = false;
		}
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.name == "bullet(Clone)")
		{
			collider.enabled = false;
			foreach (Returnable r in pieces)
				r.Activated(transform.position);
		}	
	}
	#endregion
	
	#region actions
	public void Reset()
	{
		foreach (Returnable r in pieces)
			r.Reset();
	}
	#endregion

	#region initialize
	private void SearchChildren(Transform t)
	{
		if(t.childCount < 1)
			Initialize(t);
		
		else
		{
			foreach(Transform c in t)
				SearchChildren(c);
		}
	}
	
	private void Initialize(Transform t)
	{
		t.gameObject.AddComponent<Returnable>();
		Returnable f = t.gameObject.GetComponent<Returnable>();
		f.Initialize();
		
		pieces[count] = f;
		count ++;
	}

	private void TriggerAction(int Id)
	{
		if (Id == triggerID)
			Reset();
	}
	#endregion
}
