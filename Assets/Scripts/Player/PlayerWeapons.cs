using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : Photon.MonoBehaviour, IPunObservable {

    public GameObject GrenadePrefab;

    private Slider grenadeChargeSlider;

    private Transform grenadeSpawnPositionTransform;
    private Transform laserSpawnPositionTransform;
    private LineRenderer laserGraphics;
    private Rigidbody rigbod;
    private Collider col;

    private GameManager gameManager;

    private float MAX_GRENADE_CHARGE_TIME = 2.5f;
    private float MAX_GRENADE_FORCE = 75.0f;
    private float MAX_GRENADE_COOLDOWN = 0.5f;
    private float MAX_LASER_RANGE = 200f;
    private float MAX_LASER_DAMAGE_COOLDOWN = 0.25f;
    private float LASER_DPS = 40;

    private float grenadeChargeTime = 0;
    private float grenadeForce = 0;
    private float grenadeCooldown;
    private float laserCooldown;

    private bool isFiringLasers = false;

    private void Start()
    {
        grenadeChargeSlider = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("GrenadeChargeSlider").GetComponent<Slider>();
        grenadeChargeSlider.maxValue = MAX_GRENADE_CHARGE_TIME;

        grenadeSpawnPositionTransform = transform.Find("GrenadeSpawnPosition");
        laserSpawnPositionTransform = transform.Find("LaserSpawnPosition");
        laserGraphics = GetComponentInChildren<LineRenderer>();
        rigbod = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        grenadeCooldown = MAX_GRENADE_COOLDOWN;
        laserCooldown = MAX_LASER_DAMAGE_COOLDOWN;
    }

    private void Update()
    {
        if (isFiringLasers)
        {
            laserGraphics.enabled = true;
            RaycastHit hit;
            Vector3 endPos = new Vector3(0, 0, MAX_LASER_RANGE);
            if (Physics.Raycast(transform.position, transform.forward, out hit, MAX_LASER_RANGE))
            {
                endPos = transform.InverseTransformPoint(hit.point);
            }
            laserGraphics.SetPosition(0, laserSpawnPositionTransform.localPosition);
            laserGraphics.SetPosition(1, endPos);
        } else
        {
            laserGraphics.enabled = false;
        }

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

            //Laser firing check
            if (Input.GetButton("SecondaryFire"))
            {
                isFiringLasers = true;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, MAX_LASER_RANGE) && laserCooldown <= 0)
                {
                    if (hit.collider.gameObject.GetComponent<Health>() != null)
                    {
                        Debug.Log(((int)(MAX_LASER_DAMAGE_COOLDOWN * LASER_DPS)).GetType());
                        hit.collider.gameObject.GetComponent<Health>().photonView.RPC("TakeDamage", PhotonTargets.All, (int) (MAX_LASER_DAMAGE_COOLDOWN * LASER_DPS));
                        laserCooldown = MAX_LASER_DAMAGE_COOLDOWN;
                    }
                }
            } else
            {
                isFiringLasers = false;
            }
        }

        grenadeCooldown -= Time.deltaTime;
        laserCooldown -= Time.deltaTime;
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

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isFiringLasers);
        }
        else
        {
            // Network player, receive data
            isFiringLasers = (bool)stream.ReceiveNext();
        }
    }
}