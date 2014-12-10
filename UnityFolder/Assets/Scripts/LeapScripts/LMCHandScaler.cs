using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LMC_FingertipsStitch))]
public class LMCHandScaler : MonoBehaviour 
{
	bool isGUI = false;
	LMC_FingertipsStitch fingerTipStich;
	
	void Start () 
	{
		fingerTipStich = GetComponent<LMC_FingertipsStitch>();
	}
	
	
	void Update () 
	{
		float scaleIncrement = 0;
		isGUI = false;

		if(Input.GetKey( KeyCode.Equals ))
			scaleIncrement += 0.01f * Time.deltaTime;

		if(Input.GetKey( KeyCode.Minus ))
			scaleIncrement -= 0.01f * Time.deltaTime;

		if(scaleIncrement != 0)
			isGUI = true;

		float currentScale = fingerTipStich.posScale;
		currentScale += scaleIncrement;
		
		currentScale = Mathf.Clamp(currentScale, 0.001f, 10.0f);
		fingerTipStich.posScale = currentScale;
	}

	void OnGUI()
	{	
		if(isGUI)
		{		
			GUI.Label(new Rect(600,40,200,20),"LMC Hand Scaler:" + fingerTipStich.posScale);	
		}
		
	}

}
