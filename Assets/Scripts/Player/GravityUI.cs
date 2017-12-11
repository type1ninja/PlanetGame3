using UnityEngine;
using UnityEngine.UI;

public class GravityUI : Photon.MonoBehaviour {

    private Text speedHeightText;

    private Rigidbody rigbod;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();

        speedHeightText = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("SpeedHeight").Find("Text").GetComponent<Text>();
    }

    private void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        double maxForce = 0;
        double maxForceSourceDist = 0;
        //Iterate through gravityTransforms (and gravityMasses) to figure out which one is strongest
        foreach (GravityMass gravObj in GravityObjects.gravList)
        {
            //If the current force is the strongest so far, make it the new strongest one (and also set the distance)
            if (GetGravityForce(gravObj.GetPos(), gravObj.GetMass()).magnitude > maxForce)
            {
                maxForce = GetGravityForce(gravObj.GetPos(), gravObj.GetMass()).magnitude;
                maxForceSourceDist = (transform.position - gravObj.GetPos()).magnitude;
            }
        }

        //Update the text box
        speedHeightText.text = rigbod.velocity.magnitude.ToString("F1") + " m/s \n" + maxForceSourceDist.ToString("F1") + " m";
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
        Vector3 force = dir * ((rigbod.mass * mass) / distSqrd);
        //Debug.Log((rigbod.mass * mass) / distSqrd);

        //Deprecated--more complex distance term
        //Force is direction * the strength of the pull
        //Because we're ignoring the gravitational constant, 
        //pull strength = our mass * other mass * distance modifier
        //Vector3 force = dir * ((rigbod.mass * mass) * CalcDistanceTerm(Vector3.Distance(transform.position, pos), radius));
        return force;
    }
}