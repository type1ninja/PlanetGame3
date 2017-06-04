using UnityEngine;

public class SpaceMove : MonoBehaviour {

    private Rigidbody rigbod;

    //WASD + Space + Ctrl thrust force, all directions
    public float BASE_ACCEL = 1.0f;
    //Shift thrust force, forwards
    public float MAIN_THRUST = 7.5f;
    //Maximum speed to allow omnidirectional thrust
    public float MAX_SPEED_FOR_BASE = 5.0f;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Only do omnidirectional thrust if you're going below a certain speed
        if (rigbod.velocity.magnitude < MAX_SPEED_FOR_BASE)
        {
            //Left/right thrust
            rigbod.AddForce(transform.right * Input.GetAxis("Horizontal") * BASE_ACCEL, ForceMode.VelocityChange);
            //Up/down thrust
            rigbod.AddForce(transform.up * Input.GetAxis("JumpAxis") * BASE_ACCEL, ForceMode.VelocityChange);
            //Forward/backward thrust
            rigbod.AddForce(transform.forward * Input.GetAxis("Vertical") * BASE_ACCEL, ForceMode.VelocityChange);
        }

        rigbod.AddForce(transform.forward * Input.GetAxis("Thrust") * MAIN_THRUST, ForceMode.Force);
    }
}