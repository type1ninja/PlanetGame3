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
            //Debug.Log(gravObj.name + " " + Vector3.Distance(transform.position, gravObj.transform.position));
            rigbod.AddForce(GetGravityForce(gravObj.GetPos(), gravObj.GetMass()), ForceMode.Impulse);
        }
    }

    //Method to calculate force of gravity
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
        Vector3 force = dir * ((rigbod.mass * mass) / distSqrd);
        //Debug.Log((rigbod.mass * mass) / distSqrd);
        
        //Deprecated--more complex distance term
        //Force is direction * the strength of the pull
        //Because we're ignoring the gravitational constant, 
        //pull strength = our mass * other mass * distance modifier
        //Vector3 force = dir * ((rigbod.mass * mass) * CalcDistanceTerm(Vector3.Distance(transform.position, pos), radius));
        return force;
    }

    //Deprecated
    //This is a complex method to calculate the distance term for gravity so it scales exactly how I want it to
    //It's a lot more complicated than 1/x^2
    //Essentially, I want gravity to be fairly strong around the planet, but start dropping off slowly farther away, then go to near zero
    //It looks like this: ((-arctan((x - 40) / 10)) / (pi / 2)) + 1) / 2
    //private float CalcDistanceTerm(float dist, float radius)
    //{
    //    Debug.Log(((-Mathf.Atan(dist - radius) / (radius / 4)) / (Mathf.PI / 2) + 1) / 2);
    //    return ((-Mathf.Atan(dist - radius) / (radius / 4)) / (Mathf.PI / 2) + 1) / 2;
    //}
}