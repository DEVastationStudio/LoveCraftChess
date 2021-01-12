using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public TMP_Text codeText;
    public bool isLobby;
    
    public GameObject Quit, Surrender, QuitBtn, SurrenderBtn;

    public void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
            if (isLobby && codeText != null)
            {
                if (PhotonNetwork.CurrentRoom.Name.Length < 20)
                    codeText.text = LanguageManager.RoomCode() + PhotonNetwork.CurrentRoom.Name;
                else
                    codeText.text = "";
            }
            if (SurrenderBtn != null) SurrenderBtn.SetActive(true);
        }
        else
        {
            if (QuitBtn != null) QuitBtn.SetActive(true);
        }
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
        //SceneManager.LoadScene("PreLobby");
        if (TableGenerator.instance != null && TableGenerator.instance.isOnline)
        {
            SceneManager.LoadScene("PreLobby");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("PreLobby");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
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

    public void QuitOrSurrender()
    {
        if (PhotonNetwork.IsConnected)
        {
            Surrender.SetActive(true);
        }
        else
        {
            Quit.SetActive(true);
        }
    }
}
