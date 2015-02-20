using UnityEngine;
using System.Collections;
using System.Linq;

public class BCIStreamingDataEmulator : MonoBehaviour 
{
	public TextAsset csvDataFile;
	float[][] dataPointsGridArray;

	float dataRefreshInterval = 0.1f; // 0.1 -> 100 ms
	float timeCounter;

	int currentDataLineIndex = 0;
	int dataLinesCount = 0;

	void Awake()
	{
		string[] linesArray = csvDataFile.text.Split('\n');
		// removed last null line (not checking, just blindling removing last line for now
		linesArray = linesArray.Take(linesArray.Length - 1).ToArray();
		int dataPointsPerLine = linesArray[0].Split(',').Length;
		
		Debug.Log("data points per line: " + dataPointsPerLine);
		
		dataPointsGridArray = new float[linesArray.Length][];
		for(int i = 0; i < linesArray.Length ; i++)
		{
			dataPointsGridArray[i] = new float[dataPointsPerLine];
			string[] splitLine = linesArray[i].Split(',');
			for(int j = 0; j< dataPointsPerLine ; j++)
			{
				dataPointsGridArray[i][j] = float.Parse( splitLine[j] );
			}
		}

		dataLinesCount = linesArray.Length;
	}


	void Update()
	{

		if( timeCounter > dataRefreshInterval )
		{
			timeCounter -= dataRefreshInterval;

			currentDataLineIndex ++;
			if(currentDataLineIndex > dataLinesCount -1 )
				currentDataLineIndex = 0;
		}

		timeCounter += Time.deltaTime;
	}

	public float[] GetDataPointsArray()
	{
		return dataPointsGridArray[currentDataLineIndex];
	}



}
