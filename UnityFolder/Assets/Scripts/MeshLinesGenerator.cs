using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;

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

	// collumns stuff
	GameObject[] meshCollumnsArray;
	Mesh[] meshCollumnsMeshComponentArray;
	public int collumnDepth = 200;
	Vector3[] tempCollumnVerticesArray;
	Vector3[] tempCollumnNormalsArray;
	Vector3[] freshLineMeshNormalsArray;

	public Vector3[][] collumnsArrayVerticesArray;
	Vector3[][] collumnsArrayNormalsArray;

	const int collumnStitchIndex = 5;
	public Vector3 stitchAnchorOffset = new Vector3(0, 0, 0);
	GameObject stitchPosObject;

	// predeclared temp variables (trying to avoid GC)
	GameObject tempMeshLineGO;
	int freshMeshLineIndex;
	Mesh tempMesh;
	Vector3 currentLinesForward;
	Vector3 flatScale = new Vector3(1, 1, 1);

	public Color meshColorViewer;

	public bool isAmplitudeScale = true;
	public float minimumAmplitude = 1.0f;
	public float maximumAmplitude = 8.0f;
	public float staticAmpltiudeScale = 4.0f;

	LMC_FingertipsStitch fingertipStitch;
	int jointsPerFinger = 5;
	public Vector3[][] fingerJointsArrayStitchesPosArray;

	int currentMeshlineFetchIndex = 0;

	MeshLine[] meshLineDataArray;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		GenerateCalculationsMiniMesh();

		// mesh lines (i.e. rows) setup
		vertsArrayLast2 = new Vector3[2 * verticesFrequencyDepthCount];

		meshLinesPoolArray = new GameObject[meshLinesPoolSize];
		meshLinesMeshComponentArray = new Mesh[meshLinesPoolSize];
		meshLinesPVAComponentArray = new PVA[meshLinesPoolSize];
		meshLineDataArray = new MeshLine[meshLinesPoolSize];
		for(int i = 0; i < meshLinesPoolSize; i++)
		{
			meshLinesPoolArray[i] = (GameObject)Instantiate(meshLinePrefab, transform.position, Quaternion.identity);
			meshLinesPoolArray[i].name = meshLinesPoolArray[i].name + "_" + i.ToString();
			meshLinesPoolArray[i].GetComponentInChildren<MeshRenderer>().sharedMaterial = meshMaterial;
			meshLinesMeshComponentArray[i] = meshLinesPoolArray[i].GetComponentInChildren<MeshFilter>().mesh;
			meshLinesPVAComponentArray[i] = meshLinesPoolArray[i].GetComponent<PVA>();
			// TODO  no need for PVA, will probably remove entirely later
			meshLinesPVAComponentArray[i].enabled = false;
			// left/right offset to center mesh realtive to camera
			meshLinesPoolArray[i].transform.GetChild(0).transform.localPosition = new Vector3(0.5f  * verticesFrequencyDepthCount * verticesSpread, 0, 0);
			meshLineDataArray[i] =  meshLinesPoolArray[i].transform.GetComponent<MeshLine>();
			meshLineDataArray[i].meshlineVerticesArray = new Vector3[verticesFrequencyDepthCount];
			meshLinesPoolArray[i].SetActive(false);
		}

		// do basic setup for all meshe lines / rows
		verticesArray = new Vector3[verticesFrequencyDepthCount];
		freshLineMeshNormalsArray = new Vector3[verticesFrequencyDepthCount];
		for(int i = 0; i < verticesArray.Length; i++)
			verticesArray[i] = new Vector3(i * verticesSpread , 0, 0);

		List<int> indicesList = new List<int>();
		List<Vector2> uvsLinesList = new List<Vector2>();
		List<Vector4> tangentLinesList = new List<Vector4>();
		for(int i =0; i < verticesArray.Length - 1; i++)
		{
			indicesList.Add(i);
			indicesList.Add(i +1);
			uvsLinesList.Add(new Vector2(0, 0));
			tangentLinesList.Add(new Vector4(0, 0, 0, 0));
		}
		// add final uv
		uvsLinesList.Add(new Vector2(0,0));
		tangentLinesList.Add(new Vector4(0, 0, 0, 0));
		indicesArray = indicesList.ToArray();

		for(int i = 0; i < meshLinesPoolSize; i++)
		{
			meshLinesMeshComponentArray[i].Clear();
			meshLinesMeshComponentArray[i].vertices = verticesArray;
			meshLinesMeshComponentArray[i].uv = uvsLinesList.ToArray();
			meshLinesMeshComponentArray[i].tangents = tangentLinesList.ToArray();
			meshLinesMeshComponentArray[i].SetIndices(indicesArray, MeshTopology.Lines, 0);
		}

		// mesh collumns setup
		// basic object setup
		meshCollumnsArray = new GameObject[verticesFrequencyDepthCount];
		for(int i = 0; i < meshCollumnsArray.Length; i++)
		{
			meshCollumnsArray[i] = new GameObject("MeshCollumn_" + i);
			// parenting to make sure GO stays in view and does not get culled out when moving
			//meshCollumnsArray[i].transform.parent = gameObject.transform; 
			meshCollumnsArray[i].AddComponent<MeshFilter>();
			meshCollumnsArray[i].AddComponent<MeshRenderer>();
			meshCollumnsArray[i].renderer.sharedMaterial = meshMaterial;
			meshCollumnsArray[i].renderer.receiveShadows = false;
			meshCollumnsArray[i].renderer.castShadows = false;
		}
		
		// vertices and indices setup
		tempCollumnVerticesArray = new Vector3[collumnDepth];
		tempCollumnNormalsArray = new Vector3[collumnDepth];

		
		// Generate indices
		List<int> rowIndicesList = new List<int>();
		List<Vector2> uvsCollumnsList = new List<Vector2>();
		List<Vector4> tangentsCollumnsList = new List<Vector4>();
		for(int i = 0; i< tempCollumnVerticesArray.Length -1; i++)
		{
			rowIndicesList.Add(i);
			rowIndicesList.Add(i+1);
			uvsCollumnsList.Add(new Vector2(0, 0));
			tangentsCollumnsList.Add( new Vector4(0, 0, 0, 0));
		}
		// add final uv
		uvsCollumnsList.Add(new Vector2(0, 0));
		tangentsCollumnsList.Add(new Vector4(0, 0, 0, 0));

		// setup mesh component
		meshCollumnsMeshComponentArray = new Mesh[meshCollumnsArray.Length];

		// these 2D arrays will be used to locally store and manage vertices and normals, minimizing how often mesh.verties,etc gets called (which causes GC spike)
		collumnsArrayVerticesArray = new Vector3[meshCollumnsMeshComponentArray.Length][];
		collumnsArrayNormalsArray = new Vector3[meshCollumnsMeshComponentArray.Length][];

		Vector3[] emptyNormals = new Vector3[tempCollumnVerticesArray.Length];
		for(int i = 0; i < meshCollumnsMeshComponentArray.Length; i++)
		{
			meshCollumnsMeshComponentArray[i] = meshCollumnsArray[i].GetComponent<MeshFilter>().mesh;
			meshCollumnsMeshComponentArray[i].Clear();
			meshCollumnsMeshComponentArray[i].vertices = tempCollumnVerticesArray;
			meshCollumnsMeshComponentArray[i].SetIndices(rowIndicesList.ToArray(), MeshTopology.Lines,0);
			meshCollumnsMeshComponentArray[i].normals = emptyNormals;
			meshCollumnsMeshComponentArray[i].uv = uvsCollumnsList.ToArray();
			meshCollumnsMeshComponentArray[i].tangents = tangentsCollumnsList.ToArray();
			meshCollumnsMeshComponentArray[i].bounds = new Bounds(Vector3.zero, Vector3.one * 10000000.0f);

			collumnsArrayVerticesArray[i] = new Vector3[collumnDepth];
			collumnsArrayNormalsArray[i] =  new Vector3[collumnDepth];
			for(int j = 0; j < collumnDepth; j++)
			{
				collumnsArrayVerticesArray[i][j] = new Vector3(0, 0, 0);
				collumnsArrayNormalsArray[i][j] = new Vector3(0, 0, 0);	
			}
			
		}

		stitchPosObject = new GameObject();
		stitchPosObject.transform.parent = transform;
		stitchPosObject.transform.localPosition = stitchAnchorOffset;
		tempVector = new Vector3(0, 0, 0);

		fingertipStitch = GetComponent<LMC_FingertipsStitch>();
		//  array structured so that for [i][j], i represents the joint index (ex: fingertip row) and j represents the vertices
		// see https://developer.leapmotion.com/documentation/skeletal/csharp/devguide/Intro_Skeleton_API.html
		fingerJointsArrayStitchesPosArray = new Vector3[jointsPerFinger][];
		for (int i = 0; i < fingerJointsArrayStitchesPosArray.Length; i++)
		{
			fingerJointsArrayStitchesPosArray[i] = new Vector3[verticesFrequencyDepthCount];
		}
		//stitchOriginPosArray = new Vector3[verticesFrequencyDepthCount];

		//GenerateLineMesh();
		//StitchNewRowIntoCollumns()
	}
	
	// Update is called once per frame
	void Update () 
	{

		var inputDevice = InputManager.ActiveDevice;		
		if(inputDevice.Action2.WasPressed)
		{
			isAmplitudeScale = !isAmplitudeScale;
		}

		UpdateCollumnVerticesPosition();
	
		spawnCooldownCounter += Time.deltaTime;
		if(spawnCooldownCounter > spawnCooldown)
		{
			spawnCooldownCounter -= spawnCooldown;

			Profiler.BeginSample("GenerateLineMesh");
			GenerateLineMesh();
			Profiler.EndSample();


			Profiler.BeginSample("StitchNewRowIntoCollumns");
			StitchNewRowIntoCollumns();
			Profiler.EndSample();

		}

		meshMaterial.color = audioDirector.calculatedRGB;

		meshColorViewer = meshMaterial.color;

		/*
		if (Time.frameCount % 30 == 0)
		{
		   System.GC.Collect();
		}
		*/

	}

	public void CalculateClosestMeshLinePosition(Vector3 currenPos, Quaternion currentRot,float widthOffset ,out Vector3 calculatedPos, out Quaternion calculatedRot, out float calculatedHeightValue)
	{
		float previousDiff = 0;
		int previousIndex = currentMeshlineFetchIndex;
		int previousPreviousIndex = previousIndex;
		bool isFirstDistCheck = true;
		Quaternion lerpedRot;
		Vector3 lerpedPos;
		float lerpedHeight;

		for(int i = currentMeshlineFetchIndex + 1; i < meshLinesPoolArray.Length + currentMeshlineFetchIndex; i++)
		{
			int newIndex = i % meshLinesPoolArray.Length;
			if(meshLinesPoolArray[newIndex].activeSelf == true)
			{
				float currentDiff = Vector3.Distance(currenPos, meshLinesPoolArray[newIndex].transform.position);
				// skip first loop to ensure previousDiff gets a legit value
				if( isFirstDistCheck == false )
				{
					if(currentDiff >= previousDiff)
					{
						// dist to closest/ cetern ref line
						Transform centerLineTransform = meshLinesPoolArray[previousIndex].transform;
						Vector3 centerLinePos = centerLineTransform.position;
						float distBetweenClosest = Vector3.Distance(currenPos, centerLinePos);
						float centerHeight = meshLineDataArray[previousIndex].CalculateHeighOnLine(widthOffset);

						// dist to lower line
						Transform lowerLineTransform = meshLinesPoolArray[newIndex].transform;
						Vector3 lowerLinePos = lowerLineTransform.position;
						float distCurrent = Vector3.Distance(currenPos, lowerLinePos);
						float lowerHeight = meshLineDataArray[newIndex].CalculateHeighOnLine(widthOffset);

						// dist to upper line
						Transform upperLineTransform = meshLinesPoolArray[previousPreviousIndex].transform;
						Vector3 upperLinePos = upperLineTransform.position;
						float distPrePre = Vector3.Distance(currenPos, upperLinePos);
						float upperHeight = meshLineDataArray[previousPreviousIndex].CalculateHeighOnLine(widthOffset);

						float nanConst = 0.000001f;

						// don't allow the use of the freshes mesh line
						//if(previousIndex == currentMeshlineFetchIndex )
						//	continue;

						//Debug.Log("Front index: " + currentMeshlineFetchIndex +  " | Closest index: " + previousIndex);

						//Debug.Log("Dists");
						//Debug.Log(distBetweenClosest);
						//Debug.Log(distCurrent);
						//Debug.Log(distPrePre);


						// if between center line and lower line
						if(distCurrent < distPrePre)
						{
							float distBetweenCenterAndLower = nanConst + Vector3.Distance(lowerLinePos, centerLinePos);
							float step = distBetweenClosest/distBetweenCenterAndLower;

							lerpedPos = Vector3.Lerp( centerLineTransform.position, lowerLineTransform.position, step);
							lerpedRot = Quaternion.Slerp( centerLineTransform.rotation, lowerLineTransform.rotation,step);
							lerpedHeight =  Mathf.Lerp(centerHeight, lowerHeight, step);
							//Debug.Log("Step: " + step);

							calculatedPos = lerpedPos;
							calculatedRot = lerpedRot;
							calculatedHeightValue = lerpedHeight;
							return;
						}
						else
						{
							float distBetweenCenterAndUpper = nanConst + Vector3.Distance(upperLinePos, centerLinePos);
							float step = distBetweenClosest/distBetweenCenterAndUpper;

							lerpedPos = Vector3.Lerp( centerLineTransform.position, upperLineTransform.position, step);
							lerpedRot = Quaternion.Slerp( centerLineTransform.rotation, upperLineTransform.rotation,step);
							lerpedHeight = Mathf.Lerp(centerHeight, upperHeight, step);
							//Debug.Log("Step: " + step);

							calculatedPos = lerpedPos;
							calculatedRot = lerpedRot;
							calculatedHeightValue = lerpedHeight;
							return;
						}

					}
				}
				else
					isFirstDistCheck = false;

				previousDiff = currentDiff;
				previousPreviousIndex = previousIndex;
				previousIndex = newIndex;
			}
		}

		// if reached here, failed
		Debug.LogWarning("FAILED TO GET POS/ROT, sticking in front");
		Debug.Log("Front index: " + currentMeshlineFetchIndex +  " | Closest index: " + previousIndex);
		Transform frontTransform = meshLinesPoolArray[currentMeshlineFetchIndex].transform;

		calculatedPos = frontTransform.position; //currenPos; //Vector3.zero;
		calculatedRot = frontTransform.rotation; //currentRot; //Quaternion.identity;
		calculatedHeightValue = 0;

	}

	int GetFreeMeshLineIndex()
	{
		for(int i = currentMeshlineFetchIndex; i < meshLinesPoolArray.Length + currentMeshlineFetchIndex; i++)
		{
			int newIndex = i % meshLinesPoolArray.Length;
			if(meshLinesPoolArray[newIndex].activeSelf == false)
			{
				currentMeshlineFetchIndex = newIndex;
				return newIndex;
			}
		}
		// if nothing found
			return -1;
	}

	void GenerateLineMesh()
	{
		freshMeshLineIndex = GetFreeMeshLineIndex();
		
		if(freshMeshLineIndex == -1)
			return;
		else
		{
			tempMeshLineGO = meshLinesPoolArray[freshMeshLineIndex];
			tempMeshLineGO.SetActive(true);

			tempMeshLineGO.transform.rotation = transform.rotation;

			float amplitudeScale;
			if(isAmplitudeScale)
			{
				amplitudeScale = Mathf.Clamp(audioDirector.averageAmplitude, minimumAmplitude, maximumAmplitude) ;

			}
			else
			{
				amplitudeScale = staticAmpltiudeScale;
			}
		
			tempMeshLineGO.transform.localScale = 0.03f * amplitudeScale * flatScale;

			tempMeshLineGO.transform.position = transform.position;
			//tempMeshLineGO.GetComponent<MeshLine>().UpdateCenter(verticesFrequencyDepthCount, verticesSpread);

			currentLinesForward = tempMeshLineGO.transform.forward;

		}

		tempMesh = meshLinesMeshComponentArray[freshMeshLineIndex];

		// SET HEIGHT

		for(int i = 0; i<verticesFrequencyDepthCount; i++)
		{
			tempVector = verticesArray[ verticesFrequencyDepthCount - i -1];
			tempVector.y = 16.0f * audioDirector.pseudoLogArrayBuffer[i/(dataRepCount+1)]; //* verticesAudioHeightScale * yScale; // normal version
			//tempVector.y = ( tempHeight * verticesAudioHeightScale + verticesArray[i + verticesFrequencyDepthCount].y)/2.0f ; // time axis smoothing version
			verticesArray[verticesFrequencyDepthCount - i -1] = tempVector;
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
		for(int i = 0 ; i < verticesArray.Length; i++)
		{
			meshLineDataArray[freshMeshLineIndex].meshlineVerticesArray[i] = verticesArray[i];
		}

		//meshLineDataArray[freshMeshLineIndex].meshlineVerticesArray = verticesArray;// this jsut passes a refrence to the array, we need a copy 
		
		// looks like copying values from one array to another causes GC to go wilde spikes >_<
		// Take() is much better than manual copy though
		Profiler.BeginSample("Mesh take");
		freshLineMeshNormalsArray = calculationsMiniMesh.normals.Take(verticesFrequencyDepthCount).ToArray();
		tempMesh.normals = freshLineMeshNormalsArray; //calculationsMiniMesh.normals.Take(verticesFrequencyDepthCount).ToArray();
		Profiler.EndSample();

		// TODO  : no need for pva, will probabbly remove entierly later
		//meshLinesPVAComponentArray[freshMeshLineIndex].ResetPVA();
		//meshLinesPVAComponentArray[freshMeshLineIndex].velocity = meshSpeed * transform.forward;

		
	}

	void StitchNewRowIntoCollumns()
	{

		// first push down the vertices by 1 index for all collumns

		for(int h = 0; h < meshCollumnsArray.Length; h++)
		{
			// shift values down
			for(int i = collumnDepth -1 ; i > collumnStitchIndex ; i--)
			{
				collumnsArrayVerticesArray[h][i] = collumnsArrayVerticesArray[h][i-1];
				collumnsArrayNormalsArray[h][i] = collumnsArrayNormalsArray[h][i-1];
			}
			
			// add the new row value to all corresponding collumn start vertex
			collumnsArrayVerticesArray[h][collumnStitchIndex] = tempMeshLineGO.transform.GetChild(0).transform.TransformPoint(verticesArray[h]); //- transform.position;
			collumnsArrayNormalsArray[h][collumnStitchIndex] = freshLineMeshNormalsArray[h]; //meshLinesMeshComponentArray[freshMeshLineIndex].normals[h];

			meshCollumnsMeshComponentArray[h].vertices = collumnsArrayVerticesArray[h]; //tempCollumnVerticesArray;
			meshCollumnsMeshComponentArray[h].normals = collumnsArrayNormalsArray[h]; // tempCollumnNormalsArray;
		}

	}

	void UpdateCollumnVerticesPosition()
	{
		Profiler.BeginSample("Update Collumn Vertices");
		Vector3 forwardVec = transform.forward;
		float deltaT = Time.deltaTime;
		Vector3 posIncrement =  meshSpeed * forwardVec * deltaT;
		Vector3 tempPos = Vector3.zero;

		Vector3 anchorLocalPos = stitchAnchorOffset * audioDirector.averageAmplitude;

		for(int h = 0; h < meshCollumnsArray.Length; h++)
		{	
			for(int i = 0 ; i < collumnDepth ; i++)
			{
				// handle origin stitches/vertices
				if (i < collumnStitchIndex)
				{
					// leap motion finger stitching 
					if (fingertipStitch.isValidData == true)
					{
						int reversedJointsOrderIndex = (jointsPerFinger - i) - 1;
						collumnsArrayVerticesArray[h][i] = fingerJointsArrayStitchesPosArray[reversedJointsOrderIndex][h];
					}
					else
					{
						// default to common origin stitch point
						// old/normal way, unified origin point
						stitchPosObject.transform.localPosition = anchorLocalPos;
						collumnsArrayVerticesArray[h][i] = stitchPosObject.transform.position; //stitchAnchorOffset + tempPosition;
					}
				}
				else // oridinary vertices
				{
					//// not unified physics now, could cause trouble later`
					// doing in line for perf as seen in --> https://www.youtube.com/watch?v=WE3PWHLGsX4
					
					tempPos = collumnsArrayVerticesArray[h][i];
					tempPos.x += posIncrement.x;
					tempPos.y += posIncrement.y;
					tempPos.z += posIncrement.z;
					collumnsArrayVerticesArray[h][i] = tempPos;
				}
			}

			meshCollumnsMeshComponentArray[h].vertices = collumnsArrayVerticesArray[h];
		}
		Profiler.EndSample();
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
