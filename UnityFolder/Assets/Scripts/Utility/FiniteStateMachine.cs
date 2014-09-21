using UnityEngine;

public class FiniteStateMachine <T>  
{
	private T Owner;
	public FSMState<T> CurrentState;
    private FSMState<T> PreviousState;
	
	
	public void Awake()
	{
		CurrentState = null;
		PreviousState = null;
	}

    public void Initialize(T owner, FSMState<T> InitialState) 
	{
		Owner = owner;
		ChangeState(InitialState);
	}
	
	public void  Update() 
	{
		if (CurrentState != null) CurrentState.Execute();
	}

    public void OnTriggerEnter(Collider o)
    {
        if (CurrentState != null) CurrentState.OnTriggerEnter(o);
    }

    public void OnTriggerStay(Collider o)
    {
        if (CurrentState != null) CurrentState.OnTriggerStay(o);
    }

    public void OnTriggerExit(Collider o)
    {
        if (CurrentState != null) CurrentState.OnTriggerExit(o);
    }

    public void ChangeState(FSMState<T> NewState) 
	{
		PreviousState = CurrentState;
		if (PreviousState != null)
		{
			//Debug.Log(Owner + "EXITED STATE: " + PreviousState);
			PreviousState.Exit();
		}
		//Debug.Log(Owner + "ENTERED STATE: " + NewState);
		CurrentState = NewState;
		CurrentState.Enter(Owner);
	}
	
	public void  RevertToPreviousState() 
	{
		if (PreviousState != null)
		  ChangeState(PreviousState);
	}

	public void OnCollisionEnter(Collision c)
	{
		if (CurrentState != null) CurrentState.OnCollisionEnter(c);
	}

	public void OnCollisionStay(Collision c)
	{
		if (CurrentState != null) CurrentState.OnCollisionStay(c);
	}

	public void OnCollisionExit(Collision c)
	{
		if (CurrentState != null) CurrentState.OnCollisionExit(c);
	}
}