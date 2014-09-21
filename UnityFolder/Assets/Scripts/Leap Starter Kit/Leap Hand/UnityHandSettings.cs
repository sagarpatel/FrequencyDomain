using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnityHandSettings  
{
	public bool highlightObjects = true;

	public float fingerDistanceMultiplier = 1f;
	public Vector3 leapPosMultiplier = Vector3.one;
    public int throwingStrength = 20;
    public float angularStrength = 0.2f;
    public float maxThrowingVelocity = 20;
    public float maxAngularThrowingVelocity = 20;
	
	public Transform leapPosOffset;	
}
