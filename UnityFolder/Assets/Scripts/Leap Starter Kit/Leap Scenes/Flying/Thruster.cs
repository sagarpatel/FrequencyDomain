using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour
{
	ParticleSystem[] thrusters;
    
    void Start()
    {
		thrusters = transform.parent.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newRot = transform.localRotation.eulerAngles;
        newRot.x = transform.parent.rigidbody.velocity.magnitude * 7.0f;
        transform.localRotation = Quaternion.Euler(newRot);

		foreach (ParticleSystem ps in thrusters)
			ps.enableEmission = Vector3.Dot(transform.parent.rigidbody.velocity, transform.parent.forward) > 0; // Enable thruster particle systems if we're moving forward
		
    }
}
