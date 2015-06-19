using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( SphereCollider ) )]
[AddComponentMenu( "" )]
public class AmplifyColorTriggerProxy : MonoBehaviour
{
	public Transform Reference;
	public AmplifyColorBase OwnerEffect;

	private SphereCollider sphereCollider;
	private Rigidbody rigidBody;

	void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.radius = 0.01f;
		sphereCollider.isTrigger = true;

		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		rigidBody.isKinematic = true;
	}

	void LateUpdate()
	{
		transform.position = Reference.position;
		transform.rotation = Reference.rotation;		
	}
}
