using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshLinesGenerator : MonoBehaviour 
{

	public float spawnCooldown = 1.0f;
	float spawnCooldownCounter = 0.0f;

	[Range (0,100)]
	public float meshSpeed;

	public Material meshMaterial;

	int dataRepCount = 1;

	AudioDirectorScript audioDirector;


	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
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
		GameObject newMeshLineGO = new GameObject();
		newMeshLineGO.AddComponent<MeshFilter>();
		newMeshLineGO.AddComponent<MeshRenderer>();

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
		

		//Debug.Log("MESH GENERATED");

		newMeshLineGO.AddComponent<PVA>();
		newMeshLineGO.GetComponent<PVA>().velocity = meshSpeed *transform.forward;

		newMeshLineGO.renderer.material = meshMaterial;
		
		newMeshLineGO.renderer.material.color = 3.0f * audioDirector.calculatedRGB;;
	}



}
