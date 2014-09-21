using UnityEngine;
using System.Collections;

public class BrakeDetection : MonoBehaviour {

    [HideInInspector]
    public bool brakeCollision = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider o)
    {
        HandTypeRigged myComp = o.GetComponent<HandTypeRigged>();

        if (myComp)
        {
            brakeCollision = true;
        }
    }

    void OnTriggerExit(Collider o)
    {
        HandTypeRigged myComp = o.GetComponent<HandTypeRigged>();

        if (myComp)
        {
            brakeCollision = false;
        }
    }
}
