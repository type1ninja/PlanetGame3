using UnityEngine;

//A basic script that lets me test gravity by "kicking" objects when they spawn
public class Kick : MonoBehaviour {

    private void Start()
    {
        //Get the rigidbody
        Rigidbody rigbod = GetComponent<Rigidbody>();
        //Kick it
        rigbod.AddForce(0, 15, 0, ForceMode.Impulse);
    }
	
}