using UnityEngine;
using System.Collections;

/*objects don't necessarily need rigidbodies or mesh colliders
* it can be useful for travel aesthetics
*/ 
[RequireComponent(typeof(Rigidbody))]//), typeof(MeshCollider))]
public class Returnable : MonoBehaviour {

	#region vars
	private Vector3 origin;
	private Quaternion rot;
	private Transform thisTransform;
	private Rigidbody thisRigidbody;
	private Collider thisCollider;
	private float physicsToLerpThresh = 7;
	
	private delegate void CurrentAction();
	private CurrentAction Action;
	#endregion
	
	#region Unity methods
	void Start()
	{
		Action = DoNothing;
	}
	
	void Update()
	{
		Action();
	}
	#endregion
	
	
	#region movement
	private void LerpTravel()
	{
		float returnSpeed = (1/(thisTransform.position - origin).sqrMagnitude < .5f ) ? .5f : 1/(thisTransform.position - origin).magnitude;
		
		thisTransform.position = Vector3.Lerp(thisTransform.position, origin, Time.fixedDeltaTime * returnSpeed * 10);
		thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.fixedDeltaTime * returnSpeed * 10);
			
		//if object is close enough to origin, stop travelling
		if((thisTransform.position - origin).sqrMagnitude < .001f)
		{	
			thisTransform.position = origin;
			thisTransform.rotation = rot;

            thisRigidbody.isKinematic = false;
            thisCollider.enabled = true;
            thisRigidbody.velocity = Vector3.zero;
            thisRigidbody.angularVelocity = Vector3.zero;
            thisCollider.isTrigger = false;

			//rigidbody actions and collider actions go here
			Action = DoNothing;
		}
	}
	
	//method for returning objects via their rigidbody/physics
	//the difference between lerp and physics movement is mostly aesthetic
	private void RigidBodyTravel()
	{
		thisRigidbody.AddForce((origin - thisTransform.position).normalized, ForceMode.Impulse);
		//if object is close enough to origin, switch to LerpTravel() to ensure arrival at destination
		if(Vector3.Distance(thisTransform.position, origin) < physicsToLerpThresh)
		{
            thisRigidbody.isKinematic = true;
            thisCollider.enabled = false;
			Action = LerpTravel;
		}
	}
	
	private void DoNothing()
	{
	}
	#endregion
	
	#region triggers
	public void Activated(Vector3 position)
	{
		//this portion used to control exploding
		//put enabling / disabling / on pickup actions here
		thisRigidbody.isKinematic = true;
		thisCollider.enabled = false;
		thisRigidbody.AddExplosionForce(500, position, 150);

	}
	
	public void Initialize()
	{
		thisTransform = transform;
		origin = thisTransform.position;
		rot = thisTransform.rotation;
		thisRigidbody = rigidbody;
		thisRigidbody.isKinematic = false;
		thisRigidbody.useGravity = true;
		thisCollider = collider;
		thisCollider.enabled = true;
        thisRigidbody.velocity = Vector3.zero;
		thisRigidbody.angularVelocity = Vector3.zero;
	}
	
	public void Reset()
	{	
		Action = RigidBodyTravel;
		Invoke("GoHome", .2f);
	}

	private void GoHome()
	{
		Action = LerpTravel;

		thisRigidbody.isKinematic = true;
		thisCollider.enabled = false;
        thisCollider.isTrigger = true;
	}

	private void GoHomeImmediate()
	{
		Action = LerpTravel;
		thisRigidbody.isKinematic = true;
		thisCollider.enabled = false;
        thisRigidbody.useGravity = false;
        thisCollider.isTrigger = true;
	}
	#endregion
	
	
}
