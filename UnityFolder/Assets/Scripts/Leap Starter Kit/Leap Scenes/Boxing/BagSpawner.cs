using UnityEngine;
using System.Collections;

public class BagSpawner : MonoBehaviour
{

	public GameObject bag;
	private GameObject currentBag;

	void Start()
	{
		BagManager.bagSpawner = this;
		currentBag = (GameObject)Instantiate(bag);
	}

	void Update()
	{

	}

	public void RespawnNewBag()
	{
		Destroy(currentBag);
		currentBag = (GameObject)Instantiate(bag);
	}
}
