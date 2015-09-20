using UnityEngine;
using System.Collections;

public class DebugFFTSamplesCountController : MonoBehaviour 
{ 
	FrequencyDataManager m_frequencyDataManager;

	int m_currentlyControllingSectionIndex = 0; 
	int m_maxIndex = 7; // assuming total of 8

	void Start()
	{
		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			m_currentlyControllingSectionIndex = (int)Mathf.Clamp( (float)m_currentlyControllingSectionIndex - 1, 0, (float)m_maxIndex);
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			m_currentlyControllingSectionIndex = (int)Mathf.Clamp( (float)m_currentlyControllingSectionIndex + 1, 0, (float)m_maxIndex);
		} 

		if(Input.GetKey(KeyCode.UpArrow))
			m_frequencyDataManager.IncrementFrequencyRangeSamplesCount(m_currentlyControllingSectionIndex, 1);
		else if(Input.GetKey(KeyCode.DownArrow))
			m_frequencyDataManager.IncrementFrequencyRangeSamplesCount(m_currentlyControllingSectionIndex, -1);
	}
}
