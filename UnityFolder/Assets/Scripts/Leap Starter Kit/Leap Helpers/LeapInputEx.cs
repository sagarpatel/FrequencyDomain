/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap;

/// <summary>
/// This static class serves as a static wrapper to provide some helpful C# functionality.
/// The main use is simply to provide the most recently grabbed frame as a singleton.
/// Events on aquiring, moving or loosing hands are also provided.  If you want to do any
/// global processing of data or input event dispatching, add the functionality here.
/// It also stores leap input settings such as how you want to interpret data.
/// To use it, you must call Update from your game's main loop.  It is not fully thread safe
/// so take care when using it in a multithreaded environment.
/// </summary>
public static class LeapInputEx
{	
	public static bool EnableTranslation = true;
	public static bool EnableRotation = true;
	public static bool EnableScaling = false;
	
	/// <summary>
	/// Event delegates are trigged every frame in the following order:
	/// Hand Found, Pointable Found, Hand Updated, Pointable Updated,
	/// Hand Lost, Hand Found.
	/// </summary>
	public static event Action<Pointable> PointableFound = delegate {};
	public static event Action<Pointable> PointableUpdated = delegate {};
	public static event Action<int> PointableLost = delegate {};
	
	public static event Action<Hand> HandFound = delegate {};
	public static event Action<Hand> HandUpdated = delegate {};
	public static event Action<int> HandLost = delegate {};
	
	public static event Action<GestureList> GestureDetected = delegate {};
	
	public static Leap.Frame Frame
	{
		get { return m_Frame; }
	} 

	public static Leap.Controller Controller
	{
		get { return m_controller; }
	}
	
	public static void Update() 
	{	
		if( m_controller != null )
		{			
			Frame lastFrame = m_Frame == null ? Frame.Invalid : m_Frame;
			
			frameStack.Clear();
			int i = 0;
			while (true)
			{
				Frame frame = m_controller.Frame(i);
				if (frame.Id == lastFrame.Id || !frame.IsValid || frame == Frame.Invalid)
				{
					break;
				}				
				frameStack.Push(frame);
				i++;
			}
							
			// Fix for frames that were being dropped with the standard implementation of LeapInput
			while (frameStack.Count > 0)
			{
				m_Frame = frameStack.Pop();
				DispatchLostEvents(Frame, lastFrame);
				DispatchFoundEvents(Frame, lastFrame);
				DispatchUpdatedEvents(Frame, lastFrame);
				DispatchGestureEvents(Frame);
				lastFrame = Frame;
			}
		}
	}
	
	//*********************************************************************
	// Private data & functions
	//*********************************************************************
	private enum HandID : int
	{
		Primary		= 0,
		Secondary	= 1
	};
	
	//Private variables
	static Leap.Controller 		m_controller	= new Leap.Controller();
	static Leap.Frame			m_Frame			= null;
	static Stack<Frame> 		frameStack 		= new Stack<Frame>();
		
	private static void DispatchLostEvents(Frame newFrame, Frame oldFrame)
	{
		foreach( Hand h in oldFrame.Hands )
		{
			if( !h.IsValid )
				continue;
			if( !newFrame.Hand(h.Id).IsValid)
				HandLost(h.Id);
		}
		foreach( Pointable p in oldFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( !newFrame.Pointable(p.Id).IsValid)
				PointableLost(p.Id);
		}
	}
	private static void DispatchFoundEvents(Frame newFrame, Frame oldFrame)
	{
		foreach( Hand h in newFrame.Hands )
		{
			if( !h.IsValid )
				continue;
			if( !oldFrame.Hand(h.Id).IsValid)
				HandFound(h);
		}
		foreach( Pointable p in newFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( !oldFrame.Pointable(p.Id).IsValid)
				PointableFound(p);
		}
	}
	private static void DispatchUpdatedEvents(Frame newFrame, Frame oldFrame)
	{
		foreach( Hand h in newFrame.Hands )
		{
			if( !h.IsValid )
				continue;
			if( oldFrame.Hand(h.Id).IsValid)
				HandUpdated(h);
		}
		foreach( Pointable p in newFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( oldFrame.Pointable(p.Id).IsValid)
				PointableUpdated(p);
		}
	}	
	private static void DispatchGestureEvents(Frame frame)
	{
		GestureList gestures = frame.Gestures();
		if (gestures.Count > 0)
			GestureDetected(gestures);
	}
}
