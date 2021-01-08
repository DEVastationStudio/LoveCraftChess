using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PreLobby : MonoBehaviourPunCallbacks
{
    public Button JoinRandomBtn, BackBtn;
    public TMP_Text connectedToText, statusText;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    private void Start()
    {
        string remainder = "/*";
        connectedToText.text = "Connected to "+ PhotonNetwork.CloudRegion.Replace(remainder, "") + " server";
    }
    public void JoinRandom()
    {
        JoinRandomBtn.interactable = false;
        BackBtn.interactable = false;
            statusText.text = "Connecting...";
        if (!PhotonNetwork.JoinRandomRoom())
        {
            statusText.text = "Failed to join room.";
            JoinRandomBtn.interactable = true;
            BackBtn.interactable = true;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = "There are no rooms. Creating a new room...";
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayersInRoom }))
        {
            statusText.text = ("Room created successfully.");
        }
        else
        {
            statusText.text = ("Failed to create room.");
            JoinRandomBtn.interactable = true;
            BackBtn.interactable = true;
        }
    }

    public override void OnJoinedRoom()
    {
        statusText.text = ("Joined room.");
        JoinRandomBtn.interactable = false;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        else
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public void QuitOnline()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Menu");
    }

}

