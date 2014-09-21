using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public int speed = 2000;
    private int lifeTime = 4;
    private float currentLife = 0;

	
	void Start () {
        transform.rigidbody.AddForce(transform.forward * speed);
	}
	
	
	void Update () {
        if (currentLife > lifeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            currentLife += Time.deltaTime;
        }
	}

    void OnCollisionEnter(Collision c)
    {
        Destroy(gameObject);
    }
}