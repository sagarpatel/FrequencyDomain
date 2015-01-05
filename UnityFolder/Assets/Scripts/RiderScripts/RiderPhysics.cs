using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{
	float meshlineWidth = 600; // TODO: need to get world size of mesh instead of hardoding measured value editor. Myabe using mesh Bounds could work

	// where y is height.up, x is width and z is forwawrd
	public Vector3 relativeVelocity =  new Vector3();
	float maxForwardVelocityMagnitude = 300.0f;
	float maxSidewaysVelocityMagnitude = 100.0f;
	float linearVelocityDecay = 0.2f;

	float widthOffset = 0;
	float heightOffset = 0;

	float forwardMoveScale = 7.0f;
	float sideMoveScale = 10.0f;
	float gravityScale = -10.0f;
	float maxHeight = 200.0f;

	MeshLinesGenerator meshlinesGenerator;

	void Start()
	{
		meshlinesGenerator = FindObjectOfType<MeshLinesGenerator>();
	}

	void Update()
	{
		Vector3 tempPos = transform.position;
		relativeVelocity.y = gravityScale;

		Vector3 calPos = Vector3.zero;
		Quaternion calRot = Quaternion.identity;
		float newHeightOffset = 0;

		// updated based on velocities
		widthOffset += relativeVelocity.x * Time.deltaTime;
		widthOffset = Mathf.Clamp(widthOffset, -meshlineWidth * 0.5f, meshlineWidth * 0.5f);
		float relativeLocationOnLine = 0.5f + widthOffset/meshlineWidth;


		// move rider forward before looking for closest meshline
		transform.position += transform.forward * relativeVelocity.z * Time.deltaTime;


		meshlinesGenerator.CalculateClosestMeshLinePosition(transform.position, transform.rotation, relativeLocationOnLine , out calPos, out calRot, out newHeightOffset);

		if( newHeightOffset > heightOffset )
			heightOffset = newHeightOffset;


		transform.position = calPos; // set base meshline postion
		transform.rotation = calRot;
		transform.position += transform.right * widthOffset;
		transform.position += transform.up * heightOffset;


		// apply friction to velocities
		relativeVelocity -=  linearVelocityDecay * relativeVelocity * Time.deltaTime;
		heightOffset += heightOffset * relativeVelocity.y * Time.deltaTime;
		heightOffset =  Mathf.Clamp( heightOffset, 0, maxHeight);
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
