using UnityEngine;
using System.Collections;

public class AudioDirectorScript : MonoBehaviour 
{

	//public float[] sampleArrayFreq = new float[1024];
	public float[] sampleArrayFreqBH = new float[1024];
	const int maxTrackCount = 1;
	public AudioSource[] audioSourceArray = new AudioSource[maxTrackCount];

	public float[] pseudoLogArray = new float[100];
	public int[] samplesPerDecadeArray = new int[10];

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
		// get raw FFT data 
		audioSourceArray[0].GetSpectrumData(sampleArrayFreqBH, 0, FFTWindow.BlackmanHarris);//Rectangular);

		// cleanup pseudolog array first
		for(int i = 0; i < pseudoLogArray.Length; i++)
			pseudoLogArray[i] = 0;
		
		// doing the pseudo log scale
		int decadeIndex = 0;
		int fftSampleCounter = 0;
		for(int i = 0; i < pseudoLogArray.Length; i++)
		{
			if( i != 0 && i%10 == 0)
				decadeIndex++;

			for(int j = 0; j < samplesPerDecadeArray[decadeIndex]; j++ )
			{
				pseudoLogArray[i] += sampleArrayFreqBH[fftSampleCounter];
				fftSampleCounter++;
			}
		}

		SpeadLocalMaxima();
	}

	void SpeadLocalMaxima()
	{
		float previousValue;
		float currentValue;
		float nextValue;
		for(int i = 3; i < pseudoLogArray.Length-3; i++)
		{	
			previousValue = pseudoLogArray[i-1];
			currentValue = pseudoLogArray[i];
			nextValue = pseudoLogArray[i+1];

			// find local maxima
			if( currentValue > previousValue && currentValue > nextValue)
			{
				// raise previous and next values
				pseudoLogArray[i-1] = ( previousValue + currentValue )/2.0f;
				pseudoLogArray[i+1] = ( nextValue + currentValue )/2.0f;
				// lower the maxima value
				pseudoLogArray[i] = (currentValue + previousValue)/1.5f;
			}

			// spead more
			if(previousValue > pseudoLogArray[i-2])
				pseudoLogArray[i-2] = (previousValue + pseudoLogArray[i-2])/2.0f;
			if(nextValue > pseudoLogArray[i+2])
				pseudoLogArray[i+2] = (nextValue + pseudoLogArray[i+2])/2.0f;

			// spead moaaar
			if(previousValue > pseudoLogArray[i-3])
				pseudoLogArray[i-3] = (pseudoLogArray[i-3] + pseudoLogArray[i-2])/2.0f;
			if(nextValue > pseudoLogArray[i+3])
				pseudoLogArray[i+3] = (pseudoLogArray[i+3] + pseudoLogArray[i+2])/2.0f;

		}

	}



}
