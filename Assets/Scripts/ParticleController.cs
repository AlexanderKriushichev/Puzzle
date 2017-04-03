using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    public bool destroyAfterActivate;

    public bool startActive;

    public List<ParticleSystem> particles = new List<ParticleSystem>();

    private float maxTime = 0;

    void Start()
    {
        if (startActive)
            Activate();
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        maxTime = 0;

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
            if (destroyAfterActivate)
            {
                if (maxTime < particle.main.duration + particle.main.startLifetimeMultiplier)
                {
                    maxTime = particle.main.duration + particle.main.startLifetimeMultiplier;
                }
            }
        }

        if (destroyAfterActivate)
        {
            StartCoroutine(DestroyCoroutine());
        }
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(maxTime);

        Destroy(gameObject);
    }


}
