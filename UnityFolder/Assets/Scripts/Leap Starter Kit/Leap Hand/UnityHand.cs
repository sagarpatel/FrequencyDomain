using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

/// <summary>
/// translates leap data to unity worldspace
/// </summary>
public class UnityHand : MonoBehaviour 
{
	public LeapGameObject initialLeapObject; // Setting an initial leap object indicates a scene start in a specific state rather than default

	public HandTypeBase handType;
	public bool isRightHand;

    [HideInInspector]
    public UnityHandSettings settings;
    [HideInInspector]
    public Hand hand;
	[HideInInspector]
	public bool isHandDetermined = false; // Has hand been definitively determined to be right or left?
    [HideInInspector]
	public bool handFound = false; // Indicates first appearance in the scene by hand
    [HideInInspector]
    public bool runUpdate = true; // Needed to determine which update to call

	private Vector3 originalPos;

    public FingerDetection detectedFingers;
    public Dictionary<int, Finger> leapFingers = new Dictionary<int, Finger>();
    public Dictionary<FINGERS, GameObject> unityFingers = new Dictionary<FINGERS, GameObject>();

	void Start () 
	{
		handType = (HandTypeBase)Instantiate(handType, transform.position, Quaternion.identity);
		handType.SetOwner(this);
		handType.name = isRightHand ? "rightHand" : "leftHand";

		renderer.enabled = false; // Disable visual indicator for Unityhand

		if (initialLeapObject)
		{
			initialLeapObject.gameObject.SetActive(true);
			handType.ChangeState(initialLeapObject.Activate(handType));
		}

        originalPos = transform.localPosition;

        detectedFingers = new FingerDetection(this);
        InstantiateFingers();
	}

	void Update () 
	{
        if (!runUpdate)
            return;

        UnityHandUpdate();		
	}

    void FixedUpdate()
    {
        if (runUpdate)
            return;

        UnityHandUpdate();
    }

    private void UnityHandUpdate()
    {
        if (hand != null)
        {
            UpdateHand();
			if (detectedFingers != null)
			{
				detectedFingers.CalculateFingers();
				UpdateFingers();
			}

            handType.UpdateHandType();
            DrawDebug();
        }
    }

    /// <summary>
    /// Creates individual finger objects and attaches them to the hand
    /// </summary>
    void InstantiateFingers()
    {
        GameObject fingers = new GameObject();
        fingers.name = "Fingers";
        fingers.transform.position = transform.position;
        fingers.transform.rotation = transform.rotation;
        fingers.transform.parent = transform;

        for (int i = 0; i < 5; i++)
        {
            GameObject temp;
			
			temp = new GameObject();

            temp.transform.position = transform.position;
            temp.transform.parent = fingers.transform;
            temp.SetActive(false);
            temp.name = ((FINGERS)i).ToString();
            unityFingers.Add(((FINGERS)i), temp);
        }
    }

    /// <summary>
    /// Activates and updates position of Unity Fingers if a corresponding finger is detected
    /// </summary>
    private void UpdateFingers()
    {
        List<int> fingerIDs = new List<int>(leapFingers.Keys);
        Matrix handTran = detectedFingers.handTransform;

        foreach (int i in leapFingers.Keys)
        {
            if (isRightHand)
            {
                Vector3 transformedPosition = handTran.TransformPoint(leapFingers[i].TipPosition).ToUnityScaled();
                transformedPosition *= settings.fingerDistanceMultiplier;

                unityFingers[(FINGERS)i].transform.localPosition = transformedPosition;
                unityFingers[(FINGERS)i].SetActive(true);
            }
            else
            {
                int leftVal = 4 - i;
                Vector3 transformedPosition = handTran.TransformPoint(leapFingers[i].TipPosition).ToUnityScaled();
                transformedPosition *= settings.fingerDistanceMultiplier;

                unityFingers[(FINGERS)leftVal].transform.localPosition = transformedPosition;
                unityFingers[(FINGERS)leftVal].SetActive(true);
            }
        }

        // Disable un-used finger
        for (int i = 0; i < 5; i++)
        {
            if (!fingerIDs.Contains(i) && isRightHand)
            {
                unityFingers[(FINGERS)i].SetActive(false);
            }
			else if (!fingerIDs.Contains(4 - i) && !isRightHand)
            {
                unityFingers[(FINGERS)i].SetActive(false);
            }
        }
    }

	public void UpdateHand()
	{
		// Smoothly update the orientation and position of the hand
		Vector3 newPosition = hand.PalmPosition.ToUnityTranslated();

        newPosition = new Vector3(newPosition.x * settings.leapPosMultiplier.x, newPosition.y * settings.leapPosMultiplier.y, newPosition.z * settings.leapPosMultiplier.z);

		// Offset position of hands
		// Logic works to let hands keep their localPosition (moved by camLookAt)  //TODO: is camLookAt necessary? it seems like hand position shouldn't be determined by camera -jason
		// then it offsets the hand the appropriate amount from Leap device
        transform.localPosition -= originalPos;
        transform.localPosition = transform.localPosition + newPosition;
        originalPos = newPosition;

        Vector3 normal = -hand.PalmNormal.ToUnity();
        Vector3 forward = hand.Direction.ToUnity();

		// Rotation of hands
        transform.rotation = settings.leapPosOffset.rotation;
        transform.rotation *= Quaternion.LookRotation(new Vector3(forward.x, forward.y, forward.z), new Vector3(normal.x, normal.y, normal.z));
	}

	private void DrawDebug()
	{
		Debug.DrawRay(transform.position, transform.forward * 5, Color.blue);
		Debug.DrawRay(transform.position, -transform.up * 5, Color.green);

        foreach (GameObject f in unityFingers.Values)
        {
            Vector3 tipPosition = f.transform.position;

            if (f.activeSelf)
            {
                Debug.DrawLine(transform.position, tipPosition);
            }
        }
	}

	public void AssignSettings(UnityHandSettings s)
	{
		settings = s;
		
	}

	public void HandLost()
	{
        handType.HandLost();  

		hand = null;
		isHandDetermined = false;
		handFound = false;
	}

	public void AssignHand(Hand h)
	{
		if (!handFound)
		{
			handFound = true;
			handType.HandFound();
		}
		hand = h;
	}
		   	
}
