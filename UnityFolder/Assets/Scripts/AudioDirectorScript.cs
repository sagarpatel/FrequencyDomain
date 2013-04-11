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
	public float[] scalingPerDecadeArray = new float[10];

	public float[] pseudoLogArrayBuffer = new float[100];

	public float rScale = 1.0f;
	public float bScale = 1.0f;
	public float gScale = 1.0f;
	public Color calculatedRGB = new Color();

	// Low Pass Filter Parameters
	public float initialLPFCutoffFrequency = 20000;
	float initialFOV;
	AudioLowPassFilter lowPassFilter;
	Camera mainCamera;

	// Use this for initialization
	void Start () 
	{
		//Start music
		for(int i = 0; i < maxTrackCount ; i++)
			audioSourceArray[i].Play();

		// Initialize array to 1 if not set in editor
		for(int i = 0; i < scalingPerDecadeArray.Length; i++)
			if(scalingPerDecadeArray[i] == 0)
				scalingPerDecadeArray[i] = 1.0f;
	
		lowPassFilter = (AudioLowPassFilter)GetComponent("AudioLowPassFilter");
		mainCamera = (Camera)GameObject.Find("Main Camera").GetComponent("Camera");
		initialFOV = ( (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript") ).originalFieldOfView;
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
				pseudoLogArray[i] += sampleArrayFreqBH[fftSampleCounter] * scalingPerDecadeArray[decadeIndex];
				fftSampleCounter++;
			}
		}

		SpeadLocalMaxima();

		CalculateRBG();

		HandleLowPassFilter(); // does not affect the landscape


		//update buffer
		for(int i = 0; i < pseudoLogArray.Length; i++)
			pseudoLogArrayBuffer[i] += pseudoLogArray[i];



	}


//// Functions

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


	void CalculateRBG()
	{
		/// Hanlde RGB calculation

		// linear scaling model (triangular)
		// assuming a pseudo log array of size 100, cuz I'm lazy. It's easy to generalize, but don't feel like doing it now
		
		// Red (full), linear slope down, max 1.0
		float tempR = 0;
		for( int i = 0; i < 40; i++ )
		{
			tempR += pseudoLogArray[i] * (40.0f - (float)i)/40.0f ; // weighted according to position on slope
		}
	
		// Green (first half), linear slope up, max 2/3 --> 0.67 (fromarea under curve calcuation)
		float tempG  = 0;
		for( int i = 20; i < 50; i++ )
		{
			tempG += pseudoLogArray[i] * 0.67f * ((float)(i - 20))/30.0f;
		}

		// Green (second half), linear slope down
		for( int i = 50; i < 80; i++ )
		{
			tempG += pseudoLogArray[i] * 0.67f * (30.0f - (float)(i-50))/30.0f;
		}


		// Blue (full), linear slope up
		float tempB = 0;
		for( int i = 60; i < 100; i++)
		{
			tempB += pseudoLogArray[i] * ((float)(i - 60))/40.0f;
		}
	

		calculatedRGB = new Color( tempR * rScale, tempG * gScale, tempB * bScale, 1.0f);
	}


	void HandleLowPassFilter()
	{
		float currentFOV = mainCamera.fieldOfView;
		float progressRatio = ( currentFOV - initialFOV )/(180.0f - initialFOV);
		lowPassFilter.cutoffFrequency = initialLPFCutoffFrequency * (1.0f - progressRatio);

	}


}
