using UnityEngine;
using System.Collections;

public class BreakingBag : MonoBehaviour
{

	private float bagTime = 0.0f;
	private float bagMaxTime = 5.0f;
	private bool bagDead = false;

	void Update()
	{
		if (bagDead)
		{
			bagTime += Time.deltaTime;

			if (bagTime > bagMaxTime)
			{
				bagDead = false;
				BagManager.RespawnBag();
			}
		}

	}

	void OnJointBreak(float breakForce)
	{
		Debug.Log("BAG DEFEATED!!");
		bagDead = true;
	}

}
