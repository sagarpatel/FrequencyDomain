using UnityEngine;
using System.Collections;

public class FingerLineRender : MonoBehaviour {

    private LineRenderer line;

	// Use this for initialization
	void Start () {

        LineRenderer l = GetComponent<LineRenderer>();

        if (l)
        {
            line = l;
        }	
	}

    void Update()
    {
        if (line)
        {
            line.SetPosition(0, Vector3.zero - transform.localPosition * 2);
            line.SetPosition(1, Vector3.zero);
        }
    }
}
