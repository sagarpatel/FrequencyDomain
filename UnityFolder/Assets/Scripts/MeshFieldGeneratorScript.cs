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

	public float[] rgbaArrayForm =  new float[4];
	public float rScale = 1.0f;
	public float bScale = 1.0f;
	public float gScale = 1.0f;

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
		 
		//GetComponent<MeshCollider>().sharedMesh = mesh;
		//DestroyImmediate(collider);
		//GetComponent<MeshCollider>().sharedMesh = null;
		//GetComponent<MeshCollider>().sharedMesh = mesh;


		/// This part handles the color
		for(int c = 0; c <3 ; c++)
		{
			float tempSum = 0;
			for(int i = c*verticesFrequencyDepthCount/3; i < (c*verticesFrequencyDepthCount/3 + verticesFrequencyDepthCount/3) ; i++)
			{
				tempSum += verticesArray[i].y;
			}

			rgbaArrayForm[c] = (float)tempSum/(float)(verticesFrequencyDepthCount/3);
		}
		rgbaArrayForm[3] = 1.0f;
		Color tempColor = new Color(rgbaArrayForm[0]*rScale, rgbaArrayForm[1]*gScale, rgbaArrayForm[2]*bScale, rgbaArrayForm[3]);
		GetComponent<MeshRenderer>().materials[0].color = tempColor;

	}

	// old version based on float to int casting, imprecise
	/*
	public float getHeightFromPosition(float x, float y)
	{
		float height = 0;

		int xInt = (int)x;
		int yInt = (int)y;

		int xScale = (int)transform.localScale.x;
		int yScale = (int)transform.localScale.z;

		//make sure position is within mesh
		if( xInt<0 || xInt >= verticesTimeDepthCount * xScale )
		{
			return 0;  //return height 0 is out of bounds
		}
		if( yInt <0 || yInt >= verticesFrequencyDepthCount * yScale )
		{
			return 0;
		}

		//find closest vertice to the position
		int arrayIndex = (xInt/xScale) * verticesFrequencyDepthCount + (yInt/yScale);
		//Debug.Log(arrayIndex);
		Vector3 tempVector = verticesArray[arrayIndex];
		height = tempVector.y;

		return height;
	}
	*/

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
