using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// 
/// </summary>
public class LeapFly : MonoBehaviour
{

    Controller m_leapController;

	
    // Use this for initialization
    void Start()
    {
        m_leapController = new Controller();
        if (transform.parent == null)
        {
            Debug.LogError("LeapFly must have a parent object to control");
        }

    }

    Hand GetLeftMostHand(Frame f)
    {
        float smallestVal = float.MaxValue;
        Hand h = null;
        for (int i = 0; i < f.Hands.Count; ++i)
        {
            if (f.Hands[i].PalmPosition.ToUnity().x < smallestVal)
            {
                smallestVal = f.Hands[i].PalmPosition.ToUnity().x;
                h = f.Hands[i];
            }
        }
        return h;
    }

    Hand GetRightMostHand(Frame f)
    {
        float largestVal = -float.MaxValue;
        Hand h = null;
        for (int i = 0; i < f.Hands.Count; ++i)
        {
            if (f.Hands[i].PalmPosition.ToUnity().x > largestVal)
            {
                largestVal = f.Hands[i].PalmPosition.ToUnity().x;
                h = f.Hands[i];
            }
        }
        return h;
    }

    void FixedUpdate()
    {

        Frame frame = m_leapController.Frame();

        if (frame.Hands.Count >= 2)
        {
            Hand leftHand = GetLeftMostHand(frame);
            Hand rightHand = GetRightMostHand(frame);

            // takes the average vector of the forward vector of the hands, used for the
            // pitch of the plane.
            Vector3 avgPalmForward = (frame.Hands[0].Direction.ToUnity() + frame.Hands[1].Direction.ToUnity()) * 0.5f;

            Vector3 handDiff = leftHand.PalmPosition.ToUnityScaled() - rightHand.PalmPosition.ToUnityScaled();

            Vector3 newRot = transform.parent.localRotation.eulerAngles;
            newRot.z = -handDiff.y * 20.0f;

            // adding the rot.z as a way to use banking (rolling) to turn.
            newRot.y += handDiff.z * 3.0f - newRot.z * 0.03f * transform.parent.rigidbody.velocity.magnitude;
            newRot.x = -(avgPalmForward.y - 0.1f) * 100.0f;

            float forceMult = 20.0f;

            // if closed fist, then stop the plane and slowly go backwards.
            if (frame.Fingers.Count < 3)
            {
                forceMult = -3.0f;
            }

            transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, Quaternion.Euler(newRot), 0.1f);
            transform.parent.rigidbody.velocity = transform.parent.forward * forceMult;
        }

    }

}
