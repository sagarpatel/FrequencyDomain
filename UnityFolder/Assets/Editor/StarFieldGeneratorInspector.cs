using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StarFieldGenerator))]
public class StarFieldGeneratorInspector : Editor 
{

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        StarFieldGenerator myScript = (StarFieldGenerator)target;
        if(GUILayout.Button("Generate Star Field"))
        {
            myScript.GenerateStarField();
        }

        if(GUILayout.Button("Delete All Stars"))
        {
        	myScript.DeleteAllStarsInScene();
        }
    }
}
