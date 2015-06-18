using UnityEngine;
using System.Collections;

public class FrequencyDataManager : MonoBehaviour 
{
	const int m_rawFFTDataSize = 1024;
	float[] m_currentRawFFTDataArray;

	const int m_processedFFTDataSize = 128;
	float[] m_processedFFTDataArray;

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
		// debug versoin
		for(int i = 0; i < m_processedFFTDataArray.Length; i++)
		{
			m_processedFFTDataArray[i] = m_currentRawFFTDataArray[i];
		}
	}

	public float[] GetFreshFFTData()
	{
		m_liveAudioDataManager.m_liveAudioSource.GetSpectrumData(m_currentRawFFTDataArray,0, FFTWindow.BlackmanHarris);
		ProcessRawFFTData();
		return m_processedFFTDataArray;
	}

}
