using UnityEngine;
using System.Collections;


/// <summary>
/// Throw grenade
/// </summary>
public class LeapGrenadeObject : LeapThrowableObject
{
    public ParticleEmitter explosion;

    public int explosionTime = 3;
    private float currTime = 0;

    private bool isTriggered = false;

    public override LeapState Activate(HandTypeBase h)
    {
        isTriggered = true;
        return base.Activate(h);
    }

    void Update()
    {

        if (!isTriggered)
            return;

        if (currTime > explosionTime)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            currTime = 0;
            isTriggered = false;

            Instantiate(explosion, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5);
            foreach (Collider hit in colliders)
            {
                if (!hit)
                    continue;

                if (hit.rigidbody)
                    hit.rigidbody.AddExplosionForce(1000, transform.position, 50);
            }
        }
        currTime += Time.deltaTime;
    }

}
