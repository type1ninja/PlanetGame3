using UnityEngine;

public class Grenade : MonoBehaviour {

    //Keeps track of whether the local player spawned this or another player. Basically, we only deal damage if we're local; otherwise
    //the grenade is cosmetic.
    bool isLocal = false;

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
        //TODO--Deal bonus damage if it's a player
        Explode();
    } 

    private void Explode()
    {
        //Deal damage to every player in the trigger radius
        Destroy(gameObject);
    }

    public void SetIsLocal(bool newIsLocal)
    {
        isLocal = newIsLocal;
    }
}