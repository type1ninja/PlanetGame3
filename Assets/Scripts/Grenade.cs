using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    private ParticleSystem explosionParticles;
    private ParticleSystem trailParticles;

    //Keeps track of whether the local player spawned this or another player. Basically, we only deal damage if we're local; otherwise
    //the grenade is cosmetic.
    bool isLocal = false;

    private int grenadeDamage = 75;
    private int directHitBonusDmg = 50;
    private float despawnLifetime = 20.0f;
    private float fuseTime = 0.25f;
    bool triggered = false;

    //List of health objects currently within the explosion radius
    List<Health> healthsInRange = new List<Health>();

    private void Start()
    {
        explosionParticles = transform.Find("ExplosionParticles").GetComponent<ParticleSystem>();
        trailParticles = transform.Find("Trail").GetComponent<ParticleSystem>();
    }

	private void Update () {
		if (triggered)
        {
            fuseTime -= Time.deltaTime;
        }

        despawnLifetime -= Time.deltaTime;

        if (fuseTime <= 0 || despawnLifetime <= 0)
        {
            Explode();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Health>() != null)
        {
            triggered = true;

            healthsInRange.Add(other.GetComponent<Health>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        healthsInRange.Remove(other.GetComponent<Health>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocal)
        {
            if (collision.gameObject.GetComponent<Health>() != null)
            {
                collision.gameObject.GetComponent<Health>().photonView.RPC("TakeDamage", PhotonTargets.All, directHitBonusDmg);
            }
        }
        Explode();
    } 

    private void Explode()
    {
        if (isLocal)
        {
            foreach (Health nextHealth in healthsInRange)
            {
                nextHealth.photonView.RPC("TakeDamage", PhotonTargets.All, grenadeDamage);
            }
        }
        
        explosionParticles.Play();
        explosionParticles.transform.SetParent(null);
        trailParticles.Stop();
        trailParticles.transform.SetParent(null);

        Destroy(explosionParticles.gameObject, 2f);
        Destroy(trailParticles.gameObject, 5f);
        Destroy(gameObject);
    }

    public void SetIsLocal(bool newIsLocal)
    {
        isLocal = newIsLocal;
    }
}