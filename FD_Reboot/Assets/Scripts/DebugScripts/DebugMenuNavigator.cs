using UnityEngine;
using System.Collections;

public class DebugMenuNavigator : MonoBehaviour 
{

	public enum DebugMenuCategory
	{
		FrequencyDataScaling,
		ColorScaling,
		FrequencyDataLayout,
		RiderParameters,
		ConductorParameters
	}

	DebugMenuCategory m_currentDebugCategory_Highlighted = DebugMenuCategory.FrequencyDataScaling;
	bool m_isCatergorySelected = false;

	FrequencyDataManager m_frequencyDataManager;
	LiveAudioDataManager m_liveAudioDataManager;
	RiderPhysics m_riderPhysics;



	void Start()
	{
		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();
		m_liveAudioDataManager = FindObjectOfType<LiveAudioDataManager>();
		m_riderPhysics = FindObjectOfType<RiderPhysics>();
	}

	// input should be -1 or 1
	public void HandleInput_UpDown(int input)
	{
		// move category highglighter up down
		if(m_isCatergorySelected == false)
		{
			m_currentDebugCategory_Highlighted = (DebugMenuCategory)(((int)m_currentDebugCategory_Highlighted + input) % 5) ; // TDDO: err... FIX THIS ??!?

		}


	}


	public void HandleInput_LeftRight(int input)
	{

	}


}
