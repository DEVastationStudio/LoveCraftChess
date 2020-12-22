using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerObject : MonoBehaviourPunCallbacks
{
    //public int playerId = 0;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            TableGenerator.localPlayer = 1;
        else
            TableGenerator.localPlayer = 2;
    }
}
