using UnityEngine;
using System.Collections;

public class RiderCameraTargetMeshGeneratorCreature : MonoBehaviour 
{
	public GameObject meshCreatureGenerator;

	void Start()
	{
	
	
	}

	void Update()
	{

		//Vector3 deOrientedCreaturePos = Quaternion.Inverse( meshCreatureGenerator.transform.rotation ) * meshCreatureGenerator.transform.position; 
		//Vector3 deOrientedCameraHolderPos = Quaternion.Inverse( transform.rotation) * transform.position;

		//float yDiff = Mathf.Abs( deOrientedCreaturePos.y - deOrientedCameraHolderPos.y );
		//loat zDiff = Mathf.Abs( deOrientedCreaturePos.z - deOrientedCameraHolderPos.z );

		//Debug.Log (yDiff + "   " + zDiff);

		//Debug.Log("Normal camera pos" + transform.position + "  deoriented: " + deOrientedCameraHolderPos  );

		Vector3 diffVec = meshCreatureGenerator.transform.position - transform.position;
		Vector3 deoritentedDiff = Quaternion.Inverse(transform.rotation) * diffVec ;
		Debug.Log(transform.position);


	}

}
