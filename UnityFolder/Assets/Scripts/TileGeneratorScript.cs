using UnityEngine;
using System.Collections;

public class TileGeneratorScript : MonoBehaviour 
{

	public int widthCount = 10;
	public int depthCount = 5;
	public float widthSeperation = 10;
	public float depthSeperation = 20;
	public float initialPositionOffset_x = 0;
	public float initialPositionOffset_z = 0;

	public GameObject tilePrefab;
	public GameObject[] tilesArray;
	int tilesCounter = 0;

	Vector3[] targetPositionArray;


	public float updateRefreshMinimum = 0.02f;
	float updateRefreshCounter;

	PlayerScript playerScript;

	// Use this for initialization
	void Start () 
	{
		tilesArray = new GameObject[widthCount * depthCount];
		targetPositionArray = new Vector3[widthCount * depthCount];
		// instantiate/create all the tiles
		for(int i = 0; i < depthCount; i++ )
		{
			for(int j = 0; j < widthCount; j++)
			{
				tilesArray[tilesCounter] = (GameObject)Instantiate( tilePrefab,
																	new Vector3( -i * depthSeperation + initialPositionOffset_x, 0 , j * widthSeperation + initialPositionOffset_z), 
																	Quaternion.identity);
				targetPositionArray[tilesCounter] = tilesArray[tilesCounter].transform.position;
				tilesArray[tilesCounter].transform.parent = transform;
				tilesCounter ++;
			}

		}

		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");


		//last step, activate children combination
		//GetComponent<CombineChildren>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

		updateRefreshCounter += Time.deltaTime;
		if(updateRefreshCounter > updateRefreshMinimum)
		{
			updateRefreshCounter -= updateRefreshMinimum;
			PropagateTargetArray();	

		}

		FollowPlayerHeight();
		LerpToTargetPosition();

	
	}


	// sets first row to current player height
	void FollowPlayerHeight()
	{
		Vector3 tempPosition;
		Vector3 newPlayerPosition = playerScript.transform.position;
		for(int i = 0; i < widthCount; i++)
		{
			tempPosition = targetPositionArray[i];
			tempPosition.y = newPlayerPosition.y;
			targetPositionArray[i] = tempPosition;
		}

	}

	void PropagateTargetArray()
	{
		Vector3 tempPosition;
		for(int i = tilesCounter -1; i >= widthCount; i--)
		{
			tempPosition = targetPositionArray[i];
			tempPosition.y = targetPositionArray[i - widthCount].y;
			targetPositionArray[i] = tempPosition;
		}
	}

	void LerpToTargetPosition()
	{
		for(int i = 0; i < tilesCounter; i++)
		{
			tilesArray[i].transform.position = Vector3.Lerp( tilesArray[i].transform.position, targetPositionArray[i], updateRefreshCounter/updateRefreshMinimum);

		}
	}


}
