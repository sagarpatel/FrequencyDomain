using UnityEngine;
using System.Collections;

public class ControlsInputHold : MonoBehaviour 
{
	public bool isControlsInputHold = false;
	

	void Update () 
	{

		if(Input.GetKeyDown(KeyCode.L))
			isControlsInputHold = !isControlsInputHold;
	
	}


}
