using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshLinesGenerator : MonoBehaviour 
{
	public GameObject meshLinePrefab;

	public float spawnCooldown = 1.0f;
	float spawnCooldownCounter = 0.0f;

	[Range (0,100)]
	public float meshSpeed;

	public Material meshMaterial;

	int dataRepCount = 1;

	AudioDirectorScript audioDirector;

	public int verticesFrequencyDepthCount = 200;
	Mesh calculationsMiniMesh;
	Vector3[] miniVertsArray;

	Vector3[] vertsArrayLast2;
	Vector3[] newLineNormals;

	float xScale = 1.0f;
	float zScale = 1.0f;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		GenerateCalculationsMiniMesh();

		vertsArrayLast2 = new Vector3[2 * verticesFrequencyDepthCount];
		newLineNormals = new Vector3[verticesFrequencyDepthCount];

	}
	
	// Update is called once per frame
	void Update () 
	{
	
		spawnCooldownCounter += Time.deltaTime;
		if(spawnCooldownCounter > spawnCooldown)
		{
			spawnCooldownCounter -= spawnCooldown;

			GenerateLineMesh();

		}

	}

	void GenerateLineMesh()
	{
		GameObject newMeshLineGO = (GameObject)Instantiate(meshLinePrefab, transform.position, Quaternion.identity);
		
		Mesh mesh = newMeshLineGO.GetComponent<MeshFilter>().mesh;

		Vector3[] verticesArray = new Vector3[200];
		for(int i = 0; i < verticesArray.Length; i++)
		{
			verticesArray[i] = new Vector3(i, 0, 0);
		}


		// SET HEIGHT

		float tempHeight = 0;
		Vector3 tempVector = new Vector3(0, 0, 0);
		List<int> trianglesList = new List<int>();
		for(int i = 1; i<200; i++)
		{
			tempVector = verticesArray[i];
			tempHeight = audioDirector.pseudoLogArrayBuffer[i/(dataRepCount+1)];
			tempVector.y = tempHeight; //* verticesAudioHeightScale * yScale; // normal version
			//tempVector.y = ( tempHeight * verticesAudioHeightScale + verticesArray[i + verticesFrequencyDepthCount].y)/2.0f ; // time axis smoothing version
			verticesArray[i] = tempVector;

			if(i==0)
			{
				trianglesList.Add(199);
				trianglesList.Add(i);
				trianglesList.Add(i+1);
			}
			else if(i == 199)
			{
				trianglesList.Add(0);
				trianglesList.Add(199);
				trianglesList.Add(198);
			}
			else
			{
				trianglesList.Add(i - 1);
				trianglesList.Add(i);
				trianglesList.Add(i+1);
			}
		}

		// reset the audio data buffer
		for(int i = 0; i < audioDirector.pseudoLogArray.Length; i++)
			audioDirector.pseudoLogArrayBuffer[i] = 0;


		int[] indicesArray;
		List<int> indicesList = new List<int>();
		for(int i =0; i < verticesArray.Length - 1; i++)
		{
			indicesList.Add(i);
			indicesList.Add(i +1);
		}
		indicesArray = indicesList.ToArray();

		mesh.Clear();
		mesh.vertices = verticesArray;
		//mesh.triangles = trianglesList.ToArray();
		//mesh.RecalculateNormals();




		mesh.SetIndices (indicesArray, MeshTopology.Lines, 0);

		// calculate normals

		// push down normals
		for(int i = 0; i < verticesFrequencyDepthCount; i ++)
		{
			vertsArrayLast2[i + verticesFrequencyDepthCount] = vertsArrayLast2[i];
		}
		// insert new data
		for(int i = 0; i < verticesFrequencyDepthCount; i++)
		{
			vertsArrayLast2[i] = verticesArray[i];
		}
		
		calculationsMiniMesh.vertices = vertsArrayLast2;
		calculationsMiniMesh.RecalculateNormals();

		for(int i = 0; i < verticesFrequencyDepthCount; i++)
		{
			newLineNormals[i] = calculationsMiniMesh.normals[i];
		}

		mesh.normals = newLineNormals;



		//Debug.Log("MESH GENERATED");

		newMeshLineGO.GetComponent<PVA>().velocity = meshSpeed *transform.forward;

		newMeshLineGO.renderer.sharedMaterial = meshMaterial;
		meshMaterial.color = audioDirector.calculatedRGB;
		//newMeshLineGO.renderer.material.color = audioDirector.calculatedRGB;
	}


	void GenerateCalculationsMiniMesh()
	{

		GameObject calculationsMiniMeshGameObject = new GameObject("CalculationsMiniMesh");
		calculationsMiniMeshGameObject.AddComponent("MeshFilter");
		//calculationsMiniMeshGameObject.AddComponent("MeshRenderer");
		calculationsMiniMesh = calculationsMiniMeshGameObject.GetComponent<MeshFilter>().mesh;

        List<int> trisList = new List<int>();
        List<Vector3> vertsList = new List<Vector3>();

        List<Vector2> tempUVList = new List<Vector2>();

        // initial line
        for(int j = 0; j < verticesFrequencyDepthCount; j++)
    	{
    		vertsList.Add( new Vector3(0,0,j * zScale) );
    		tempUVList.Add( new Vector2(0,0) );
    	}

    	// populate the rest of the vertices, triangles
    	// use verticesFrequencyDepthCount to shift between frewuency collumns

    	// generate only one extra row for normals calculations
        for(int i = 1; i < 2; i++)
        {
        	for(int j = 0; j < verticesFrequencyDepthCount; j += 2)
        	{
        		// bottom left triangle
        		vertsList.Add( new Vector3(i * xScale,0, j * zScale) );
        		int currentListIndex = vertsList.Count -1;

	        	trisList.Add(currentListIndex);
	        	trisList.Add(currentListIndex - verticesFrequencyDepthCount);
	        	trisList.Add(currentListIndex - verticesFrequencyDepthCount + 1);

	        	// fill triangles in between this and previous triangle below
	        	if( j > 0) // is not at the edge
	        	{
	        		// bottom left triangle
	        		trisList.Add(currentListIndex -1);
		        	trisList.Add(currentListIndex - verticesFrequencyDepthCount -1);
		        	trisList.Add(currentListIndex - verticesFrequencyDepthCount);

		        	// top right triangle
		        	trisList.Add(currentListIndex);
		        	trisList.Add(currentListIndex - 1);
		        	trisList.Add(currentListIndex - verticesFrequencyDepthCount);
	        	}	

	        	// top right triangle
	        	vertsList.Add( new Vector3( i*xScale,0, (j + 1)*zScale ) );
	        	currentListIndex++;
	        	
	        	trisList.Add(currentListIndex);
	        	trisList.Add(currentListIndex - 1);
	        	trisList.Add(currentListIndex - verticesFrequencyDepthCount);
  	
	        	tempUVList.Add( new Vector2(0,0) );
	        	tempUVList.Add( new Vector2(0,0) );
        	}
        }

		calculationsMiniMesh.Clear();
		calculationsMiniMesh.MarkDynamic();
		calculationsMiniMesh.vertices = vertsList.ToArray();
		calculationsMiniMesh.uv = tempUVList.ToArray();
		calculationsMiniMesh.triangles = trisList.ToArray();
		calculationsMiniMesh.RecalculateNormals();

		miniVertsArray = calculationsMiniMesh.normals;

	}




}
