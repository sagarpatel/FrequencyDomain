using System;
using UnityEngine;


namespace InControl
{
	public class UnityMouseAxisSource : InputControlSource
	{
		public string MouseAxisQuery;


		public UnityMouseAxisSource()
		{
		}


		public UnityMouseAxisSource( string axis )
		{
			MouseAxisQuery = "mouse " + axis;
		}
		
		
		public float GetValue( InputDevice inputDevice )
		{
			return Input.GetAxisRaw( MouseAxisQuery );
		}
		
		
		public bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}
	}
}

