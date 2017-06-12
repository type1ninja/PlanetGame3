using UnityEngine;
using UnityEngine.UI;

public class SpaceMove : MonoBehaviour {

    private Rigidbody rigbod;
    private Slider thrustFuelSlider;
    private Slider boostCooldownSlider;

    //WASD + Space + Ctrl thrust force, all directions
    public float BASE_ACCEL = 1.0f;
    //Maximum speed to allow omnidirectional thrust
    public float MAX_SPEED_FOR_BASE = 5.0f;
    //Shift thrust force, forwards
    public float MAIN_THRUST = 0.05f;
    //Shift thrust fuel in seconds
    public float MAIN_THRUST_FUEL_MAX = 3.0f;
    //Shift thrust regen cooldown in seconds
    public float MAIN_THRUST_FUEL_REGEN_MAX = 0.5f;
    //Shift thrust regen speed
    public float MAIN_THRUST_FUEL_REGEN_SPEED = 0.5f;
    //Shift thrust regen speed when grounded
    public float MAIN_THRUST_FUEL_REGEN_SPEED_GROUNDED = 3.0f;
    //Boost thrust, forwards
    public float BOOST_FORCE = 30.0f;
    //Boost max cooldown in seconds
    public float BOOST_COOLDOWN_MAX = 3.0f;

    private bool nearSurface = false;
    private float boostCooldown = 0;
    private float thrustFuel = 1.0f;
    private float thrustRegenCooldown = 0;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();
        thrustFuel = MAIN_THRUST_FUEL_MAX;

        thrustFuelSlider = GameObject.Find("HUDCanvas").transform.Find("ThrustFuelSlider").GetComponent<Slider>();
        thrustFuelSlider.maxValue = MAIN_THRUST_FUEL_MAX;

        boostCooldownSlider = GameObject.Find("HUDCanvas").transform.Find("BoostCooldownSlider").GetComponent<Slider>();
        boostCooldownSlider.maxValue = BOOST_COOLDOWN_MAX;
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
        if (Input.GetAxis("MainThrust") != 0 && thrustFuel >= 0)
        {
            rigbod.AddForce(transform.forward * Input.GetAxis("MainThrust") * MAIN_THRUST, ForceMode.Impulse);
        } 
    }

    private void Update()
    {
        //Only do strong boost if you're very near a planet, walking, and not on cooldown
        if (nearSurface && boostCooldown <= 0 && Input.GetAxis("Boost") != 0 && rigbod.velocity.magnitude < MAX_SPEED_FOR_BASE)
        {
            rigbod.AddForce(transform.forward * BOOST_FORCE, ForceMode.Impulse);
            boostCooldown = BOOST_COOLDOWN_MAX;
        }

        //If boostCooldown is above 0, reduce it
        if (boostCooldown > 0)
        {
            boostCooldown -= Time.deltaTime;
        }

        //Main thrust fuel consumption and regen
        if (Input.GetAxis("MainThrust") != 0)
        {
            thrustRegenCooldown = MAIN_THRUST_FUEL_REGEN_MAX;

            if (thrustFuel >= 0)
            {
                thrustFuel -= Time.fixedDeltaTime;
            }
        }
        if (thrustRegenCooldown <= 0 && thrustFuel < MAIN_THRUST_FUEL_MAX && Input.GetAxis("MainThrust") == 0)
        {
            if (nearSurface)
            {
                thrustFuel += Time.deltaTime * MAIN_THRUST_FUEL_REGEN_SPEED_GROUNDED;
            }
            else
            { 
                thrustFuel += Time.deltaTime * MAIN_THRUST_FUEL_REGEN_SPEED;
            }
        }
        else if (thrustRegenCooldown >= 0)
        {
            thrustRegenCooldown -= Time.deltaTime;
        }
        //Debug.Log("Thrust Fuel: " + thrustFuel + " Thrust Regen Cooldown: " + thrustRegenCooldown);

        //UI Stuff
        thrustFuelSlider.value = thrustFuel;
        boostCooldownSlider.value = BOOST_COOLDOWN_MAX - boostCooldown;
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