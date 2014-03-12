using UnityEngine;
using System.Collections;

public class CreaturePartsEmitter : MonoBehaviour 
{
	public GameObject partPrefab;

	AudioDirectorScript audioDirector;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(audioDirector.averageAmplitude > 2.0f)
		{
			for(int i = 0 ; i < audioDirector.decadesAveragesArray.Length; i += 1)
			{
				float decadeAverage = audioDirector.decadesAveragesArray[i];
				if( decadeAverage > 2.0f)
				{
					GameObject newPart = (GameObject)Instantiate(partPrefab, transform.position, Quaternion.identity);
					newPart.GetComponent<PVA>().velocity = 4.0f * decadeAverage * transform.forward;			
				}
			}
		}

	}


}
