using UnityEngine;
using UnityEngine.UI;

public class SpaceMove : Photon.MonoBehaviour, IPunObservable {

    private Rigidbody rigbod;
    private ParticleSystem thrusterParticles;
    private ParticleSystem boostParticles;

    private Slider thrustFuelSlider;
    private Slider boostCooldownSlider;
    private GameManager gameManager;

    //WASD + Space + Ctrl thrust force, all directions
    public float BASE_ACCEL = 1.0f;
    //Maximum speed to allow omnidirectional thrust
    public float MAX_SPEED_FOR_BASE = 5.0f;
    //Shift thrust force, forwards
    public float MAIN_THRUST = 2.0f;
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
    private bool isThrusting = false;
    private float boostCooldown = 0;
    private float thrustFuel = 1.0f;
    private float thrustRegenCooldown = 0;

    private void Start()
    {
        rigbod = GetComponent<Rigidbody>();
        thrusterParticles = transform.Find("ThrusterParticles").GetComponent<ParticleSystem>();
        boostParticles = transform.Find("BoostParticles").GetComponent<ParticleSystem>();

        thrustFuel = MAIN_THRUST_FUEL_MAX;

        thrustFuelSlider = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("ThrustFuelSlider").GetComponent<Slider>();
        thrustFuelSlider.maxValue = MAIN_THRUST_FUEL_MAX;

        boostCooldownSlider = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("BoostCooldownSlider").GetComponent<Slider>();
        boostCooldownSlider.maxValue = BOOST_COOLDOWN_MAX;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (isThrusting)
        {
            thrusterParticles.Emit(1);
            isThrusting = false;
        }

        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        if (!gameManager.GetIsPaused())
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
            if (Input.GetButton("MainThrust") && thrustFuel >= 0)
            {
                rigbod.AddForce(transform.forward * Input.GetAxis("MainThrust") * MAIN_THRUST, ForceMode.Force);

                isThrusting = true;
            }

            //Only do strong boost if you're very near a planet, walking, and not on cooldown
            if (nearSurface && boostCooldown <= 0 && Input.GetButton("Boost") && rigbod.velocity.magnitude < MAX_SPEED_FOR_BASE)
            {
                rigbod.AddForce(transform.forward * BOOST_FORCE, ForceMode.Impulse);
                boostCooldown = BOOST_COOLDOWN_MAX;

                photonView.RPC("FireBoostParticles", PhotonTargets.All);
            }

            //Main thrust fuel consumption and regen
            if (Input.GetButton("MainThrust"))
            {
                thrustRegenCooldown = MAIN_THRUST_FUEL_REGEN_MAX;

                if (thrustFuel >= 0)
                {
                    thrustFuel -= Time.fixedDeltaTime;
                }
            }
        }

        //If boostCooldown is above 0, reduce it
        if (boostCooldown > 0)
        {
            boostCooldown -= Time.deltaTime;
        }

        if (thrustRegenCooldown <= 0 && thrustFuel < MAIN_THRUST_FUEL_MAX && (!Input.GetButton("MainThrust") || gameManager.GetIsPaused()))
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

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isThrusting);
        }
        else
        {
            // Network player, receive data
            isThrusting = (bool) stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void FireBoostParticles()
    {
        boostParticles.Play();
    }
}