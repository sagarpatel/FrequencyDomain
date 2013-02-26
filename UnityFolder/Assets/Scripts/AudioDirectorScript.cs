using UnityEngine;
using System.Collections;

public class AudioDirectorScript : MonoBehaviour 
{

	//public float[] sampleArrayFreq = new float[1024];
	public float[] sampleArrayFreqBH = new float[1024];
	const int maxTrackCount = 1;
	public AudioSource[] audioSourceArray = new AudioSource[maxTrackCount];

	public float[] pseudoLogArray = new float[100];

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
		
		/// first attempts at pseudo log array, FFT at max resolution
		/// using pattern : 20+40+80+80+160+320+640+1280+2560+2560  = 7740
		int logIndex = 0;
		int loopCounter = 0;
		//cleanup pseudolog array first
		for(int i = 0; i < 100; i++)
			pseudoLogArray[i] = 0;
		
		for(int i = 0; i < 8000; i++)
		{
			if(i < 20)
				loopCounter = 2;
			else if(i < 60)
				loopCounter = 4;
			else if(i < 140)
				loopCounter = 8;
			else if(i < 220)
				loopCounter = 8;
			else if(i < 380)
				loopCounter = 16;
			else if(i < 700)
				loopCounter = 32;
			else if(i < 1340)
				loopCounter = 64;
			else if(i < 2620)
				loopCounter = 128;
			else if(i < 5180)
				loopCounter = 256;
			else if(i < 8000)
				loopCounter = 256;

			for(int j = 0; j < loopCounter; j++ )
			{
				pseudoLogArray[logIndex] += sampleArrayFreqBH[i];
				i++;
			}
			logIndex++;			
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
