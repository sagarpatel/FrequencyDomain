using UnityEngine;
using System.Collections;

public class DistanceReset : MonoBehaviour {

    public GameObject bowlingBall;
    public float triggerDistance = 30;
    public int triggerID = -1;

    private float waitMaxTime = 3f;
    private float waitTime = 0;

    private void Update()
    {
        BallCheck();
    }

    private void BallCheck()
    {
        if (bowlingBall.transform.position.z > triggerDistance)
        {
            waitTime += Time.deltaTime;

            if (waitTime > waitMaxTime)
            {
                waitTime = 0;
                Messenger.Broadcast<int>(SIG.TRIGGERACTIVATED.ToString(), triggerID);
            }
        }
    }
}
