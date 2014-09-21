using UnityEngine;
using System.Collections;


/// <summary>
/// Handles turning on/off flashlight
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class LeapFlashlightObject : LeapBasicObject
{
    public Light spotLight;
    public Light bulbLight;
    
    public override LeapState Activate(HandTypeBase h)
    {
        spotLight.enabled = true;
        bulbLight.enabled = true;
        return base.Activate(h);
    }

    public override LeapState Release(HandTypeBase h)
    {
        spotLight.enabled = false;
        bulbLight.enabled = false;
        return base.Release(h);
    }

}
