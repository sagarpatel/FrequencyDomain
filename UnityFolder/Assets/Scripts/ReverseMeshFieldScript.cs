using UnityEngine;
using System.Collections;

public class ReverseMeshFieldScript : MonoBehaviour 
{
	Mesh mesh;
	MeshFieldGeneratorScript meshFieldGeneratorScript;
	AudioDirectorScript audioDirector;

	bool isInit = false;


	// Use this for initialization
	void Start () 
	{
		gameObject.AddComponent("MeshFilter");
        gameObject.AddComponent("MeshRenderer");

        mesh = GetComponent<MeshFilter>().mesh;
        meshFieldGeneratorScript = (MeshFieldGeneratorScript)GameObject.Find("MainMeshField").GetComponent("MeshFieldGeneratorScript");

        audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(isInit == false)
		{
			isInit = true;
			mesh.Clear();
			mesh.MarkDynamic();
			mesh.vertices = meshFieldGeneratorScript.verticesArray;
			mesh.uv = meshFieldGeneratorScript.uvArray;
			mesh.triangles = meshFieldGeneratorScript.trianglesArray;
			//mesh.RecalculateNormals();

			GetComponent<MeshRenderer>().materials[0].color = Color.green;
			renderer.material.shader = Shader.Find("Parallax Diffuse");
		}
		else
		{
			mesh.vertices = meshFieldGeneratorScript.verticesArray;
			mesh.normals = meshFieldGeneratorScript.mesh.normals;
			GetComponent<MeshRenderer>().materials[0].color = audioDirector.calculatedRGB;

		}

	}
}
