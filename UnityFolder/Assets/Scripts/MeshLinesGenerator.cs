using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeshLinesGenerator : MonoBehaviour 
{
	public GameObject meshLinePrefab;
	GameObject[] meshLinesPoolArray;
	Mesh[] meshLinesMeshComponentArray;
	PVA[] meshLinesPVAComponentArray;
	public int meshLinesPoolSize = 100;

	public float spawnCooldown = 1.0f;
	float spawnCooldownCounter = 0.0f;

	[Range (0,100)]
	public float meshSpeed;

	public Material meshMaterial;

	int dataRepCount = 1;

	AudioDirectorScript audioDirector;

	public int verticesFrequencyDepthCount = 200;
	public float verticesSpread = 1.0f;
	Mesh calculationsMiniMesh;
	Vector3[] miniVertsArray;
	Vector3[] vertsArrayLast2;

	Vector3[] verticesArray;
	int[] indicesArray;
	Vector3 tempVector;

	float xScale = 1.0f;
	float zScale = 1.0f;

	// predeclared temp variables (trying to avoid GC)
	GameObject tempMeshLineGO;
	int tempMeshLineIndex;
	Mesh tempMesh;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		GenerateCalculationsMiniMesh();

		vertsArrayLast2 = new Vector3[2 * verticesFrequencyDepthCount];

		meshLinesPoolArray = new GameObject[meshLinesPoolSize];
		meshLinesMeshComponentArray = new Mesh[meshLinesPoolSize];
		meshLinesPVAComponentArray = new PVA[meshLinesPoolSize];
		for(int i = 0; i < meshLinesPoolSize; i++)
		{
			meshLinesPoolArray[i] = (GameObject)Instantiate(meshLinePrefab, transform.position, Quaternion.identity);
			meshLinesPoolArray[i].renderer.sharedMaterial = meshMaterial;
			meshLinesPoolArray[i].SetActive(false);

			meshLinesMeshComponentArray[i] = meshLinesPoolArray[i].GetComponent<MeshFilter>().mesh;
			meshLinesPVAComponentArray[i] = meshLinesPoolArray[i].GetComponent<PVA>();
		}
		


		// do basic setup for all meshes
		verticesArray = new Vector3[verticesFrequencyDepthCount];
		for(int i = 0; i < verticesArray.Length; i++)
		{
			verticesArray[i] = new Vector3(i * verticesSpread , 0, 0);
		}

		
		List<int> indicesList = new List<int>();
		for(int i =0; i < verticesArray.Length - 1; i++)
		{
			indicesList.Add(i);
			indicesList.Add(i +1);
		}
		indicesArray = indicesList.ToArray();


		for(int i = 0; i < meshLinesPoolSize; i++)
		{
			meshLinesMeshComponentArray[i].Clear();
			meshLinesMeshComponentArray[i].vertices = verticesArray;
			meshLinesMeshComponentArray[i].SetIndices(indicesArray, MeshTopology.Lines, 0);
		}


		tempVector = new Vector3(0, 0, 0);
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
		meshMaterial.color = audioDirector.calculatedRGB;


		/*
		if (Time.frameCount % 30 == 0)
		{
		   System.GC.Collect();
		}
		*/

	}

	int GetFreeMeshLineIndex()
	{
		for(int i = 0; i < meshLinesPoolArray.Length; i++)
		{
			if(meshLinesPoolArray[i].activeSelf == false)
				return i;
		}
		// if nothing found
			return -1;
	}

	void GenerateLineMesh()
	{
		tempMeshLineIndex = GetFreeMeshLineIndex();
		
		if(tempMeshLineIndex == -1)
			return;
		else
		{
			tempMeshLineGO = meshLinesPoolArray[tempMeshLineIndex];
			tempMeshLineGO.SetActive(true);
		
			tempMeshLineGO.transform.localScale = 0.05f * audioDirector.averageAmplitude * new Vector3(1, 1, 1);

			tempMeshLineGO.transform.position = transform.position;
			float xOffset = - 0.5f * tempMeshLineGO.transform.localScale.x * verticesFrequencyDepthCount * verticesSpread;
			tempVector = tempMeshLineGO.transform.position;
			tempVector.x += xOffset;
			tempMeshLineGO.transform.position = tempVector;
		}

		tempMesh = meshLinesMeshComponentArray[tempMeshLineIndex];

		// SET HEIGHT

		for(int i = 1; i<verticesFrequencyDepthCount; i++)
		{
			tempVector = verticesArray[i];
			tempVector.y = 4.0f * audioDirector.pseudoLogArrayBuffer[i/(dataRepCount+1)]; //* verticesAudioHeightScale * yScale; // normal version
			//tempVector.y = ( tempHeight * verticesAudioHeightScale + verticesArray[i + verticesFrequencyDepthCount].y)/2.0f ; // time axis smoothing version
			verticesArray[i] = tempVector;
		}

		// reset the audio data buffer
		for(int i = 0; i < audioDirector.pseudoLogArray.Length; i++)
			audioDirector.pseudoLogArrayBuffer[i] = 0;

			
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
		
		// apply data to mesh
		tempMesh.vertices = verticesArray;
		
		// looks like copying values from one array to another causes GC to go wilde spikes >_<
		// Take() is much better than manual copy though
		tempMesh.normals = calculationsMiniMesh.normals.Take(verticesFrequencyDepthCount).ToArray();

		meshLinesPVAComponentArray[tempMeshLineIndex].ResetPVA();
		meshLinesPVAComponentArray[tempMeshLineIndex].velocity = meshSpeed *transform.forward;
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
