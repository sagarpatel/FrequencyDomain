using UnityEngine;
using System.Collections;

public class AudioDirectorScript : MonoBehaviour 
{

	//public float[] sampleArrayFreq = new float[1024];
	public float[] sampleArrayFreqBH = new float[1024];
	const int maxTrackCount = 1;
	public AudioSource[] audioSourceArray = new AudioSource[maxTrackCount];

	// Use this for initialization
	void Start () 
	{

		for(int i = 0; i < maxTrackCount ; i++)
		{
			audioSourceArray[i].Play();
			//audioSourceArray[i].volume = 0;
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//audioSourceArray[0].GetSpectrumData(sampleArrayFreqBH, 0, FFTWindow.Blackman);
		audioSourceArray[0].GetSpectrumData(sampleArrayFreqBH, 0, FFTWindow.BlackmanHarris);//Rectangular);

	
	}



}
