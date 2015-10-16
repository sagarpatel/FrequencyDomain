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

	float m_catergoryHighlightChange_Cooldown = 0.25f;
	float m_categoryHighlightChange_TimeCounter = 0;


	void Start()
	{
		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();
		m_liveAudioDataManager = FindObjectOfType<LiveAudioDataManager>();
		m_riderPhysics = FindObjectOfType<RiderPhysics>();
	}

	void Update()
	{
		m_categoryHighlightChange_TimeCounter += Time.deltaTime;
	}

	// input should be -1 or 1
	public void HandleInput_UpDown(int input)
	{
		// move category highglighter up down
		if(m_isCatergorySelected == false)
		{
			if( m_categoryHighlightChange_TimeCounter > m_catergoryHighlightChange_Cooldown )
			{
				// bleck at top and bottom, don't wrap
				if(input == -1 && m_currentDebugCategory_Highlighted == DebugMenuCategory.FrequencyDataScaling)
					return;
				if(input == 1 && m_currentDebugCategory_Highlighted == DebugMenuCategory.ConductorParameters)
					return;

				input = -8 * input;
				m_currentDebugCategory_Highlighted = (DebugMenuCategory)(((int)m_currentDebugCategory_Highlighted + input) ) ; // TDDO: err... FIX THIS ??!?
				m_categoryHighlightChange_TimeCounter = 0;
			}

		}


	}


	public void HandleInput_LeftRight(int input)
	{

	}


}
