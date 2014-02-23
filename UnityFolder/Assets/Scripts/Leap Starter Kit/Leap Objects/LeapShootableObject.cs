using UnityEngine;
using System.Collections;
using Leap;

public class LeapShootableObject : LeapGameObject {

    public GameObject projectile;
    public Transform projectileOffset;

    private float waitMaxTime = 0.1f;
    private float waitTime = 0;

    public override LeapState Activate(HandTypeBase h)
    {
        if (owner != null)
            return null;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        return new LeapShootableState(this);
    }

    public override LeapState Release(HandTypeBase h)
    {
        LeapState state = null;

        if (!isStatePersistent)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            state = base.Release(h);
        }

        return state;
    }

    public override void UpdateTransform(HandTypeBase t)
    {
        UpdateShootable(t);
    }

    void UpdateShootable(HandTypeBase t)
    {
        Collider o = collider;

        Vector3 handOffset = new Vector3();
        Vector3 grabOffsetPos = new Vector3();
        bool fingerFound = false;
        
        // Uses Index Finger to aim
        if (owner.unityHand.unityFingers.ContainsKey(FINGERS.INDEX))
        {
            fingerFound = true;
        }

        o.transform.position = owner.transform.position + handOffset - grabOffsetPos;

        Vector3 tipPosition;

        if (fingerFound)
        {
            tipPosition = owner.unityHand.unityFingers[FINGERS.INDEX].transform.position;
        }
        else
        {
            tipPosition = owner.unityHand.hand.Fingers.Frontmost.TipPosition.ToUnityTranslated();
        }

        //Look at Finger
        o.transform.LookAt(tipPosition, owner.unityHand.transform.up); // Add finger rotation
    }

    public void CheckFireBullet()
    {
        if (owner.unityHand.hand.Fingers.Count == 1)
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        if (waitTime > waitMaxTime)
        {
            waitTime = 0;

            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            if (projectileOffset != null)
            {
                pos = projectileOffset.position;
                rot = projectileOffset.rotation;
            }

            GameObject.Instantiate(projectile, pos, rot);
        }
        waitTime += Time.deltaTime;
    }
}
