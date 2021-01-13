using System;
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
    public GameObject connecting;
    public TMP_Text connectProgText;
    public GameObject Quit, Surrender, QuitBtn, SurrenderBtn;
    public CodeImage[] codeInUI = new CodeImage[5];

    public void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0,0,0), Quaternion.identity, 0);
            if (isLobby && codeText != null)
            {
                string[] code = PhotonNetwork.CurrentRoom.Name.Split('-');
                for (int i = 0; i < code.Length; i++) 
                {
                    Debug.Log("Piece "+i+": "+code[i]);
                    codeInUI[i].ChangeImage(code[i], true);
                }
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
            StartCoroutine(LoadLevelCR("Lobby"));
        }
        else
        {
            StartCoroutine(LoadLevelCR("Game"));
        }
    }
    public IEnumerator LoadLevelCR(string level)
    {
        connecting.SetActive(true);
        PhotonNetwork.LoadLevel(level);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            connectProgText.text = LanguageManager.LoadingRoom() + (PhotonNetwork.LevelLoadingProgress*100) + "%";
            yield return new WaitForEndOfFrame();
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
