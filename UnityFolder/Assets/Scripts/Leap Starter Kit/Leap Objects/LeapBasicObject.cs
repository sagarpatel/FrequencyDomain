using UnityEngine;
using System.Collections;

/// <summary>
/// Basic Leap Game Object, dropped when released
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class LeapBasicObject : LeapGameObject {

    public override LeapState Activate(HandTypeBase h)
    {
        if (owner != null)
            return null;

        if (canGoThroughGeometry && rigidbody)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
        return base.Activate(h);
    }

    public override LeapState Release(HandTypeBase h)
    {
        LeapState state = null;

        if (!isStatePersistent)
        {
            if (canGoThroughGeometry && rigidbody)
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
            }

            state = base.Release(h);
        }

        return state;
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        base.UpdateTransform(t);

        if (!rigidbody.isKinematic)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            // Necessary to switch Hand Updates for collisions
            if (collisionOccurred)
            {
                owner.unityHand.runUpdate = false;
            }
            else
            {
                owner.unityHand.runUpdate = true;
            }
        }
    }
}
