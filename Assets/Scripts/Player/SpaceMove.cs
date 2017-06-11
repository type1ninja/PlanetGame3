using UnityEngine;

public class SpaceMove : MonoBehaviour {

    private Rigidbody rigbod;

    //WASD + Space + Ctrl thrust force, all directions
    public float BASE_ACCEL = 5.0f;
    //Maximum speed to allow omnidirectional thrust
    public float MAX_SPEED_FOR_BASE = 5.0f;
    //Shift thrust force, forwards
    public float MAIN_THRUST = 0.05f;
    //Boost thrust, forwards
    public float BOOST_FORCE = 30.0f;
    //Boost max cooldown in seconds
    public float BOOST_COOLDOWN_MAX = 3.0f;

    private bool nearSurface = false;
    private float boostCooldown = 0;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Only do omnidirectional thrust if you're very near a planet and going below a certain speed
        if (nearSurface && rigbod.velocity.magnitude < MAX_SPEED_FOR_BASE)
        {
            //Forward/backward thrust
            rigbod.AddForce(transform.forward * Input.GetAxis("Vertical") * BASE_ACCEL, ForceMode.VelocityChange);
            //Left/right thrust
            rigbod.AddForce(transform.right * Input.GetAxis("Horizontal") * BASE_ACCEL, ForceMode.VelocityChange);
            //Up/down thrust (deprecated)
            //rigbod.AddForce(transform.up * Input.GetAxis("JumpAxis") * BASE_ACCEL, ForceMode.VelocityChange);
        }

        //Strong boost is located in Update()

        //Main thruster, for orbital adjustments
        rigbod.AddForce(transform.forward * Input.GetAxis("MainThrust") * MAIN_THRUST, ForceMode.Impulse);
    }

    private void Update()
    {
        //Only do strong boost if you're very near a planet and not on cooldown
        if (nearSurface && boostCooldown <= 0 && Input.GetAxis("Boost") != 0)
        {
            rigbod.AddForce(transform.forward * BOOST_FORCE, ForceMode.Impulse);
            boostCooldown = BOOST_COOLDOWN_MAX;
        }

        //If boostCooldown is above 0, reduce it
        if (boostCooldown > 0)
        {
            boostCooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Planet")) {
            nearSurface = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Planet")) {
            nearSurface = false;
        }
    }
}