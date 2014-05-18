using UnityEngine;
using System.Collections;

public class PVA : MonoBehaviour 
{	
	public bool rotationPointsToCurrentVelocity = false;

	public Vector3 position;
	public Vector3 velocity;
	public Vector3 acceleration;

	[Range(0,0.1f)]
	public float velocityDecay = 0;

	[Range(0,1)]
	public float accelerationDecay = 0;

	public Space refrenceFrame = Space.World;
	Vector3 deltaPos;

	public bool isDecay = false;
	public float velocityKillThreashold = 0.0f;
	public Vector3 deltaV;
	Vector3 previousV;

	// Use this for initialization
	void Start () 
	{
		Init();
		//previousV = new Vector3(0, 0, 0);
	}

	void Init()
	{
		position = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{

		ApplyPVA();
		
		// do rotation, if necessary
		if(rotationPointsToCurrentVelocity)
		{
			if(velocity.magnitude != 0)
			{
				Vector3 direction = velocity;
				direction.Normalize();
				transform.forward = Vector3.Lerp(transform.forward, direction, 20.0f *Time.deltaTime);
			}
		}
	

	}

	public void ApplyPVA()
	{
		// do core PVA update
		//position = transform.position;
		//position += velocity * Time.deltaTime;
		deltaPos = velocity * Time.deltaTime;
		velocity += acceleration * Time.deltaTime;
		//transform.position = position;
		transform.Translate(deltaPos, refrenceFrame);

		if(isDecay)
		{
			// apply decay
			velocity = (1.0f - velocityDecay) * velocity;
			acceleration = (1.0f - accelerationDecay) * acceleration;
		}

		if( Mathf.Abs(velocity.x) <= velocityKillThreashold )
			velocity.x = 0;
		if( Mathf.Abs(velocity.y) <= velocityKillThreashold )
			velocity.y = 0;
		if( Mathf.Abs(velocity.z) <= velocityKillThreashold )
			velocity.z = 0;

		deltaV = velocity - previousV;
		previousV = velocity;
	}

	public void ResetPVA()
	{
		Init();
	}


}
