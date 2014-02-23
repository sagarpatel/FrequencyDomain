using UnityEngine;

[System.Serializable]
public class HandModel
{
	public GameObject model;
	private GameObject m;

	[HideInInspector]
	public bool isActive;

	public HANDSTATE type;

	public void Initialize(Transform parent)
	{
		m = (GameObject)GameObject.Instantiate(model, parent.position, Quaternion.identity);
		m.transform.parent = parent;
		m.SetActive(false);
	}

	public void SetActive(HANDSTATE hm)
	{
		isActive = hm == type;
		m.SetActive(isActive);
	}
	

	public GameObject GetModel()
	{
		return m;
	}
}
