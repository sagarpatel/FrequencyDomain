using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;

/// <summary>
/// This class detects and assigns fingers to the Unity Hand
/// </summary>
public class FingerDetection
{
    public UnityHand unityHand;
    private int prevFingerAmt;
    public Matrix handTransform;

    public FingerDetection(UnityHand hand)
    {
        unityHand = hand;
    }

    public void CalculateFingers()
    {
        UpdateHandTransform();
        UpdateFingers();
        prevFingerAmt = unityHand.hand.Fingers.Count;
    }

    /// <summary>
    /// Used for repositioning fingers based on the hand's transform
    /// </summary>
    private void UpdateHandTransform()
    {
        Hand hand = unityHand.hand;
        Vector handXBasis = hand.PalmNormal.Cross(hand.Direction).Normalized;
        Vector handYBasis = -hand.PalmNormal;
        Vector handZBasis = -hand.Direction;
        Vector handOrigin = hand.PalmPosition;
        handTransform = new Matrix(handXBasis, handYBasis, handZBasis, handOrigin);
        handTransform = handTransform.RigidInverse();
    }

    /// <summary>
    /// If the amount of fingers has changed then reassign fingers.
    /// Also responsible for updating rigged hand joints.
    /// </summary>
    private void UpdateFingers()
    {
        if (unityHand.hand != null)
        {
            Hand hand = unityHand.hand;

            // Assign Fingers
            if (prevFingerAmt != hand.Fingers.Count)
            {
                ReAssignFingers(hand);
            }

            UpdateFingerPositions();
        }
    }

    /// <summary>
    /// Reassigns the fingers depending on if there are 5 fingers or less than 5 fingers.
    /// Reassigns fingers based on fingers X position from the hands transform.
    /// </summary>
    /// <param name="hand"></param>
    private void ReAssignFingers(Hand hand)
    {
        List<Finger> fingers = new List<Finger>();

        for (int f = 0; f < hand.Fingers.Count; f++)
        {
            Finger leapFinger = hand.Fingers[f];
            fingers.Add(leapFinger);
        }

        List<Finger> leftList = new List<Finger>();
        leftList = fingers.OrderBy(e => handTransform.TransformPoint(e.TipPosition).ToUnityTranslated().x).ToList();

        if (leftList.Count >= 5)
        {
            unityHand.leapFingers.Clear();
            AssignAllFingers(leftList);
        }
        else
        {
            RemoveOldFingers(leftList);
            AssignFingers(leftList);
        }

    }

    /// <summary>
    /// Remove fingers no longer active
    /// </summary>
    /// <param name="leftList"></param>


    private void RemoveOldFingers(List<Finger> leftList)
    {
        List<int> fingerIDs = new List<int>();
        List<int> removeList = new List<int>();

        // Get all finger ID's
        foreach (Finger f in leftList)
        {
            fingerIDs.Add(f.Id);
        }

        // Check if fingerID matches current fingers, if not, add it to remove finger
        foreach (KeyValuePair<int, Finger> f in unityHand.leapFingers)
        {
            if (!fingerIDs.Contains(f.Value.Id))
            {
                removeList.Add(f.Key);
            }
        }

        // Remove Fingers
        foreach (int fingerVal in removeList)
        {
            unityHand.leapFingers.Remove(fingerVal);
        }

    }

    /// <summary>
    /// Calculate to figure out which finger is which
    /// </summary>
    /// <param name="leftList"></param>
    /// <param name="handTransform"></param>
    private void AssignFingers(List<Finger> leftList)
    {

        List<int> fingerIDs = new List<int>();

        // Get all finger ID's
        foreach (Finger f in unityHand.leapFingers.Values)
        {
            fingerIDs.Add(f.Id);
        }

        for (int f = 0; f < leftList.Count; f++)
        {
            if (!fingerIDs.Contains(leftList[f].Id))
            {
                AssignFingerBasedOnPosition(leftList, f);
            }
        }
    }

