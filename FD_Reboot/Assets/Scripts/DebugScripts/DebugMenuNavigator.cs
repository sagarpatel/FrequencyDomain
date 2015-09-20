using UnityEngine;
using System.Collections;

public class DebugMenuNavigator : MonoBehaviour 
{

	public enum DebugTypes
	{
		FrequencyDataScaling,
		ColorScaling,
		FrequencyDataLayout,
		RiderParameters,
		ConductorParameters
	}

	public DebugTypes m_currentDebugType = DebugTypes.FrequencyDataScaling;

	FrequencyDataManager m_frequencyDataManager;
	LiveAudioDataManager m_liveAudioDataManager;
	RiderPhysics m_riderPhysics;

	void Start()
	{
		m_frequencyDataManager = FindObjectOfType<FrequencyDataManager>();
		m_liveAudioDataManager = FindObjectOfType<LiveAudioDataManager>();
		m_riderPhysics = FindObjectOfType<RiderPhysics>();
	}

	public void ChangeDebugMenuType(bool menuMoveDirection)
	{
		int change = 0;
		if(menuMoveDirection == true)
			change = 1;
		else
			change = -1;

		m_currentDebugType = (DebugTypes)((int)m_currentDebugType + change); // TDDO: err... FIX THIS ??!?

	}



}
