using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandTypeBasic : HandTypeBase 
{

	public GameObject hand;
	public GameObject fingers;

    [HideInInspector]
    private GameObject basicHand;

    [HideInInspector]
    public Dictionary<FINGERS, GameObject> basicFingers = new Dictionary<FINGERS, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        basicHand = (GameObject)Instantiate(hand, transform.position, Quaternion.identity);
        basicHand.transform.parent = transform;
        basicHand.gameObject.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            GameObject temp;

            temp = (GameObject)Instantiate(fingers);
            temp.transform.position = transform.position;
            temp.transform.parent = transform;
            temp.SetActive(false);
            temp.name = ((FINGERS)i).ToString();
            basicFingers.Add(((FINGERS)i), temp);
        }

        stateController.Initialize(this, new LeapGodHandState());
    }

    public override void UpdateHandType()
    {
        base.UpdateHandType();

        for (int i = 0; i < 5; i++)
        {
            basicFingers[(FINGERS)i].transform.localPosition = unityHand.unityFingers[(FINGERS)i].transform.localPosition;
        }

        UpdateState();
    }

    public override void HandLost()
    {
        HideHand();
        base.HandLost();
    }


    public override void HideHand()
    {
        canBeVisible = false;
        basicHand.gameObject.SetActive(false);

        for (int i = 0; i < 5; i++)
        {
            basicFingers[(FINGERS)i].SetActive(false);
        }
    }

    public override void ShowHand()
    {
        canBeVisible = true;
        basicHand.gameObject.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            basicFingers[(FINGERS)i].SetActive(true);
        }
    }

    public override void HandFound()
    {
        ShowHand();
        base.HandFound();
    }
}
