using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshFieldGeneratorScript : MonoBehaviour 
{

	public Vector3[] verticesArray;
	public int[] trianglesArray;

	public int verticesFrequencyDepthCount = 64;
	public int verticesTimeDepthCount = 100;
	public int verticesStartFrequency = 1; // must be greater than 0
	public float verticesAudioHeightScale = 10.0f;

	

	AudioDirectorScript audioDirector;

	public Mesh mesh;
	public float minimumAmplitude = 0.001f;
	public int dataRepCount = 0;

	// Use this for initialization
	void Start () 
	{

		gameObject.AddComponent("MeshFilter");
        gameObject.AddComponent("MeshRenderer");
        gameObject.AddComponent("MeshCollider");
        mesh = GetComponent<MeshFilter>().mesh;

        List<int> trianglesList = new List<int>();
        List<Vector3> verticesList = new List<Vector3>();

        Vector2[] uvArray;
        List<Vector2> uvList = new List<Vector2>();


        // initial line
        for(int j = 0; j < verticesFrequencyDepthCount; j++)
    	{
    		verticesList.Add( new Vector3(0,0,j) );
    		uvList.Add( new Vector2(0,0) );
    	}

    	// populate the rest of the vertices, triangles
    	// use verticesFrequencyDepthCount to shift between frewuency collumns
        for(int i = 1; i < verticesTimeDepthCount  ; i++)
        {
        	for(int j = 0; j < verticesFrequencyDepthCount; j += 2)
        	{
        		// bottom left triangle
        		verticesList.Add( new Vector3(i,0, j) );
        		int currentListIndex = verticesList.Count -1;

	        	trianglesList.Add(currentListIndex);
	        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount);
	        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount + 1);

	        	// fill triangles in between this and previous triangle below
	        	if( j > 0) // is not at the edge
	        	{
	        		// bottom left triangle
	        		trianglesList.Add(currentListIndex -1);
		        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount -1);
		        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount);

		        	// top right triangle
		        	trianglesList.Add(currentListIndex);
		        	trianglesList.Add(currentListIndex - 1);
		        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount);
	        	}	

	        	// top right triangle
	        	verticesList.Add( new Vector3(i,0, j + 1) );
	        	currentListIndex++;
	        	
	        	trianglesList.Add(currentListIndex);
	        	trianglesList.Add(currentListIndex - 1);
	        	trianglesList.Add(currentListIndex - verticesFrequencyDepthCount);
  	
	        	uvList.Add( new Vector2(0,0) );
	        	uvList.Add( new Vector2(0,0) );
        	}
        }

        verticesArray = verticesList.ToArray();
		trianglesArray = trianglesList.ToArray();
		uvArray = uvList.ToArray();

		Debug.Log(trianglesList.Count);

		mesh.Clear();
		mesh.MarkDynamic();
		mesh.vertices = verticesArray;
		mesh.uv = uvArray;
		mesh.triangles = trianglesArray;
		mesh.RecalculateNormals();

		GetComponent<MeshRenderer>().materials[0].color = Color.green;
		renderer.material.shader = Shader.Find("Parallax Diffuse");

		//GetComponent<MeshCollider>().sharedMesh = mesh;
	
		audioDirector = (AudioDirectorScript) GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	

	void Update () 
	{
		Vector3 tempVector;

		// propagate old audio data along time axis
		for(int i = verticesArray.Length -1; i > verticesFrequencyDepthCount ; i--)
        {
    		tempVector = verticesArray[i];
    		tempVector.y = verticesArray[i - verticesFrequencyDepthCount].y;
    		verticesArray[i] = tempVector;
        }
		
        // insert fresh audio data into first frequency collumn
		// copy one to one
		float tempHeight = 0;
		for(int i = 1; i<verticesFrequencyDepthCount; i++)
		{
			tempVector = verticesArray[i];
			tempHeight = audioDirector.pseudoLogArray[i/(dataRepCount+1)];
			if( tempHeight < minimumAmplitude)
				tempVector.y = 0.0f;
			else
				tempVector.y = tempHeight * verticesAudioHeightScale;

			verticesArray[i] = tempVector;
		}

		mesh.MarkDynamic();
		mesh.vertices = verticesArray;
		mesh.RecalculateNormals();

		GetComponent<MeshRenderer>().materials[0].color = audioDirector.calculatedRGB;

	}


//// Functions

	public float getHeightFromPosition(float xPos, float zPos)
	{
		float height = 0;

		float xScale = transform.localScale.x;
		float zScale = transform.localScale.z;

		//normalize postion to a unit scale
		xPos = xPos / xScale;
		zPos = zPos / zScale;

		float xFloor = Mathf.Floor(xPos);
		float xCeil = Mathf.Ceil(xPos);
		float zFloor = Mathf.Floor(zPos);
		float zCeil = Mathf.Ceil(zPos);

		//make sure position is within mesh
		if( xFloor <0 || xCeil >= (float)verticesTimeDepthCount  )
			return 0;  //return height 0 is out of bounds
		if( zFloor <0 || zCeil >= (float)verticesFrequencyDepthCount  )
			return 0;

		// get height of 4 corners arround position
		float TL = getHeightFromHeightMap( (int)xFloor, (int)zFloor );
		float TR = getHeightFromHeightMap( (int)xFloor, (int)zCeil );
		float BL = getHeightFromHeightMap( (int)xCeil, (int)zFloor );
		float BR = getHeightFromHeightMap( (int)xCeil, (int)zCeil );

		float xLeftLerp = Mathf.Lerp(BL, TL, (xCeil - xPos ) );
		float xRightLerp = Mathf.Lerp(BR, TR, (xCeil - xPos) ) ;

		height = Mathf.Lerp( xLeftLerp, xRightLerp, zPos - zFloor );

		return height;
	}

	public float getHeightFromHeightMap(int x, int z)
	{
		float heightmapHeight = 0;

		int arrayIndex = (x) * verticesFrequencyDepthCount + (z);
		Vector3 tempVector = verticesArray[arrayIndex];
		heightmapHeight = tempVector.y;

		return heightmapHeight;
	}

	public bool isPlayerInMeshBounds(float xPos, float zPos)
	{
		float xScale = transform.localScale.x;
		float zScale = transform.localScale.z;

		//normalize postion to a unit scale
		xPos = xPos / xScale;
		zPos = zPos / zScale;

		float xFloor = Mathf.Floor(xPos);
		float xCeil = Mathf.Ceil(xPos);
		float zFloor = Mathf.Floor(zPos);
		float zCeil = Mathf.Ceil(zPos);

		//make sure position is within mesh
		if( xFloor <0 || xCeil >= (float)verticesTimeDepthCount  )
		{
			if( zFloor <0 || zCeil >= (float)verticesFrequencyDepthCount  )
				return true;
			else
				return false;
		}
		else
			return false;
	}

}
