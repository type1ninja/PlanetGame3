using UnityEngine;

//This script goes on gravity-enabled objects
public class Gravity : MonoBehaviour
{

    //The rigidbody for applying forces
    private Rigidbody rigbod;

    private void Start()
    {
        //Get the rigidbody for this character
        rigbod = GetComponent<Rigidbody>();
    }

    //Physics calculations
    private void FixedUpdate()
    {
        //Iterate through gravityTransforms (and gravityMasses) to apply the forces
        foreach(GravityMass gravObj in GravityObjects.gravList)
        {
            rigbod.AddForce(GetGravityForce(gravObj.GetPos(), gravObj.GetMass()), ForceMode.Impulse);
        }
    }

    private Vector3 GetGravityForce(Vector3 pos, float mass)
    {
        //Get the direction to the attractor
        // Make sure the length is one, so we can scale it up easily with the force
        Vector3 dir = (pos - transform.position).normalized;

        //Set distSqrd to the distance
        float distSqrd = Vector3.Distance(transform.position, pos);
        //Now we actually square distSqrd
        distSqrd *= distSqrd;

        //Force is the direction * the strength of the pull
        //Because we're ignoring the Gravitational Constant, 
        //pull strength = our mass * other mass / distance squared
        Vector3 force = dir * (float)((rigbod.mass * mass) / distSqrd);
        return force;
    }
}