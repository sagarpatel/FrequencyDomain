using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// gets updates from leap api
/// assigns leap hand data to unity hand
/// </summary>
public class LeapHandController : MonoBehaviour 
{
	public UnityHand[] unityHands;
	public UnityHandSettings handSettings;

	private float timeVisible = 0.2f;
					
	void Start () 
	{
		//attach controller methods to Leap's hand updates
		LeapInputEx.HandUpdated += OnHandUpdated;
		LeapInputEx.HandFound += OnHandFound;
		LeapInputEx.HandLost += OnHandLost;
		
		//enable gestures
		LeapInputEx.Controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
		LeapInputEx.Controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		LeapInputEx.Controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
		LeapInputEx.Controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);

		unityHands[0].AssignSettings(handSettings);
		unityHands[1].AssignSettings(handSettings);
	}

	void Update()
	{
		//process the Leap message pump
		LeapInputEx.Update();
	}
	
	private void OnHandFound(Hand h)
	{
		Messenger.Broadcast<int>(SIG.HANDFOUND.ToString(), h.Id); //broadcast new hand ID to registered listeners
	}
    	  	
	private void OnHandUpdated(Hand h)
	{
		bool undeterminedHand = true;

		for (int i = 0; i < 2; i++)
		{
			if ((unityHands[i]).hand != null && unityHands[i].hand.Id == h.Id && unityHands[i].isHandDetermined)
			{
				unityHands[i].hand = h; //update the state of unity hand
				undeterminedHand = false;
			}
		}

		if (undeterminedHand)
			AssignHands(h);																		
	}

	private void OnHandLost(int Id)
	{
		Messenger.Broadcast<int>(SIG.HANDLOST.ToString(), Id);	// broadcast lost hand ID to registered listeners e.g. game objects, controllers, etc.
		
		for (int i = 0; i < 2; i++)
		{
			if (unityHands[i].hand != null && unityHands[i].hand.Id == Id)
			{
				unityHands[i].HandLost();
			}
		}
	}

	/// <summary>
	/// determines if detected leap hand is left or right
	/// </summary>
	/// <param name="h">leap input hand</param>
	private void AssignHands(Hand h)
	{										
		// delay left/right analysis for better accuracy
		if (h.TimeVisible < timeVisible)
			return;

		//if fingers are displayed, determine left / right based on thumb
		for (int i = 0; i < h.Fingers.Count; i++)
		{
			Finger f = h.Fingers[i];

			if (!f.IsValid) 
				continue; 

			Vector3 palmRightAxis = Vector3.Cross(h.Direction.ToUnity(), h.PalmNormal.ToUnity());
			float fingerDot = Vector3.Dot((f.TipPosition - h.PalmPosition).ToUnity().normalized, palmRightAxis.normalized);

			// is finger tip where we would expect the thumb to be?
			if (Mathf.Abs(fingerDot) > 0.9f)
			{
				int index = fingerDot > 0 ? 0 : 1;

				if (unityHands[index].hand != null && unityHands[index].hand.Id != h.Id)
				{
					if (unityHands[index].isHandDetermined)
					{
						SolveAmbiguity(index, h);
						return;
					}

					Debug.Log("OVERRIDING OLD HAND");
					unityHands[1 - index].isHandDetermined = true;
					unityHands[1 - index].AssignHand(unityHands[index].hand);
					unityHands[index].isHandDetermined = true;
					unityHands[index].AssignHand(h);
					
					return;
				}

				unityHands[index].AssignHand(h);
				unityHands[index].isHandDetermined = true;

				// If this hand was previously the opposite hand, call HandLost on opposite hand
				if (unityHands[1 - index].hand != null && unityHands[index].hand.Id == unityHands[1 - index].hand.Id)
				{
					unityHands[1 - index].HandLost();
				}
				return;
			}
		}

		for (int i = 0; i < 2; i++)
		{
			if (unityHands[i].hand != null && unityHands[i].hand.Id == h.Id)
			{
				unityHands[i].AssignHand(h);
				return;
			}
		}

		//if finger analysis did not determine a hand, make an assumption based on position
		AssumeHand(h);
	}
	
	/// <summary>
	/// assume left / right hand based on absolute leap position
	/// </summary>
	/// <param name="h">hand to be solved</param>
	private void AssumeHand(Hand h)
	{
		int index = h.PalmPosition.x < 0 ? 0 : 1;

		if (unityHands[index].hand == null)
			unityHands[index].AssignHand(h);
		else
			SolveAmbiguity(index, h);
	}

	/// <summary>
	/// Solve Hand ambiguity (two right hands or two left hands)
	/// </summary>
	/// <param name="conflictIndex"> Index of conflict with new hand</param>
	/// <param name="h"></param>
	private void SolveAmbiguity(int conflictIndex, Hand h)
	{
		Hand leftMost = h.PalmPosition.x < unityHands[conflictIndex].hand.PalmPosition.x ? h : unityHands[conflictIndex].hand;
		Hand rightMost = leftMost.Id == h.Id ? unityHands[conflictIndex].hand : h;

		unityHands[0].hand = leftMost;
		unityHands[1].hand = rightMost;
	}
	
		

	private void OnDestroy()
	{
		//necessary to clean delegate assignment between scenes
		LeapInputEx.HandFound -= OnHandFound;
		LeapInputEx.HandUpdated -= OnHandUpdated;
		LeapInputEx.HandLost -= OnHandLost;
	}

}
