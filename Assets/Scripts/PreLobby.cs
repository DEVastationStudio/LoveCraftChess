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
    public Button JoinRandomBtn, BackBtn, ShowCustomBtn;
    public TMP_Text connectedToText, statusText, customText;
    public TMP_InputField customInputField;
    public GameObject customPanel;
    public Button createCustomBtn, joinCustomBtn, backCustomBtn;
    private bool _creatingCustomRoom;

    public GameObject connecting;
    public Text connectProgText;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    private void Start()
    {
        string remainder = "/*";
        connectedToText.text = LanguageManager.ConnectedToRegion(PhotonNetwork.CloudRegion.Replace(remainder, ""));
    }
    public void JoinRandom()
    {
        JoinRandomBtn.interactable = false;
        BackBtn.interactable = false;
        ShowCustomBtn.interactable = false;
            statusText.text = LanguageManager.Connecting();
        if (!PhotonNetwork.JoinRandomRoom())
        {
            statusText.text = LanguageManager.JoinRoomFail();
            JoinRandomBtn.interactable = true;
            BackBtn.interactable = true;
            ShowCustomBtn.interactable = true;
        }
    }

    public void ShowCustomPanel()
    {
        customPanel.SetActive(true);
        SetCustomButtons(true);
        customText.SetText("");
    }

    public void CreateCustom()
    {
        if (customInputField.text == "") {
            customText.SetText(LanguageManager.EnterRoomName());
            return;
        }
        SetCustomButtons(false);
        _creatingCustomRoom = true;
        AttemptCustomRoomCreation();
    }
    private void AttemptCustomRoomCreation()
    {
        int randomCode = Random.Range(0,10000);
        string newCode = customInputField.text + "-" + string.Format("{0:D2}", randomCode);
        Debug.Log("Attempting to create room " + newCode);
        RoomOptions options = new RoomOptions();
        options.IsVisible = false;
        PhotonNetwork.CreateRoom(newCode, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameIdAlreadyExists && _creatingCustomRoom)
        {
            Debug.Log("Room already existed. Trying again...");
            AttemptCustomRoomCreation();
        }
        else
        {
            Debug.Log("Room couldn't be created");
            SetCustomButtons(true);
            _creatingCustomRoom = false;
        }
    }

    public void JoinCustom()
    {
        if (customInputField.text == "") {
            customText.SetText(LanguageManager.EnterRoomName());
            return;
        }
        SetCustomButtons(false);
        PhotonNetwork.JoinRoom(customInputField.text);
    }

    public void SetCustomButtons(bool active)
    {
        createCustomBtn.interactable = active;
        joinCustomBtn.interactable = active;
        backCustomBtn.interactable = active;
        customInputField.interactable = active;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = LanguageManager.NoNewRooms();
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayersInRoom }))
        {
            statusText.text = (LanguageManager.RoomCreateSuccess());
        }
        else
        {
            statusText.text = (LanguageManager.RoomCreateFailure());
            JoinRandomBtn.interactable = true;
            BackBtn.interactable = true;
            ShowCustomBtn.interactable = true;
        }
    }

    public override void OnJoinedRoom()
    {
        statusText.text = (LanguageManager.JoinedRoom());
        JoinRandomBtn.interactable = false;     
        BackBtn.interactable = false;   
        ShowCustomBtn.interactable = false;

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

    public void QuitOnline()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Menu");
    }

}

