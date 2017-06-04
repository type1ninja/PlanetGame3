﻿using UnityEngine;

//This script allows players to look around in a 3D environment
//https://keithmaggio.wordpress.com/2011/07/01/unity-3d-code-snippet-flight-script/
//This script is adapted from the above link ^^^
public class SpaceMouselook : MonoBehaviour {

    private static float ROLL_SPEED_CONSTANT = 7.0f;

    //Sensitivities for turning
    public float xsens = 2.0f;
    public float ysens = 2.0f;
    public float zsens = 2.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Quaternion AddRot = Quaternion.identity;
        float yaw = Input.GetAxis("Mouse X") * xsens;
        float pitch = Input.GetAxis("Mouse Y") * ysens;
        float roll = Input.GetAxis("ViewRot") * zsens * ROLL_SPEED_CONSTANT;

        AddRot.eulerAngles = new Vector3(-pitch, yaw, roll);
        transform.rotation *= AddRot;
    }
}