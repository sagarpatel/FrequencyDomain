using UnityEngine;
using System.Collections;

public class DebugReset : MonoBehaviour 
{
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) == true)
		{
			Debug.Log("Reset!");
			Application.LoadLevel(Application.loadedLevel);
		}
	}

}
