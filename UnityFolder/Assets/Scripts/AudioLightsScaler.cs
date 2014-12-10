using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioLightsScaler : MonoBehaviour 
{
	Light[] lightsArray;
	AudioDirectorScript audioDirector;
	float baseRange = 5.0f;


	public float test;

	void Start () 
	{
		lightsArray = GameObject.FindGameObjectsWithTag("MeshLight").Select(g => g.GetComponent<Light>()).ToArray();
		audioDirector = GetComponent<AudioDirectorScript>();
		print(audioDirector);
	}


	void Update () 
	{
		float currentAudioScale = audioDirector.averageAmplitude;
		float rangeValue = Mathf.Clamp( currentAudioScale, 10.0f, 1000.0f);
		for(int i = 0; i < lightsArray.Length; i++)
		{
			lightsArray[i].range = baseRange * currentAudioScale;
		}
	}


}
