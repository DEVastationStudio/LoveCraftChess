using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public bool isLobby;
    
    public void Start()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
    }

    public override void OnLeftRoom()
    {
        if (isLobby)
        {
            SceneManager.LoadScene("PreLobby");
        }
    }
    public void QuitGameButton()
    {
        SceneManager.LoadScene("PreLobby");
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("PreLobby");
        }
        SceneManager.LoadScene("Menu");
    }
    void LoadGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to Load a level but we are not the master Client");
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        else
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && isLobby)
        {
            LoadGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!isLobby)
        {
            TableGenerator.instance.AbandonVictory();
        }
    }
}
