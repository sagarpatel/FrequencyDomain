using UnityEngine;
using System.Collections;

public class ImmediateReturn : MonoBehaviour {

    public int triggerID = -1; // Set this ID to a number, and set any objects that should receive its messages to the same number
    private Vector3 origin;
    private Quaternion originRot;

	void Start () {
        origin = transform.position;
        originRot = transform.rotation;
        if (triggerID >= 0)
            Messenger.AddListener<int>(SIG.TRIGGERACTIVATED.ToString(), ResetPosition);
	}

    private void ResetPosition(int id)
    {
        if (triggerID == id)
        {
            transform.position = origin;
            transform.rotation = originRot;

            if (!rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            gameObject.SetActive(true);
        }
    }
}
