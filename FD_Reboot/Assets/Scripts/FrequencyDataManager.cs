using UnityEngine;
using System.Collections;

public class FrequencyDataManager : MonoBehaviour 
{
	const int m_rawFFTDataSize = 8192;
	float[] m_currentRawFFTDataArray;

	const int m_processedFFTDataSize = 128; // separated into 8 sections
	float[] m_processedFFTDataArray;

	int[] samplesAccumulationArray = {1,2,5,9,12,25,55,175};
	int samplesAccumulationStartIndexOffset = 10;

	enum DataSources
	{
		Microphone,
		StereoMix,
	};

	DataSources m_dataSource;
	LiveAudioDataManager m_liveAudioDataManager;

	public AnimationCurve m_rValueCurve;
	public AnimationCurve m_gValueCurve;
	public AnimationCurve m_bValueCurve;

	public float d_r;
	public float d_g;
	public float d_b;
	public Color d_color;

	float m_rScaler = 1.0f;
	float m_gScaler = 1.0f;
	float m_bScaler = 1.0f;
	float m_colorScaler = 75.0f;

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
			m_processedFFTDataArray[i] = Mathf.Clamp( 1.0f * tempSum, 0 ,1); ///(float)currentRawSamplesPerProcessedPoint;
			//Debug.Log("Sum for acuumuator: " + currentRawSamplesPerProcessedPoint + " , " + tempSum);
		}
	}

	public Color GetFreshRGB()
	{
		int subdivisionIndex = 0;
		int subdivisionInterval = m_processedFFTDataArray.Length/8;

		// goes through subdivisions 0 - 3
		float r = 0;
		int r_SubdivisionStart = 0;
		int r_SubdivisionEnd = 3;
		float r_weight = 1.0f/((r_SubdivisionEnd - r_SubdivisionStart + 1) * (float)subdivisionInterval);

		// goes through subdivisions 3 - 6
		float g = 0;
		int g_SubdivisionStart = 3;
		int g_SubdivisionEnd = 6;
		float g_weight = 1.0f/((g_SubdivisionEnd - g_SubdivisionStart + 1) * (float)subdivisionInterval);

		// goes through subdivisions 4 - 7
		float b = 0;
		int b_SubdivisionStart = 4;
		int b_SubdivisionEnd = 7;
		float b_weight = 1.0f/((b_SubdivisionEnd - b_SubdivisionStart + 1) * (float)subdivisionInterval);

		float progressOnCurve = 0;
		for(int i = 0; i< m_processedFFTDataArray.Length; i++)
		{
			if(i % subdivisionInterval == 0 && i != 0)
				subdivisionIndex += 1;

			if(subdivisionIndex <= r_SubdivisionEnd)
			{
				progressOnCurve = Mathf.InverseLerp( subdivisionInterval * r_SubdivisionStart, subdivisionInterval * r_SubdivisionEnd , i );
				r += m_rValueCurve.Evaluate(progressOnCurve) * m_processedFFTDataArray[i] * r_weight;
			}

			if(subdivisionIndex >= g_SubdivisionStart && subdivisionIndex <= g_SubdivisionEnd)
			{
				progressOnCurve = Mathf.InverseLerp( subdivisionInterval * g_SubdivisionStart, subdivisionInterval * g_SubdivisionEnd, i);
				g += m_gValueCurve.Evaluate(progressOnCurve) * m_processedFFTDataArray[i] * g_weight;
			}

			if(subdivisionIndex >= b_SubdivisionStart)
			{
				progressOnCurve = Mathf.InverseLerp( subdivisionInterval * b_SubdivisionStart, subdivisionInterval * b_SubdivisionEnd, i);
				b += m_bValueCurve.Evaluate(progressOnCurve) * m_processedFFTDataArray[i] * b_weight;
			}
		}

		Color calculatedColor = m_colorScaler * new Color(r * m_rScaler, g * m_gScaler, b * m_bScaler);
		d_color = calculatedColor;
		d_r = calculatedColor.r;
		d_g = calculatedColor.g;
		d_b = calculatedColor.b;
		return calculatedColor;
	}

	public float[] GetFreshFFTData()
	{
		m_liveAudioDataManager.m_liveAudioSource.GetSpectrumData(m_currentRawFFTDataArray,0, FFTWindow.BlackmanHarris);
		ProcessRawFFTData();
		return m_processedFFTDataArray;
	}

}
