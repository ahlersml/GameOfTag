using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionScript : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private Button _playButton;



    // Start is called before the first frame update
    void Start()
    {
        // Clear out the status
        SetStatus("");

        SetInputsEnabled(false);

        //Restore the player name
        _nameInput.text = PlayerPrefs.GetString("playername");

        // configure photon
        PhotonNetwork.AutomaticallySyncScene = true;

        //connection to the photon master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SetStatus("Connected to Master");
        SetInputsEnabled(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetStatus($"Disconnected: {cause}");
    }

    public void OnClick_Play()
    {
        //Make sure the player has entered a name
        if (string.IsNullOrEmpty(_nameInput.text))
        {
            SetStatus("Please enter a name");
            return;
        }

        // Set the players name
        PhotonNetwork.NickName = _nameInput.text;

        //Store my name for the next run
        PlayerPrefs.SetString("playername", _nameInput.text);

        //Attempt to joina  random room
        PhotonNetwork.JoinRandomRoom();

    }

    private void SetStatus(string message)
    {
        _statusText.text = message;
    }

    private void SetInputsEnabled(bool enabled)
    {
        _nameInput.interactable = enabled;
        _playButton.interactable = enabled;
    }

    //~~~~~~~~JOIN RANDOM~~~~~~~~~~
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //There is no game to join
        //Start new game
        PhotonNetwork.CreateRoom("GameOfTag", new RoomOptions
        {
            MaxPlayers = 20
        }) ;
    }

    public override void OnJoinedRoom()
    {
        //If we are the host
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
