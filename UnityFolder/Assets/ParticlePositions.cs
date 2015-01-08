using UnityEngine;
using System.Collections;

public class ParticlePositions : MonoBehaviour 
{
	public int lengthCount = 10;
	public float particleSize = 1.0f;
	public Vector3[] positionsArray;

	public ParticleSystem.Particle[] particlesArray;

	public float posScale = 1.0f;

	void Start () 
	{
		particlesArray = new ParticleSystem.Particle[lengthCount * lengthCount * lengthCount];
		particleSystem.SetParticles(particlesArray, particlesArray.Length);

		positionsArray = new Vector3[particleSystem.particleCount];

		Vector3 tempPos = Vector3.zero;
		int xCounter = 0;
		int yCounter = 0;
		int zCounter = 0;
		Vector3 posOffset = new Vector3(lengthCount, lengthCount, lengthCount);
		for(int i = 0 ; i < positionsArray.Length; i++)
		{

			tempPos.x = xCounter;
			tempPos.y = yCounter;
			tempPos.z = zCounter;

			positionsArray[i] = posScale * ( tempPos - 0.5f * posOffset);

			xCounter += 1;
			if(xCounter == lengthCount - 1)
			{
				xCounter = 0;
				yCounter += 1;
			}
			if(yCounter == lengthCount - 1)
			{
				yCounter = 0;
				zCounter += 1;
			}

		}

		for(int i = 0; i < particlesArray.Length; i++)
		{
			particlesArray[i].position = positionsArray[i];
			particlesArray[i].size = particleSize;
			particlesArray[i].color = Color.white;
		}

		particleSystem.SetParticles(particlesArray, particlesArray.Length);

	}


	void Update () 
	{
	
	}
}
