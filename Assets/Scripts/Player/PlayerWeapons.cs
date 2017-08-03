using UnityEngine;

public class PlayerWeapons : Photon.MonoBehaviour {

    public GameObject GrenadePrefab;

    private Transform GrenadeSpawnPositionTransform;
    private Rigidbody rigbod;
    private Collider col;

    private GameManager gameManager;

    private float grenadeForce = 10.0f;
    private float maxGrenadeCooldown = 0.5f;
    private float currentGrenadeCooldown;

    private void Start()
    {
        GrenadeSpawnPositionTransform = transform.Find("GrenadeSpawnPosition");
        rigbod = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        currentGrenadeCooldown = maxGrenadeCooldown;
    }

    private void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        if (!gameManager.GetIsPaused())
        {
            if (Input.GetButton("PrimaryFire") && currentGrenadeCooldown <= 0)
            {
                //Call the method for ourselves, then call it as an RPC for everyone else
                FireGrenade(GrenadeSpawnPositionTransform.position, transform.rotation, true, rigbod.velocity, transform.forward * grenadeForce);
                photonView.RPC("FireGrenade", PhotonTargets.Others, GrenadeSpawnPositionTransform.position,
                    transform.rotation, false, rigbod.velocity, transform.forward * grenadeForce);

                currentGrenadeCooldown = maxGrenadeCooldown;
            }
        }

        currentGrenadeCooldown -= Time.deltaTime;
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