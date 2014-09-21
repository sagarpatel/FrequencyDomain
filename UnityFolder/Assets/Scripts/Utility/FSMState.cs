using UnityEngine;

abstract public class FSMState  <T>   
{
	abstract public void Enter (T owner);
	abstract public void Execute();
    abstract public void OnTriggerStay(Collider o);
    abstract public void OnTriggerEnter(Collider o);
    abstract public void OnTriggerExit(Collider o);
	abstract public void OnCollisionEnter(Collision c);
	abstract public void OnCollisionStay(Collision c);
	abstract public void OnCollisionExit(Collision c);
	abstract public void Exit();
}