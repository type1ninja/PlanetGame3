using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//Tutorial: https://doc.photonengine.com/en-us/pun/current/tutorials/pun-basics-tutorial/game-scenes
//Copied somewhat closely, as such some comment formatting is different from what I usually do

public class GameManager : Photon.PunBehaviour {

    #region Private Variables

    private GameObject pauseMenuPanel;

    private bool isPaused = false;

    #endregion

    #region Public Variables

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    #endregion 

    #region Photon Messages

    /// <summary>
    /// Called when the local player left the roomm. We need to load the launcher scene.
    /// </summary> 
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    public void TogglePaused()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        pauseMenuPanel = GameObject.Find("Canvas").transform.Find("PauseMenuPanel").gameObject;
        pauseMenuPanel.SetActive(false);

        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.Log("We are Instantiating LocalPlayer");
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, -50f), Quaternion.identity, 0);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePaused();
        }
    }

    #endregion
}