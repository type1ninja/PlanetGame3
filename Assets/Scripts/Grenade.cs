using UnityEngine;

public class Grenade : MonoBehaviour {

    private float despawnLifetime = 20.0f;
    private float fuseTime = 0.25f;
    bool triggered = false;

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
        if (other.tag.Equals("Player"))
        {
            triggered = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    } 

    private void Explode()
    {
        Destroy(gameObject);
    }
}