using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Swingable Sword object
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class LeapSwingableObject : LeapGameObject
{
    public TrailRenderer swipe;

    protected override void Start()
    {
        base.Start();

        TrailRenderer trail = GetComponent<TrailRenderer>();

        if (trail && !swipe)
        {
            swipe = trail;
        }
    }

    public override LeapState Activate(HandTypeBase h)
    {
        if (owner != null)
            return null;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        collider.enabled = false;

        return new LeapSwingableState(this);
    }

    public override LeapState Release(HandTypeBase h)
    {
        LeapState state = null;

        if (!isStatePersistent)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            collider.enabled = true;

            state = base.Release(h);
        }

        return state;
    }

    public void CheckSwipe()
    {
        if (swipe)
        {
            swipe.enabled = (owner.unityHand.hand.PalmVelocity.ToUnityTranslated().magnitude > 18);

            if (swipe.enabled)
            {
                collider.enabled = true;
            }
            else
            {
                collider.enabled = false;
            }
        }
    }
}