    /// <summary>
    /// Calculate Finger Position
    /// </summary>
    /// <param name="leftList"></param>
    /// <param name="handTransform"></param>
    /// <param name="currentCount"></param>
    private void AssignFingerBasedOnPosition(List<Finger> leftList, int currentCount)
    {
        Finger leapFinger = leftList[currentCount];
        Vector3 transformedPosition = handTransform.TransformPoint(leapFinger.TipPosition).ToUnityScaled();

        Vector basePosition = -leapFinger.Direction * leapFinger.Length;
        basePosition += leapFinger.TipPosition;
        Vector3 baseTransformedPosition = handTransform.TransformPoint(basePosition).ToUnityTranslated();

        bool fingerAssigned = false;

        // It is a Thumb
        if ((transformedPosition.x <= -0.9f) &&
            (baseTransformedPosition.x < -0.2f) &&
            !unityHand.leapFingers.ContainsKey(0))
        {
            unityHand.leapFingers.Add(0, leapFinger);
            fingerAssigned = true;
        }
        // It is an Index Finger
        else if ((transformedPosition.x >= -0.5f &&
                 transformedPosition.x < 0.3f) &&
                 !unityHand.leapFingers.ContainsKey(1))
        {
            unityHand.leapFingers.Add(1, leapFinger);
            fingerAssigned = true;
        }
        // It is Middle Finger
        else if ((transformedPosition.x >= -0.1f &&
                 transformedPosition.x < 0.5f) &&
                 !unityHand.leapFingers.ContainsKey(2))
        {
            unityHand.leapFingers.Add(2, leapFinger);
            fingerAssigned = true;
        }
        // It is Ring Finger
        else if (transformedPosition.x >= 0.2f &&
                 transformedPosition.x < 0.4f &&
                 !unityHand.leapFingers.ContainsKey(3))
        {
            unityHand.leapFingers.Add(3, leapFinger);
            fingerAssigned = true;
        }
        // It is Pinky
        else if (!unityHand.leapFingers.ContainsKey(4))
        {
            unityHand.leapFingers.Add(4, leapFinger);
            fingerAssigned = true;
        }

        // If finger isn't assigned, then assign it to an opening
        if (!fingerAssigned)
        {
            // It is a Thumb
            if (!unityHand.leapFingers.ContainsKey(0))
            {
                unityHand.leapFingers.Add(0, leapFinger);
            }
            // It is an Index Finger
            else if (!unityHand.leapFingers.ContainsKey(1))
            {
                unityHand.leapFingers.Add(1, leapFinger);
            }
            // It is a Middle Finger
            else if (!unityHand.leapFingers.ContainsKey(2))
            {
                unityHand.leapFingers.Add(2, leapFinger);
            }
            // It is a Ring Finger
            else if (!unityHand.leapFingers.ContainsKey(3))
            {
                unityHand.leapFingers.Add(3, leapFinger);
            }
        }
    }

    /// <summary>
    /// If there are 5 fingers, then assigns them in order
    /// </summary>
    /// <param name="leftList"></param>
    /// <param name="handTransform"></param>
    private void AssignAllFingers(List<Finger> leftList)
    {
        for (int f = 0; f < leftList.Count; f++)
        {
            Finger leapFinger = leftList[f];

            unityHand.leapFingers.Add(f, leapFinger);
        }
    }

    /// <summary>
    /// Updates fingers from Leap Motion
    /// </summary>
    private void UpdateFingerPositions()
    {
        Hand hand = unityHand.hand;
        List<int> fingerIDs = new List<int>(unityHand.leapFingers.Keys);

        foreach (Finger f in hand.Fingers)
        {
            foreach (int i in fingerIDs)
            {
                if (unityHand.leapFingers[i].Id == f.Id)
                {
                    unityHand.leapFingers[i] = f;
                }
            }
        }
    }
}
