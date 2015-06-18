using UnityEngine;
using System.Collections;

public class LiveAudioDataManager : MonoBehaviour 
{
	public AudioSource m_liveAudioSource;
	string[] m_audioDevicesArray;
	string m_currentAudioDeviceName;
	int m_currentAudioDeviceIndex = 0;

	int m_liveAudioSampleRate = 44100;
	int m_liveAudioClipLength = 1;
	float m_liveAudioAnalysisWindow = 0.2f;


	void Start()
	{
		m_liveAudioSource = gameObject.AddComponent<AudioSource>();
		m_liveAudioSource.loop = true;
		m_liveAudioSource.volume = 1.0f;
		m_liveAudioSource.mute = true;
		m_liveAudioSource.playOnAwake = false;

		m_audioDevicesArray = Microphone.devices;
		m_currentAudioDeviceIndex = 1;
		m_currentAudioDeviceName = m_audioDevicesArray[m_currentAudioDeviceIndex];

		// debug launch here
		HandleLiveAudioDeviceSwitch(m_currentAudioDeviceName);

	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			m_currentAudioDeviceIndex = (m_currentAudioDeviceIndex + 1) % Microphone.devices.Length;
			m_currentAudioDeviceName = Microphone.devices[m_currentAudioDeviceIndex];
			HandleLiveAudioDeviceSwitch(m_currentAudioDeviceName);
		}

	}

	// based on VJkit's VJMicrohpone.cs
	void HandleLiveAudioDeviceSwitch(string deviceName)
	{
		Debug.Log("New audio device: " + deviceName);

		// clean up old one
		m_liveAudioSource.Stop();
		m_liveAudioSource.clip = null;
		Microphone.End(deviceName);
		StopCoroutine(LaunchLiveAudioSource());

		// start new one
		int newDeviceFreq_Min = 0;
		int newDeviceFreq_Max = 0;
		Microphone.GetDeviceCaps(deviceName, out newDeviceFreq_Min, out newDeviceFreq_Max);
		if(newDeviceFreq_Min > 0 && newDeviceFreq_Max > 0)
			m_liveAudioSampleRate = Mathf.Clamp(m_liveAudioSampleRate, newDeviceFreq_Min, newDeviceFreq_Max);
		m_currentAudioDeviceName = deviceName;
		StartCoroutine(LaunchLiveAudioSource());

	}

	IEnumerator LaunchLiveAudioSource()
	{
		m_liveAudioSource.clip = Microphone.Start(m_currentAudioDeviceName, true, m_liveAudioClipLength, m_liveAudioSampleRate );
		m_liveAudioSource.Play();

		// http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
		float maxF = (8191.0f * ((float)m_liveAudioSampleRate)/2.0f) / 8192.0f;
		float[] liveAudioSamplesArray = new float[(int)(m_liveAudioAnalysisWindow * m_liveAudioSampleRate)];


		while(true)
		{
			int livePosition = Microphone.GetPosition(m_currentAudioDeviceName);
			//if(livePosition <  liveAudioSamplesArray.Length)
			//	livePosition += m_liveAudioClipLength * m_liveAudioSampleRate;

			m_liveAudioSource.clip.GetData(liveAudioSamplesArray, Microphone.GetPosition(m_currentAudioDeviceName));
			m_liveAudioSource.timeSamples = livePosition;

			yield return null;
		}


	}








}
