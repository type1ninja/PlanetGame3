using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Tutorial: https://doc.photonengine.com/en-us/pun/current/tutorials/pun-basics-tutorial/lobby-ui
//Copied very closely, as such some comment formatting is different from what I usually do

/// <summary>
/// Player name input field. Let the user input their name, and it will appear above the player in the game. 
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour {

    #region Public Variables

    public bool resetPlayerName = false;

    #endregion

    #region Private Variables

    // Store the PlayerPref Key to avoid types
    static string playerNamePrefKey = "PlayerName";

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviouir method called on GameObject by Unity during initialization phase.
    /// </summary>
    private void Start()
    {
        //If resetPlayerName is true--and it should only be true for debugging--set the playerPrefs playerName to nothing
        if (resetPlayerName)
        {
            PlayerPrefs.SetString(playerNamePrefKey, "");
        }

        // We immediately set the text field and Photon playerName to the default playerName, which is stored with PlayerPrefs
        string defaultName = "Generic McGamer";

        InputField _inputField = GetComponent<InputField>();

        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
    }

    #endregion

    #region Public Methods

    public void SetPlayerName(string newName)
    {
        // #Important
        PhotonNetwork.playerName = newName + " "; // force a trailing space string in case value is an empty string, else playerName would not be updated.

        PlayerPrefs.SetString(playerNamePrefKey, newName);
    }

    #endregion 
}
