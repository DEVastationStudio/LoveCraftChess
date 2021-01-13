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
    public GameObject statusBox;
    public TMP_InputField customInputField;
    public GameObject customPanel;
    public Button createCustomBtn, joinCustomBtn, backCustomBtn;
    private bool _creatingCustomRoom;

    public GameObject connecting;
    public TextMeshProUGUI connectProgText;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    public CodeImage[] codeInUI = new CodeImage[5];
    private int[] code = new int[] { -1, -1, -1, -1, -1 };
    private int codeCount = 0;

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
        statusBox.SetActive(true);
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
        //customText.SetText("");
    }

    public void CreateCustom()
    {
        SetCustomButtons(false);
        _creatingCustomRoom = true;
        AttemptCustomRoomCreation();
    }
    private void AttemptCustomRoomCreation()
    {
        GenerateRandomCombination();
        string newCode = code[0].ToString() + "^" + code[1].ToString() +"^"+ code[2].ToString() + "^" + code[3].ToString() + "^" + code[4].ToString();
        Debug.Log("Attempting to create room " + newCode);
        RoomOptions options = new RoomOptions();
        options.IsVisible = false;
        PhotonNetwork.CreateRoom(newCode, options);
    }

    private void GenerateRandomCombination() 
    {
        for (int i = 0; i < 5; i++) 
        {
            code[i] = Random.Range(1, 6);
        }
    }
    public void ResetCode() 
    {
        for (int i = 0; i < codeInUI.Length; i++) 
        {
            for (int j = 0; j < codeInUI[i].possibleImages.Length; j++) 
            {
                codeInUI[i].ChangeImage(j.ToString(), false);
            }
            codeInUI[i].ChangeImage(0.ToString(), true);
        }
        for (int i = 0; i < code.Length; i++)
            code[i] = -1;
        codeCount = 0;
    }
    public void AddCode(int i) 
    {
        if (codeCount >= 5) return;
        codeInUI[codeCount].ChangeImage(0.ToString(), false);
        code[codeCount] = i;
        codeInUI[codeCount].ChangeImage(code[codeCount].ToString(),true);
        codeCount++;
    }
    public void DeleteCode()
    {
        if (codeCount <= 0) return;
        codeCount--;
        codeInUI[codeCount].ChangeImage(code[codeCount].ToString(), false);
        codeInUI[codeCount].ChangeImage(0.ToString(), true);
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
        if (codeCount<5)
        {
            //customText.SetText(LanguageManager.EnterRoomName());
            return;
        }
        SetCustomButtons(false);
        PhotonNetwork.JoinRoom(code[0].ToString() + "^" + code[1].ToString() + "^" + code[2].ToString() + "^" + code[3].ToString() + "^" + code[4].ToString());
    }

    public void SetCustomButtons(bool active)
    {
        //createCustomBtn.interactable = active;
        //joinCustomBtn.interactable = active;
        //backCustomBtn.interactable = active;
        //customInputField.interactable = active;
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
        //JoinRandomBtn.interactable = false;     
        //BackBtn.interactable = false;   
        //ShowCustomBtn.interactable = false;

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
        AudioManager.instance.changeTheme(0);
    }

}

