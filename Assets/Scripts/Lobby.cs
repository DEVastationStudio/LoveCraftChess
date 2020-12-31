using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    public Button ConnectBtn;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    //public int playerCounter;
    //public Text PlayerCounter;

    private bool isConnecting;

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (isConnecting = PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("Connected to server.");
            }
            else
            {
                Debug.Log("Error connecting to server.");
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to region: " + PhotonNetwork.CloudRegion);
        ConnectBtn.interactable = false;
        SceneManager.LoadScene("PreLobby");
    }

    public void JoinLocalGame()
    {
        SceneManager.LoadScene("Game");
    }

    /*public void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
           playerCounter = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        PlayerCounter.text = playerCounter + "/" + maxPlayersInRoom;
    }*/
}
