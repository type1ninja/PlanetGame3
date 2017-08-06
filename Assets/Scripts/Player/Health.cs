using UnityEngine;

public class Health : Photon.MonoBehaviour, IPunObservable {
    private int maxHealth = 150;
    private int currentHealth = 150;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        //TODO--display health slider
        if (currentHealth <= 0)
        {
            Debug.Log("YOU ARE DED, NOT PIG SOUP RICE");
        }

        if (Input.GetButtonDown("SecondaryFire"))
        {
            TakeDamage(100);
        }
        Debug.Log(currentHealth);
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
    private void SetHealth(int newHealth)
    {
        if (photonView.isMine)
        {
            currentHealth = newHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("SetHealth", PhotonTargets.All, currentHealth - damage);
    }

    private int GetHealth()
    {
        return currentHealth;
    }

}