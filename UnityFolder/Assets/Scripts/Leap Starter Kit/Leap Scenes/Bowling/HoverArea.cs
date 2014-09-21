using UnityEngine;
using System.Collections;

public class HoverArea : MonoBehaviour {

    public float hoverForce = 12;
    public bool up = true;

    void OnTriggerStay(Collider o)
    {
        if (up)
        {
            o.rigidbody.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
        }
        else
        {
            o.rigidbody.AddForce(-Vector3.up * hoverForce, ForceMode.Acceleration);
        }
    }

    void OnTriggerExit(Collider o)
    {
        if (!o.rigidbody.isKinematic)
        {
            o.rigidbody.velocity = Vector3.zero;
        }
    }

}
