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
        if (isPaused)
        {
            isPaused = false;
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            isPaused = true;
            pauseMenuPanel.SetActive(true);
        }
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        pauseMenuPanel = GameObject.Find("Canvas").transform.Find("PauseMenuPanel").gameObject;
        pauseMenuPanel.SetActive(false);
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