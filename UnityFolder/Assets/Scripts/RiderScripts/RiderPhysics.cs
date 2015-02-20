using UnityEngine;
using System.Collections;

public class RiderPhysics : MonoBehaviour 
{
	float meshlineWidth = 4800; // TODO: need to get world size of mesh instead of hardoding measured value editor. Myabe using mesh Bounds could work

	// where y is height.up, x is width and z is forwawrd
	public Vector3 relativeVelocity =  new Vector3();
	float maxForwardVelocityMagnitude = 0.150f;
	float maxSidewaysVelocityMagnitude = 1600.0f;
	float maxUpVelocity = 500.0f;
	float linearVelocityDecay = 1.2f;

	float progressionOnMesh = 0; // should be clamped between 0 and 1
	float widthOffset = 0;
	float heightOffset = 0;
	float previousHeight = 0;

	float forwardMoveScale = 0.005f;
	float sideMoveScale = 20.0f;
	float gravityScale = -800.0f;
	float maxHeight = 100000.0f;

	float rampupVelocityIncrementScale = 400.0f;

	MeshLinesGenerator meshlinesGenerator;

	bool wasSidePressed = false;
	bool wasForwardPressed = false;

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
		//relativeVelocity -=  linearVelocityDecay * relativeVelocity * Time.deltaTime;

		if(wasSidePressed == false)
		{
			relativeVelocity.x -= linearVelocityDecay * relativeVelocity.x * Time.deltaTime;
			if(Mathf.Abs(relativeVelocity.x) < 0.01f)
				relativeVelocity.x =0;
		}
		if(wasForwardPressed == false)
		{
			relativeVelocity.z -= linearVelocityDecay * relativeVelocity.z * Time.deltaTime;
		}

		wasSidePressed = false;
		wasForwardPressed = false;
	}

	void HandleNewHeight(float newHeight)
	{
		// if rising
		if(newHeight > previousHeight)
		{
			relativeVelocity.y += rampupVelocityIncrementScale * Time.deltaTime;
			relativeVelocity.y = Mathf.Clamp(relativeVelocity.y,0,maxUpVelocity);
			heightOffset = newHeight;
		}
		else if(newHeight < previousHeight) // if fallingß
		{
			relativeVelocity.y += gravityScale * Time.deltaTime;
			heightOffset += relativeVelocity.y * Time.deltaTime;
			//Debug.Log("FALLING vel: " + relativeVelocity.y + "  at time: " + Time.time);
		}
		else if( newHeight == previousHeight)
		{
			relativeVelocity.y = 0;
			heightOffset = newHeight;
		}


		heightOffset =  Mathf.Clamp( heightOffset, 0, maxHeight);

		previousHeight = heightOffset;
	}

	public void MoveForward(float controlMagnitude)
	{
		relativeVelocity.z += controlMagnitude * forwardMoveScale;
		relativeVelocity.z = Mathf.Clamp(relativeVelocity.z, -maxForwardVelocityMagnitude, maxForwardVelocityMagnitude);
		wasForwardPressed = true;
	}

	public void MoveSideways(float controlMagnitude)
	{

			
		if(relativeVelocity.x > 0 && controlMagnitude < 0)
			relativeVelocity.x += controlMagnitude * sideMoveScale * 2.0f;
		else if(relativeVelocity.x < 0 && controlMagnitude > 0)
			relativeVelocity.x += controlMagnitude * sideMoveScale * 2.0f;
		else
			relativeVelocity.x += controlMagnitude * sideMoveScale;


		relativeVelocity.x = Mathf.Clamp(relativeVelocity.x, -maxSidewaysVelocityMagnitude , maxSidewaysVelocityMagnitude);
		wasSidePressed = true;
	}

	// ranges from -1 to 1
	public float GetSideMoveProgressRatio()
	{
		float ratio = relativeVelocity.x/maxSidewaysVelocityMagnitude;
		return ratio;
	}
}
