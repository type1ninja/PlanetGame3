using UnityEngine;

//A basic script that lets me test gravity by "kicking" objects when they spawn
public class Kick : MonoBehaviour {

    public Vector3 kick = new Vector3(0, 15, 0);
    private Rigidbody rigbod;

    private void Start()
    {
        //Get the rigidbody
        rigbod = GetComponent<Rigidbody>();
        KickIt();
    }

    public void KickIt()
    {
        //Kick it
        rigbod.AddForce(kick, ForceMode.Impulse);
    }
}