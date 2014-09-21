using UnityEngine;
using System.Collections;

public class ToggleCam : MonoBehaviour {

    public Camera FPSCam;
    public Camera ThirdCam;
    public KeyCode key = KeyCode.C;

    bool camToggle = false;

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(key))
        {
            camToggle = !camToggle;

            if (camToggle)
            {
                ThirdCam.enabled = true;
                FPSCam.enabled = false;
            }
            else
            {
                ThirdCam.enabled = false;
                FPSCam.enabled = true;
            }
        }
	
	}
}
