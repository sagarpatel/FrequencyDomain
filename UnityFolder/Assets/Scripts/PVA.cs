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

	// Use this for initialization
	void Start () 
	{
		Init();
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

		// apply decay
		velocity = (1.0f - velocityDecay) * velocity;
		acceleration = (1.0f - accelerationDecay) * acceleration;
	}

	public void ResetPVA()
	{
		Init();
	}


}
