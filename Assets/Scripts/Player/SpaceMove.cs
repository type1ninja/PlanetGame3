using UnityEngine;

public class SpaceMove : MonoBehaviour {

    private Rigidbody rigbod;

    //WASD + Space + Ctrl thrust force, all directions
    public float BASE_THRUST = 5.0f;
    //Shift thrust force, forwards
    public float MAIN_THRUST = 20.0f;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Left/right thrust
        rigbod.AddForce(transform.right * Input.GetAxis("Horizontal") * BASE_THRUST, ForceMode.Force);
        //Up/down thrust
        rigbod.AddForce(transform.up * Input.GetAxis("JumpAxis") * BASE_THRUST, ForceMode.Force);
        //Only do forward/backward thrust if we're not boosting; don't want to add inputs here
        if (Input.GetAxis("Thrust") == 0)
        {
            rigbod.AddForce(transform.forward * Input.GetAxis("Vertical") * BASE_THRUST, ForceMode.Force);
        }

        rigbod.AddForce(transform.forward * Input.GetAxis("Thrust") * MAIN_THRUST, ForceMode.Force);
    }
}