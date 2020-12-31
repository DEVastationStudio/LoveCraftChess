using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreLobby : MonoBehaviourPunCallbacks
{
    public Button JoinRandomBtn;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;


    public void JoinRandom()
    {
        if (!PhotonNetwork.JoinRandomRoom())
        {
            Debug.Log("Failed to join room.");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("There are no rooms. Creating a new room...");
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayersInRoom }))
        {
            Debug.Log("Room created successfully.");
        }
        else
        {
            Debug.Log("Failed to create room.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
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

