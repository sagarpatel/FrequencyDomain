using UnityEngine;
using System.Collections;

public class ParticleGeneratorScript : MonoBehaviour 
{
	ParticleSystem.Particle[] particlesArray;
	public int particlesPerRow = 100;
	public int rowCount = 10;
	public float distanceBetweenParticles = 10;
	public float distanceBetweenRows = 10;
	public float particleSize = 10.0f;
	int particleCounter = 0;
	PlayerScript playerScript;

	public float updateRefreshMinimum = 0.02f;
	float updateRefreshCounter;



	// Use this for initialization
	void Start () 
	{
		particlesArray = new ParticleSystem.Particle[particlesPerRow * rowCount];
		
		for(int i = 0; i < rowCount; i++)
		{
			for(int j =0; j < particlesPerRow; j++ )
			{
				particlesArray[particleCounter].position = new Vector3(-j * distanceBetweenParticles , 0, i * distanceBetweenRows );
				//Debug.Log(particlesArray[j + (i * rowCount)].position);
				particlesArray[particleCounter].color = new Color(0.7f, 0.7f, 0.7f);
				particlesArray[particleCounter].size = particleSize;
				particleCounter ++;
			}
		}

		playerScript = (PlayerScript)GameObject.FindGameObjectWithTag("Player").GetComponent("PlayerScript");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateRefreshCounter += Time.deltaTime;

		if(updateRefreshCounter > updateRefreshMinimum)
		{
			updateRefreshCounter -= updateRefreshMinimum;
			
			PropagateHeight();
			
		}
		
		FollowPlayerHeight();
		particleSystem.SetParticles(particlesArray, particlesArray.Length);

	}

	// sets first collumn to current player height
	void FollowPlayerHeight()
	{
		float newHeight = playerScript.transform.position.y;
		
		// set the first of each row, i.e. 1rst collumn
		Vector3 tempPosition;
		for(int i = 0; i < rowCount; i++)
		{
			tempPosition = particlesArray[i * particlesPerRow].position;
			tempPosition.y = newHeight;
			particlesArray[i * particlesPerRow].position = tempPosition;
		}
	}

	// sends height values down the rows
	void PropagateHeight()
	{
		Vector3 tempPosition;
		for(int i = particleCounter -1; i > 0; i--)
		{
			// if first collumn particle
			if(i % particlesPerRow == 0)
				continue;
			tempPosition = particlesArray[i].position;
			tempPosition.y = particlesArray[i - 1].position.y;
			particlesArray[i].position = tempPosition;
		}
	}


}
