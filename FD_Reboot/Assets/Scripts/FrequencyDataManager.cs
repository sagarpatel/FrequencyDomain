using UnityEngine;
using System.Collections;

public class FrequencyDataManager : MonoBehaviour 
{
	const int m_rawFFTDataSize = 1024;
	float[] m_currentRawFFTDataArray;

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
	}

	void Update()
	{
		m_liveAudioDataManager.m_liveAudioSource.GetSpectrumData(m_currentRawFFTDataArray,0, FFTWindow.BlackmanHarris);
	}

}
