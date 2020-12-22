using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    public Button ConnectBtn;
    public Button JoinRandomBtn;
    public Text Log;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    public int playerCounter;
    public Text PlayerCounter;

    private bool isConnecting;

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (isConnecting = PhotonNetwork.ConnectUsingSettings())
            {
                Log.text += "\nConnected to server.";
            }
            else
            {
                Log.text += "\nError connecting to server.";
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to region: " + PhotonNetwork.CloudRegion);
        ConnectBtn.interactable = false;
        JoinRandomBtn.interactable = true;
    }

    public void JoinRandom()
    {
        if (!PhotonNetwork.JoinRandomRoom())
        {
            Log.text += "\nFailed to join room.";
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.text += "\nThere are no rooms. Creating a new room...";
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() {MaxPlayers = maxPlayersInRoom}))
        {
            Log.text += "\nRoom created successfully.";
        }
        else
        {
            Log.text += "\nFailed to create room.";
        }
    }

    public override void OnJoinedRoom()
    {
        Log.text += "\nJoined room.";
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

    public void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
           playerCounter = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        PlayerCounter.text = playerCounter + "/" + maxPlayersInRoom;
    }
}
