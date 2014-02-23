using UnityEngine;
using System.Collections;

public class InstantiateObject : MonoBehaviour {

    public GameObject obj;
    public int triggerID = -1; // Set this ID to a number, and set any objects that should receive its messages to the same number

    private GameObject objInst;

	// Use this for initialization
	void Start () {
        objInst = (GameObject)Instantiate(obj);

        if (triggerID >= 0)
            Messenger.AddListener<int>(SIG.TRIGGERACTIVATED.ToString(), ResetObject);
	}

    private void ResetObject(int id)
    {
        if (triggerID == id)
        {
            Destroy(objInst);
            objInst = (GameObject)Instantiate(obj);
        }
    }

}
