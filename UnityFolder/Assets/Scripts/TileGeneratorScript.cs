using UnityEngine;
using System.Collections;

public class TileGeneratorScript : MonoBehaviour 
{

	public int widthCount = 10;
	public int depthCount = 5;
	public float widthSeperation = 10;
	public float depthSeperation = 20;

	public GameObject tilePrefab;
	GameObject[] tilesArray;
	int tilesCounter = 0;

	// Use this for initialization
	void Start () 
	{
		tilesArray = new GameObject[widthCount * depthCount];
		// instantiate/create all the tiles
		for(int i = 0; i < depthCount; i++ )
		{
			for(int j = 0; j < widthCount; j++)
			{
				tilesArray[tilesCounter] = (GameObject)Instantiate( tilePrefab, new Vector3( -i * depthSeperation, 0 , j * widthSeperation), Quaternion.identity);
				tilesArray[tilesCounter].transform.parent = transform;
				//tilesArray[tilesCounter].renderer.sharedMaterial.shader = Shader.Find("Parallax Diffuse");
				tilesCounter ++;
			}

		}


		//last step, activate children combination
		GetComponent<CombineChildren>().enabled = true;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}



}
