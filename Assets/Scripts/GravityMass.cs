using UnityEngine;

public class GravityMass : MonoBehaviour
{
    //This boolean indicates whether the gravity center can move
    //TODO--it is currently unused, but it could be used to update position regularly
    public bool moves = false;

    //Current strength and position of this gravity attractor
    public float mass = 100.0f;
    private Vector3 pos = Vector3.zero;

    private void Start()
    {
        //Set position
        pos = transform.position;
        //Add self to static gravity objects list
        GravityObjects.AddGravObj(this);
    }

    private void FixedUpdate()
    {
        if (moves)
        {
            //If this can move, you need to update the position every frame
            pos = transform.position;
        }
    }

    //Public method to retrieve the mass
    public float GetMass()
    {
        return mass;
    }

    //Public method to set the mass
    public void SetMass(float newMass)
    {
        mass = newMass;
    }

    //Public method to retrieve the position
    public Vector3 GetPos()
    {
        return pos;
    }
}