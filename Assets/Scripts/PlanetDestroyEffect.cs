using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDestroyEffect : MonoBehaviour {

    public List<ParticleSystem> particles = new List<ParticleSystem>();

    public void Activate()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
