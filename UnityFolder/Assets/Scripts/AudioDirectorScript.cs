using UnityEngine;
using System.Collections;

public class AudioDirectorScript : MonoBehaviour 
{

	//public float[] sampleArrayFreq = new float[1024];
	public float[] sampleArrayFreqBH = new float[1024];
/*
	public float[] colorArray =  new float[4];
	public float rgbAverage;
	public float rScale = 20.0f;
	public float gScale = 20.0f;
	public float bScale = 20.0f;
	public float aScale = 20.0f;
	*/
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

		//audioSourceArray[0].GetSpectrumData(sampleArrayFreq, 0, FFTWindow.Rectangular);
		audioSourceArray[0].GetSpectrumData(sampleArrayFreqBH, 0, FFTWindow.BlackmanHarris);//Rectangular);
/*
		float sum = 0;
		for(int c = 0; c<4; c++)
		{
			for(int i= c * 16; i < c*64 + 64; i++)
			{
				sum += sampleArrayFreqBH[i];
			}
			colorArray[c] = sum/16 ;

			sum = 0;
		}

		colorArray[0] *= rScale;
		colorArray[1] *= gScale;
		colorArray[2] *= bScale;
		colorArray[3] *= aScale;

		rgbAverage = (colorArray[0] + colorArray[1] + colorArray[2])/3.0f;

*/
	
	}





}
