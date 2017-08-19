using UnityEngine;
using UnityEngine.UI;

public class Health : Photon.MonoBehaviour, IPunObservable {

    private Slider healthSlider;
    private Rigidbody rigbod;

    private int MAX_HEALTH = 100;
    private int currentHealth = 100;
    private int deathCount = 0;

    private void Start()
    {
        healthSlider = GameObject.Find("Canvas").transform.Find("HUDPanel").Find("HealthSlider").GetComponent<Slider>();
        healthSlider.maxValue = MAX_HEALTH;

        rigbod = GetComponent<Rigidbody>();

        currentHealth = MAX_HEALTH;
    }

    private void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }

        healthSlider.value = currentHealth;

        if (currentHealth <= 0 || Input.GetButton("Suicide"))
        {
            photonView.RPC("Die", PhotonTargets.All);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            // Network player, receive data
            currentHealth = (int) stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.isMine)
        {
            currentHealth -= damage;
        }
    }
    [PunRPC]
    private void Die()
    {
        currentHealth = MAX_HEALTH;
        transform.position = new Vector3(0, 0, -50);
        rigbod.velocity = Vector3.zero;
        deathCount++;
    }

    private int GetHealth()
    {
        return currentHealth;
    }
}