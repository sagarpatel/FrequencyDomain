using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshFieldGeneratorScript : MonoBehaviour 
{

	public float xScale = 1;
	public float zScale = 1;

	public Vector3[] verticesArray;
	public int[] trianglesArray;
	public Vector2[] uvArray;

	public int verticesFrequencyDepthCount = 64;
	public int verticesTimeDepthCount = 100;
	public int verticesStartFrequency = 1; // must be greater than 0
	public float verticesAudioHeightScale = 10.0f;

	

	AudioDirectorScript audioDirector;

	public Mesh mesh;
	public float minimumAmplitude = 0.001f;
	public int dataRepCount = 0;

	public float updateRefreshMinimum = 0.02f;
	float updateRefreshCounter;

	public Color currentColor;

	// Use this for initialization
	void Start () 
	{

		gameObject.AddComponent("MeshFilter");
        gameObject.AddComponent("MeshRenderer");
       // gameObject.AddComponent("MeshCollider");
        mesh = GetComponent<MeshFilter>().mesh;

        List<int> trianglesList = new List<int>();
        List<Vector3> verticesList = new List<Vector3>();

        
        List<Vector2> uvList = new List<Vector2>();


        // initial line
        for(int j = 0; j < verticesFrequencyDepthCount; j++)
    	{
    		verticesList.Add( new Vector3(0,0,j * zScale) );
    		uvList.Add( new Vector2(0,0) );
    	}

    	// populate the rest of the vertices, triangles
    	// use verticesFrequencyDepthCount to shift between frewuency collumns
        for(int i = 1; i < verticesTimeDepthCount  ; i++)
        {
        	for(int j = 0; j < verticesFrequencyDepthCount; j += 2)
        	{
        		// bottom left triangle
        		verticesList.Add( new Vector3(i * xScale,0, j * zScale) );
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
	        	verticesList.Add( new Vector3( i*xScale,0, (j + 1)*zScale ) );
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

		updateRefreshCounter += Time.deltaTime;

		if(updateRefreshCounter > updateRefreshMinimum)
		{
			//Debug.Log(updateRefreshCounter);
			updateRefreshCounter -= updateRefreshMinimum;

			Vector3 tempVector;
			// propagate old audio data along time axis
			for(int i = verticesArray.Length -1; i > verticesFrequencyDepthCount ; i--)
	        {
	    		tempVector = verticesArray[i];
	    		tempVector.y = verticesArray[i - verticesFrequencyDepthCount].y ;
	    		verticesArray[i] = tempVector;
	        }
			
	        // insert fresh audio data into first frequency collumn
			// copy one to dataRepCount+1
			float tempHeight = 0;
			for(int i = 1; i<verticesFrequencyDepthCount; i++)
			{
				tempVector = verticesArray[i];
				tempHeight = audioDirector.pseudoLogArrayBuffer[i/(dataRepCount+1)];
				tempVector.y = tempHeight * verticesAudioHeightScale; // normal version
				//tempVector.y = ( tempHeight * verticesAudioHeightScale + verticesArray[i + verticesFrequencyDepthCount].y)/2.0f ; // time axis smoothing version
				verticesArray[i] = tempVector;
			}
			// reset the audio data buffer
			for(int i = 0; i < audioDirector.pseudoLogArray.Length; i++)
				audioDirector.pseudoLogArrayBuffer[i] = 0;

			mesh.MarkDynamic();
			mesh.vertices = verticesArray;
			mesh.RecalculateNormals();

			currentColor = audioDirector.calculatedRGB;
			GetComponent<MeshRenderer>().materials[0].color = currentColor;
		}

	}


//// Functions

	public float getHeightFromPosition(float xPos, float zPos)
	{
		float height = 0;

		//normalize postion to a unit scale
		xPos = xPos / xScale;
		zPos = zPos / zScale;

		// Getting position 1 ahead
		//xPos = xPos -1;

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
