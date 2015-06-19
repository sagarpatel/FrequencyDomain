using UnityEngine;
using System.Collections;

public class FrequencyDataManager : MonoBehaviour 
{
	const int m_rawFFTDataSize = 8192;
	float[] m_currentRawFFTDataArray;

	const int m_processedFFTDataSize = 128; // separated into 8 sections
	float[] m_processedFFTDataArray;

	int[] samplesAccumulationArray = {2,4,5,8,10,20,41,200};
	int samplesAccumulationStartIndexOffset = 10;

	enum DataSources
	{
		Microphone,
		StereoMix,
	};

	DataSources m_dataSource;
	LiveAudioDataManager m_liveAudioDataManager;

	void Start()
	{
		m_currentRawFFTDataArray = new float[m_rawFFTDataSize];
		m_dataSource = DataSources.Microphone;
		m_liveAudioDataManager = FindObjectOfType<LiveAudioDataManager>();

		m_processedFFTDataArray = new float[m_processedFFTDataSize];
	}

	void ProcessRawFFTData()
	{
		int samplesSum = 0 + samplesAccumulationStartIndexOffset;
		int accumulationIndex = 0;
		int accumulationInterval = m_processedFFTDataArray.Length/8;
		for(int i = 0; i< m_processedFFTDataArray.Length; i++)
		{
			if(i % accumulationInterval == 0 && i != 0)
				accumulationIndex += 1;

			samplesSum += samplesAccumulationArray[accumulationIndex];
		}
		if(samplesSum > m_currentRawFFTDataArray.Length)
			Debug.LogError("Sum of samples is too large: " + samplesSum + " , max is: " + m_currentRawFFTDataArray.Length);



		accumulationIndex = 0;
		accumulationInterval = m_processedFFTDataArray.Length/8;
		float tempSum = 0;
		int rawFFTIndex = 0;
		int currentRawSamplesPerProcessedPoint = 0;
		for(int i = 0; i < m_processedFFTDataArray.Length; i++)
		{
			if(i % accumulationInterval == 0 && i != 0)
				accumulationIndex += 1;

			tempSum = 0;
			currentRawSamplesPerProcessedPoint = samplesAccumulationArray[accumulationIndex];
			for(int j = 0; j < currentRawSamplesPerProcessedPoint; j++)
			{
				tempSum += m_currentRawFFTDataArray[rawFFTIndex + samplesAccumulationStartIndexOffset];
				rawFFTIndex += 1;
			}
			m_processedFFTDataArray[i] = tempSum;///(float)currentRawSamplesPerProcessedPoint;
			//Debug.Log("Sum for acuumuator: " + currentRawSamplesPerProcessedPoint + " , " + tempSum);
		}
	}

	public float[] GetFreshFFTData()
	{
		m_liveAudioDataManager.m_liveAudioSource.GetSpectrumData(m_currentRawFFTDataArray,0, FFTWindow.BlackmanHarris);
		ProcessRawFFTData();
		return m_processedFFTDataArray;
	}

}
