using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Utilities;

public class Lobby : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    public Button ConnectBtn, MainBtn, OfflineBtn;
    public TMP_Dropdown serverDropdown;
    public string[] regions;

    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;

    //public int playerCounter;
    //public Text PlayerCounter;

    private bool isConnecting;


    void Start()
    {
        regions = new string[14];
        regions[0] = "";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Best region (auto)"));
        regions[1] = "asia";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Asia"));
        regions[2] = "au";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Australia"));
        regions[3] = "cae";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Canada, East"));
        regions[4] = "eu";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Europe"));
        regions[5] = "in";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("India"));
        regions[6] = "jp";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Japan"));
        regions[7] = "ru";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Russia"));
        regions[8] = "rue";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("Russia, East"));
        regions[9] = "za";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("South Africa"));
        regions[10] = "sa";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("South America"));
        regions[11] = "kr";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("South Korea"));
        regions[12] = "us";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("USA, East"));
        regions[13] = "usw";
        serverDropdown.options.Add(new TMP_Dropdown.OptionData("USA, West"));

        serverDropdown.value = PlayerPrefs.GetInt("preferredRegion", 0);
    }

    public void Connect()
    {
        PlayerPrefs.SetInt("preferredRegion",serverDropdown.value);
        ConnectBtn.interactable = false;
        MainBtn.interactable = false;
        OfflineBtn.interactable = false;
        serverDropdown.interactable = false;
        if (!PhotonNetwork.IsConnected)
        {
            AppSettings settings = PhotonNetwork.PhotonServerSettings.AppSettings;
            settings.FixedRegion = regions[serverDropdown.value];
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("Connected to server.");
            }
            else
            {
                Debug.Log("Error connecting to server.");
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ConnectBtn.interactable = true;
        MainBtn.interactable = true;
        OfflineBtn.interactable = true;
        serverDropdown.interactable = true;
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
