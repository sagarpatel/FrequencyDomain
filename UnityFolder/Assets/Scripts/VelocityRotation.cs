using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PVA))]
public class VelocityRotation : MonoBehaviour 
{
	PVA pva;

	public float xScale = 1.0f;
	public float yScale = 1.0f;
	public float zScale = 1.0f;

	// Use this for initialization
	void Start () 
	{
		pva = GetComponent<PVA>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		transform.Rotate(Vector3.right, pva.velocity.y * yScale * Time.deltaTime , Space.Self);

		transform.Rotate(Vector3.up, pva.velocity.x * xScale * Time.deltaTime , Space.Self);
		transform.Rotate(Vector3.forward, pva.zRotationVelocity * zScale * Time.deltaTime , Space.Self);		
	}
}
