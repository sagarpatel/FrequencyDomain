using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	#region vars
	public Transform car;
	private Transform thisTransform;
	private Camera cam;

	public float offset;
	public float height;
	public float speed;
	public float rotationSpeed;

	private float defaultFOV = 60f;
	private float zoomRatio = .1f;
	#endregion

	#region UnityMethods
	void Start()
	{
		thisTransform = transform;
		cam = Camera.main;
	}

	void FixedUpdate()
	{
		Move();
		AdjustFOV();
	}
	#endregion

	#region Actions
	private void Move()
	{
		thisTransform.position = Vector3.Lerp(thisTransform.position, GetTarget(), Time.fixedDeltaTime * speed);
		thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, Quaternion.LookRotation(car.position - thisTransform.position, car.up), Time.fixedDeltaTime * rotationSpeed);
	}

	private Vector3 GetTarget()
	{
		Vector3 pos = new Ray(car.position, -(car.forward * offset) + (car.up * height)).GetPoint(offset);
		return pos;
	}

	private void AdjustFOV()
	{
		cam.fieldOfView = defaultFOV + car.rigidbody.velocity.magnitude * zoomRatio;
	}
	#endregion
}
