using UnityEngine;
using System.Collections;
using System.Linq;

public class BCIDataDirector : MonoBehaviour 
{

	BCIStreamingDataEmulator streamingDataEmulator;
	BCILiveDataManager liveDataManager;
	public float rawDataScaler = 1.0f;
	public float overallColorScaler = 1.0f;

	public float[] currentDataArary_Raw;
	public float[] currentDataArray;

	public bool isLiveData = false;

	void Awake()
	{
		streamingDataEmulator = FindObjectOfType<BCIStreamingDataEmulator>();
		liveDataManager = FindObjectOfType<BCILiveDataManager>();

		// TODO: maybe this should go into OnEnable() ?
		if(isLiveData == true)
		{
		
			currentDataArary_Raw = liveDataManager.fftarr;


		}
		else
		{
			currentDataArary_Raw = streamingDataEmulator.GetDataPointsArray();
		}

		currentDataArray = new float[currentDataArary_Raw.Length];


	}


	void Update()
	{
		if(isLiveData == true)
		{
			currentDataArary_Raw = liveDataManager.fftarr;
		}
		else
		{
			currentDataArary_Raw = streamingDataEmulator.GetDataPointsArray();
		}


		for(int i = 0; i< currentDataArray.Length; i += 2)
		{

			currentDataArray[i] = (currentDataArary_Raw[i/2] + currentDataArary_Raw[i/2 +1])/2.0f;
			currentDataArray[i+1] = currentDataArray[i];
		}

		for(int i = 0; i < currentDataArary_Raw.Length ; i++)
			currentDataArray[i] = rawDataScaler * currentDataArary_Raw[i];


	}


	public float GetCurrentAverageAmplitude()
	{
		float temp = 0;
		for(int i = 0 ; i < currentDataArray.Length ; i++)
			temp += currentDataArray[i];

		return temp/(float)currentDataArray.Length;
	}

	public Color GetCurrentRGB()
	{
		/// Hanlde RGB calculation
		
		// linear scaling model (triangular)

		float dataPointsCount = (float)currentDataArray.Length;
		int rCount = ((int)(0.40f * dataPointsCount)) - 1;
		int gCount = ((int)(0.60f * dataPointsCount)) - 1;
		int bCount = ((int)(0.40f * dataPointsCount)) - 1;
		int indexCounter = 0;
		int dataArrayHalfPointIndex = currentDataArray.Length/2;
		// Red (full), linear slope down, max 1.0
		// sine sweek test shows linear 1:1 dropoff is too weak, need to make slope less steep
		float tempR = 0;
		for( int i = 0; i < rCount; i++ )
		{
			tempR += currentDataArray[i] * Mathf.Clamp( ((float)rCount - 0.75f*(float)i)/(float)rCount ,0,1) ; // weighted according to position on slope
			indexCounter ++;
		}
		
		// Green (first half), linear slope up, max 2/3 --> 0.67 (fromarea under curve calcuation)
		// sine sweek test shows linear 1:1 dropoff is too weak, need to make slope less steep
		float tempG  = 0;
		int gHalfWidth = gCount/2;
		int gRiseStart = dataArrayHalfPointIndex - gHalfWidth;
		for( int i = gRiseStart ; i < gRiseStart + gHalfWidth ; i++ )
		{
			tempG += currentDataArray[i] * Mathf.Clamp( 0.6f * ( (1.6f*(float)i - (float)gRiseStart) )/(float)gHalfWidth ,0,1);
			indexCounter ++;
		}
		
		// Green (second half), linear slope down
		int gFallStart = dataArrayHalfPointIndex;
		for( int i = gFallStart ; i < gFallStart + gHalfWidth; i++ )
		{
			tempG += currentDataArray[i] * Mathf.Clamp( 0.6f * (gHalfWidth - 0.8f*(float)(i - dataArrayHalfPointIndex) )/(float)gHalfWidth ,0,1);
			indexCounter ++;
		}
		
		
		// Blue (full), linear slope up
		// blue is to powerful, going from 1.5f to 1.0f
		float tempB = 0;
		int bFallStart = currentDataArray.Length - 1 - bCount;
		for( int i = bFallStart; i < bFallStart + bCount; i++)
		{
			tempB += currentDataArray[i] * Mathf.Clamp( ( (1.0f*(float)i - (float)bFallStart) )/(float)bCount , 0,1);
			indexCounter ++;
		}
		
		
		Color tempColor = new Color( tempR , tempG , tempB, 1.0f);
		return overallColorScaler * tempColor;
	}

}
