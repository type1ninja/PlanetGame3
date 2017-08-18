using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : Photon.MonoBehaviour {

    public GameObject GrenadePrefab;

    private Slider grenadeChargeSlider;

    private Transform grenadeSpawnPositionTransform;
    private Rigidbody rigbod;
    private Collider col;

    private GameManager gameManager;

    private float MAX_GRENADE_CHARGE_TIME = 4.0f;
    private float MAX_GRENADE_FORCE = 35.0f;
    private float MAX_GRENADE_COOLDOWN = 0.5f;

    private float grenadeChargeTime = 0;
    private float grenadeForce = 0;
    private float grenadeCooldown;

    private void Start()
    {
        grenadeChargeSlider = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("GrenadeChargeSlider").GetComponent<Slider>();
        grenadeChargeSlider.maxValue = MAX_GRENADE_CHARGE_TIME;

        grenadeSpawnPositionTransform = transform.Find("GrenadeSpawnPosition");
        rigbod = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        grenadeCooldown = MAX_GRENADE_COOLDOWN;
    }

    private void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        if (!gameManager.GetIsPaused())
        {
            //Grenade charging check
            if (Input.GetButton("PrimaryFire"))
            {
                grenadeChargeTime += Time.deltaTime;
                //Grenade force = % of total charge * maximum charge
                grenadeForce = (grenadeChargeTime / MAX_GRENADE_CHARGE_TIME) * MAX_GRENADE_FORCE;
                //Grenade force is limited to max grenade force
                if (grenadeForce > MAX_GRENADE_FORCE)
                {
                    grenadeForce = MAX_GRENADE_FORCE;
                }
            }

            grenadeChargeSlider.value = grenadeChargeTime;

            //Grenade firing check
            if (Input.GetButtonUp("PrimaryFire") && grenadeCooldown <= 0)
            {
                //Call the method for ourselves, then call it as an RPC for everyone else
                FireGrenade(grenadeSpawnPositionTransform.position, transform.rotation, true, rigbod.velocity, transform.forward * grenadeForce);
                photonView.RPC("FireGrenade", PhotonTargets.Others, grenadeSpawnPositionTransform.position,
                    transform.rotation, false, rigbod.velocity, transform.forward * grenadeForce);

                grenadeCooldown = MAX_GRENADE_COOLDOWN;
                grenadeChargeTime = 0;
            }
        }

        grenadeCooldown -= Time.deltaTime;
    }

    [PunRPC]
    private void FireGrenade(Vector3 pos, Quaternion rot, bool isLocal, Vector3 inheritedVelocity, Vector3 launchForce) 
    {
        GameObject grenade = Instantiate(GrenadePrefab, pos, rot);
        grenade.GetComponent<Grenade>().SetIsLocal(isLocal);
        grenade.GetComponent<Rigidbody>().AddForce(inheritedVelocity, ForceMode.VelocityChange);
        grenade.GetComponent<Rigidbody>().AddForce(launchForce, ForceMode.Impulse); 

        foreach (Collider grenadeCol in grenade.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(col, grenadeCol);
        }
    }
}