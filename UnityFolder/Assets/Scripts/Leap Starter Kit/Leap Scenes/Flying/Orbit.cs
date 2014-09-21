using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour
{
    public Transform m_center;
    public Vector3 m_initVelocity = Vector3.zero;
    // Use this for initialization
    void Start()
    {
        rigidbody.velocity = m_initVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody.velocity += (m_center.position - transform.position).normalized * 0.5f;
    }
}