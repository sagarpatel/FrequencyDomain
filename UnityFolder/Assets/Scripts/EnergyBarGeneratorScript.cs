using UnityEngine;
using System.Collections;

public class EnergyBarGeneratorScript : MonoBehaviour 
{

	public GameObject energyBarPiecePrefab;
	public GameObject[] energyPiecesArray = new GameObject[10];

	public float radius = 200.0f;
	public float depthScale = 1.0f;
	public float rotationCount = 1.0f;

	// Use this for initialization
	void Start () 
	{

		Vector3 tempPosition;
		float rotationFactor = (rotationCount / (float)energyPiecesArray.Length) * (2*Mathf.PI) ;
		for(int i =0; i < energyPiecesArray.Length; i++)
		{
			tempPosition = new Vector3( -(float)i*depthScale , radius * Mathf.Cos( (float)i*rotationFactor), radius* Mathf.Sin( (float)i*rotationFactor)  );
			energyPiecesArray[i] = (GameObject)Instantiate(energyBarPiecePrefab, tempPosition, transform.localRotation );
			energyPiecesArray[i].transform.parent = transform;
			radius --;
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}



}
