using UnityEngine;
using System.Collections;
using UnityOSC;
using System.Collections.Generic;
using System.Linq;

public class BCILiveDataManager : MonoBehaviour 
{

	public float[] fftarr = new float[128];

	void Awake()
	{
		OSCHandler.Instance.Init();

		OSCHandler.Instance.CreateServer("Muse", 5000);

		for(int i=0; i<fftarr.Length; i++)
		{
			fftarr[i]=0;
		}


		Dictionary<string, ServerLog> serversDict = new Dictionary<string, ServerLog>();
		serversDict = OSCHandler.Instance.Servers;


		foreach(string key in serversDict.Keys)
		{
			serversDict[key].server.PacketReceivedEvent += OSCReceicePacket;
		}



	}

	void Update()
	{
		if(OSCHandler.Instance.Servers.Count == 0)
			return;

		OSCHandler.Instance.UpdateLogs();

	}


	void OSCReceicePacket(OSCServer s, OSCPacket p)
	{


		if(p.Address.Equals("/muse/elements/raw_fft0"))
		{
			//Debug.Log("Receiving FFT!");
			//Debug.Log(test.Count);
			//fftqueue.Enqueue(packet.Data);
			for(int i=0; i< fftarr.Length; i++)
			{
				fftarr[i]=(float)p.Data[i];
				//Debug.Log(fftarr[i]);
			}
			
			
		}

	}

	


}
