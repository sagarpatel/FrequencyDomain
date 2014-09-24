using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleGrid : MonoBehaviour 
{
	ParticleSystem particleSystem;
	ParticleSystem.Particle[] particlesArray;

	int subdivistionCount = 1000;
	float gapBetweenSubdivisions = 100.0f;

	bool isInit = false;

	void Start () 
	{


	}

	void Update()
	{
		if(isInit == false)
		{
			isInit = true;


			
			particleSystem = GetComponent<ParticleSystem>();
			particlesArray = new ParticleSystem.Particle[particleSystem.maxParticles];

			for (int i = 0; i < particlesArray.Length; i++) 
			{
				particlesArray[i].position = 1000.0f * Random.insideUnitSphere;
				particlesArray[i].size = 100.0f;

			}

			particleSystem.SetParticles(particlesArray, particlesArray.Length);

			print (particleSystem);
			print (particlesArray.Length);

		}

	}


}
