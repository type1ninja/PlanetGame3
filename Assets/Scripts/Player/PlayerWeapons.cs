using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour {

    public GameObject GrenadePrefab;

    private Transform GrenadeSpawnPositionTransform;
    private Rigidbody rigbod;
    private Collider col;

    private float grenadeForce = 10.0f;
    private float maxGrenadeCooldown = 0.5f;
    private float currentGrenadeCooldown;
    private float grenadeSpawnZOffset = 1.0f;

    private void Start()
    {
        GrenadeSpawnPositionTransform = transform.Find("GrenadeSpawnPosition");
        rigbod = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        currentGrenadeCooldown = maxGrenadeCooldown;
    }

    private void Update()
    {
        if (Input.GetButton("PrimaryFire") && currentGrenadeCooldown <= 0)
        {
            FireGrenade(); 

            currentGrenadeCooldown = maxGrenadeCooldown;
        }

        currentGrenadeCooldown -= Time.deltaTime;
    }

    private void FireGrenade()
    {
        GameObject grenade = Instantiate(GrenadePrefab, GrenadeSpawnPositionTransform.position, Quaternion.identity);
        grenade.GetComponent<Rigidbody>().AddForce(rigbod.velocity, ForceMode.VelocityChange);
        grenade.GetComponent<Rigidbody>().AddForce(transform.forward * grenadeForce, ForceMode.Impulse); 

        foreach (Collider grenadeCol in grenade.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(col, grenadeCol);
        }
    }
}