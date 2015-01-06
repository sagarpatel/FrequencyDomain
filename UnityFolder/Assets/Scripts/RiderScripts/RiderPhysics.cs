using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{
	float meshlineWidth = 600; // TODO: need to get world size of mesh instead of hardoding measured value editor. Myabe using mesh Bounds could work

	// where y is height.up, x is width and z is forwawrd
	public Vector3 relativeVelocity =  new Vector3();
	float maxForwardVelocityMagnitude = 0.150f;
	float maxSidewaysVelocityMagnitude = 100.0f;
	float linearVelocityDecay = 0.2f;

	float progressionOnMesh = 0; // should be clamped between 0 and 1
	float widthOffset = 0;
	float heightOffset = 0;
	float previousHeight = 0;

	float forwardMoveScale = 0.005f;
	float sideMoveScale = 10.0f;
	float gravityScale = -0.1f;
	float maxHeight = 200.0f;

	float rampupVelocityIncrementScale = 100.0f;

	MeshLinesGenerator meshlinesGenerator;

	void Start()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}

	void Update()
	{
		Vector3 tempPos = transform.position;

		Vector3 calPos = Vector3.zero;
		Quaternion calRot = Quaternion.identity;
		float newHeightOffset = 0;

		// updated based on velocities
		widthOffset += relativeVelocity.x * Time.deltaTime;
		widthOffset = Mathf.Clamp(widthOffset, -meshlineWidth * 0.5f, meshlineWidth * 0.5f);
		float relativeLocationOnLine = 0.5f + widthOffset/meshlineWidth;


		// move rider forward before looking for closest meshline
		progressionOnMesh += relativeVelocity.z * Time.deltaTime;
		progressionOnMesh = Mathf.Clamp(progressionOnMesh, 0,1);

		meshlinesGenerator.CalculatePositionAndRotationOnMesh(progressionOnMesh, relativeLocationOnLine , out calPos, out calRot, out newHeightOffset);

		HandleNewHeight(newHeightOffset);

		transform.position = calPos; // set base meshline postion
		transform.rotation = calRot;
		transform.position += transform.right * widthOffset;
		transform.position += transform.up * heightOffset;


		// apply friction to velocities
		relativeVelocity -=  linearVelocityDecay * relativeVelocity * Time.deltaTime;

	}

	void HandleNewHeight(float newHeight)
	{
		// if rising
		if(newHeight > previousHeight)
		{
			relativeVelocity.y += rampupVelocityIncrementScale * Time.deltaTime;
			heightOffset = newHeight;
		}
		else if(newHeight < previousHeight)
		{
			relativeVelocity.y += gravityScale * Time.deltaTime;
			heightOffset += relativeVelocity.y * Time.deltaTime;
		}
		else if( newHeight == previousHeight)
		{
			relativeVelocity.y = 0;
			heightOffset = newHeight;
		}


		heightOffset =  Mathf.Clamp( heightOffset, 0, maxHeight);

		previousHeight = newHeight;
	}

	public void MoveForward(float controlMagnitude)
	{
		relativeVelocity.z += controlMagnitude * forwardMoveScale;
		relativeVelocity.z = Mathf.Clamp(relativeVelocity.z, -maxForwardVelocityMagnitude, maxForwardVelocityMagnitude);
	}

	public void MoveSideways(float controlMagnitude)
	{
		relativeVelocity.x += controlMagnitude * sideMoveScale;
		relativeVelocity.x = Mathf.Clamp(relativeVelocity.x, -maxSidewaysVelocityMagnitude , maxSidewaysVelocityMagnitude);
	}


}
