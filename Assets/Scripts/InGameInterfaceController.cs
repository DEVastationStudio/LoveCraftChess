using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InGameInterfaceController : MonoBehaviourPunCallbacks
{
    [Header("Universal variables")]
    [SerializeField] private TableGenerator TB;
    [SerializeField] private TableObj[] AvailableMapTableObj;

    [Header("Local game")]
    [SerializeField] private Image MapImage;
    [SerializeField] private GameObject localInterface;
    private int currentMap = 0;


    [Header("Random Multiplayer game")]

    [SerializeField] private GameObject onlineInterface;
    [SerializeField] private Image mapImageP1;
    [SerializeField] private Image mapImageP2;
    [SerializeField] private GameObject multiInterfaceP1;
    [SerializeField] private GameObject multiInterfaceP2;
    private int currentMapP1 = 0;
    private int currentMapP2 = 0;
    private bool isPlayer1Ready = false;
    private bool isPlayer2Ready = false;
    private int[] playerMapSelection = new int[2];

    #region MultiRandom Region

    public void OnlineInterface() 
    {
        mapImageP1.sprite = AvailableMapTableObj[currentMap].preview;
        mapImageP2.sprite = AvailableMapTableObj[currentMap].preview;
        onlineInterface.SetActive(true); 
        if (PhotonNetwork.IsMasterClient)
            multiInterfaceP1.SetActive(true); 
        else
            multiInterfaceP2.SetActive(true); 
    }

    public void LeftMapButton(int player)
    {
        photonView.RPC("LeftMapRPC", RpcTarget.All, player);
    }
    public void RightMapButton(int player)
    {
        photonView.RPC("RightMapRPC", RpcTarget.All, player);
    }
    public void PlayerReadyButton(int player)
    {
        photonView.RPC("PlayerReadyRPC", RpcTarget.All, player, Random.Range(0, 1));
    }


    [PunRPC]
    public void LeftMapRPC(int player)
    {
        if (player == 1)
        {
            if (currentMapP1 - 1 < 0)
                currentMapP1 = AvailableMapTableObj.Length - 1;
            else
                currentMapP1--;
            mapImageP1.sprite = AvailableMapTableObj[currentMapP1].preview;
        }
        else
        {
            if (currentMapP2 - 1 < 0)
                currentMapP2 = AvailableMapTableObj.Length - 1;
            else
                currentMapP2--;
            mapImageP2.sprite = AvailableMapTableObj[currentMapP2].preview;
        }
    }

    [PunRPC]
    public void RightMapRPC(int player)
    {
        if (player == 1)
        {
            if (currentMapP1 + 1 >= AvailableMapTableObj.Length)
                currentMapP1 = 0;
            else
                currentMapP1++;
            mapImageP1.sprite = AvailableMapTableObj[currentMapP1].preview;
        }
        if(player == 2)
        {
            if (currentMapP2 + 1 >= AvailableMapTableObj.Length)
                currentMapP2 = 0;
            else
                currentMapP2++;
            mapImageP2.sprite = AvailableMapTableObj[currentMapP2].preview;
        }

    }

    [PunRPC]
    public void PlayerReadyRPC(int readyPlayer, int map) 
    {
        if (readyPlayer == 1)
        {
            if (isPlayer1Ready) return;
            playerMapSelection[0] = currentMapP1;
            isPlayer1Ready = true;
        }
        else if (readyPlayer == 2)
        {
            if (isPlayer2Ready) return;
            playerMapSelection[1] = currentMapP2;
            isPlayer2Ready = true;
        }
        if (isPlayer1Ready && isPlayer2Ready) {
            TB.GenerateTable(AvailableMapTableObj[playerMapSelection[map]]);
            onlineInterface.SetActive(false); 
        }
    }
    /*
     * Llamada: photonView.RPC("ButtonNextTurnRPC", RpcTarget.All, Los parametros que sean necesarios);
     * 
     [PunRPC]
    public void ButtonNextTurnRPC(int readyPlayer)
    {
        if (initialTurn)
        {
            if (readyPlayer == 1) 
            {
                if (p1Ready) return;
                p1Ready = true;
            }
            else if (readyPlayer == 2) 
            {
                if (p2Ready) return;
                p2Ready = true;
            }
            
            if (readyPlayer == localPlayer)
            {
                curPiece?.SetChosen(false);
                ResetClickables();
                confirmButton.interactable = false;
            }


            if (p1Ready && p2Ready) NextTurn();
        }
    }
     */

    public void changeMapMulti() 
    {
        TB.GenerateTable(AvailableMapTableObj[0]);
    }

    #endregion

    #region Local Region
    public void LocalInterface() 
    {
        MapImage.sprite = AvailableMapTableObj[currentMap].preview;
        localInterface.SetActive(true); 
    }
    public void LeftMap() 
    {
        if (currentMap - 1 < 0)
            currentMap = AvailableMapTableObj.Length - 1;
        else
            currentMap--;
        ChangeMap();
    }
    public void RightMap()
    {
        if (currentMap + 1 >= AvailableMapTableObj.Length)
            currentMap = 0;
        else
            currentMap++;
        ChangeMap();
    }
    public void ChangeMap()
    {
        MapImage.sprite = AvailableMapTableObj[currentMap].preview;
    }

    public void GenerateMap() 
    {
        TB.GenerateTable(AvailableMapTableObj[currentMap]);
    }
    #endregion
}
