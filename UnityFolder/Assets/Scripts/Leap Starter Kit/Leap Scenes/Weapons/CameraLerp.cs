using UnityEngine;
using System.Collections;

public class CameraLerp : MonoBehaviour {

    public Transform camPos;
    public Transform camPos1;

    public LeapGameObject obj;
	
	void Start () {
	
	}
	
	
	void Update () {

        if (obj.isHeld)
        {
            transform.position = Vector3.Lerp(transform.position, camPos1.transform.position, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, camPos1.rotation, Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, camPos.transform.position, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, camPos.rotation, Time.deltaTime);
        }
	
	}
}
