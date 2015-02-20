using UnityEngine;
using System.Collections;

public class BCIAudioGenerator : MonoBehaviour 
{
	BCIDataDirector bciDataDirector;

	public float gain = 0.1f;

	void Awake()
	{

		bciDataDirector = FindObjectOfType<BCIDataDirector>();

	}

	void OnAudioFilterRead(float[] data, int channels)
	{


		float[] bciData = bciDataDirector.currentDataArray;
		int bciDataLength = bciData.Length;
		int audioDataLength_Effective = data.Length/channels;


		// assuming that bciDataLength is always smaller than data length
		int sampleRatio = audioDataLength_Effective/bciDataLength ;
		for(int i  = 0; i < data.Length ; i  = i + channels)
		{
			int lowerBCIIndex = i / sampleRatio;
			int upperBCIIndex = lowerBCIIndex + 1;

			float step = (float)(i % sampleRatio)/(float)sampleRatio;

			// clamp indices
			if( lowerBCIIndex >= bciDataLength )
				lowerBCIIndex = bciDataLength -1;
			if( upperBCIIndex >= bciDataLength )
				upperBCIIndex = bciDataLength - 1;

			float lerpedValue = Mathf.Lerp( bciData[lowerBCIIndex], bciData[upperBCIIndex], step);


			data[i] = gain * lerpedValue;

			// copy data to both channels if exist
			if(channels == 2)
			{
				data[i +1] = data[i];
			}
		}



	}


}
