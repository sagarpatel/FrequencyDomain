using UnityEngine;
using System.Collections;

public class CreaturePartsEmitter : MonoBehaviour 
{
	public GameObject partPrefab;

	public float emisionRadius = 0.0f;

	float minDecadeAmplitude = 5.0f;

	AudioDirectorScript audioDirector;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		for(int i = 0 ; i < audioDirector.decadesAveragesArray.Length; i += 1)
		{
			float decadeAverage = audioDirector.decadesAveragesArray[i];
			if( decadeAverage > minDecadeAmplitude)
			{
				GameObject newPart = (GameObject)Instantiate(partPrefab, GetEmissionPosition(i), Quaternion.identity);
				// set rotation
				newPart.transform.forward = transform.forward;
				newPart.transform.Rotate(newPart.transform.forward, GetEmissionRotationAngle(i), Space.World);
				// set scale
				float scaler = 1.0f + 4.0f *(decadeAverage - minDecadeAmplitude);
				newPart.transform.localScale = newPart.transform.localScale * scaler; //Mathf.Pow(scaler, 2.0f) ;		
				// set velocity
				SetEmissionDirection(newPart);
				newPart.GetComponent<PVA>().velocity = 1000.0f * newPart.transform.forward;
				// set Color
				newPart.renderer.material.color = audioDirector.calculatedRGB;
				
			}
		}
	

	}

	void SetEmissionDirection(GameObject partGO)
	{
		partGO.transform.Rotate(partGO.transform.right, -0.01f * Mathf.Pow( audioDirector.averageAmplitude, 2.0f ) , Space.Self);
	}

	Vector3 GetEmissionPosition(int decade)
	{
		Vector3 emissionPos = new Vector3(0, 0, 0);

		float angle = (float)decade/10.0f * Mathf.PI;
		emissionPos.x = 0.1f * Mathf.Pow( audioDirector.averageAmplitude, 3.0f ) * Mathf.Cos(angle);
		emissionPos.y = 0.1f * Mathf.Pow( audioDirector.averageAmplitude, 3.0f ) * Mathf.Sin(angle);

		// apply rotation of the emitter itself
		emissionPos = transform.rotation * emissionPos;
		// apply emitter position offset
		emissionPos += transform.position;

		return emissionPos;

	}

	float GetEmissionRotationAngle(int decade)
	{
		float rotationAngle;
		float angle = (float)decade/10.0f * Mathf.PI;
		rotationAngle = - 360.0f * Mathf.Cos(angle)/(2*Mathf.PI) ;
		return rotationAngle;
	}

}
