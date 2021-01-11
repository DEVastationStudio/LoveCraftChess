using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsDisplayer : MonoBehaviour
{
    public TMP_Text statsText;
    void Start()
    {
        statsText.text = "Local Multiplayer Stats:\n";
        statsText.text += "Deep One victories: " + PlayerPrefs.GetInt("P1Victories", 0) + "\n";//
        statsText.text += "Lloigor victories: " + PlayerPrefs.GetInt("P2Victories", 0) + "\n";//
        statsText.text += "\nOnline Multiplayer Stats:\n";
        statsText.text += "Games Played: " + PlayerPrefs.GetInt("OnlineGames", 0) + "\n";//
        statsText.text += "Victories: " + PlayerPrefs.GetInt("OnlineWins", 0) + "\n";//
        statsText.text += "Victories (Conqueror): " + PlayerPrefs.GetInt("OnlineWinsConqueror", 0) + "\n";//
        statsText.text += "Victories (Devastator): " + PlayerPrefs.GetInt("OnlineWinsDevastator", 0) + "\n";//
        statsText.text += "Victories (Surrender): " + PlayerPrefs.GetInt("OnlineWinsSurrender", 0) + "\n";//
        statsText.text += "Defeats: " + PlayerPrefs.GetInt("OnlineLosses", 0) + "\n";//
        statsText.text += "Defeats (Conqueror): " + PlayerPrefs.GetInt("OnlineLossesConqueror", 0) + "\n";//
        statsText.text += "Defeats (Devastator): " + PlayerPrefs.GetInt("OnlineLossesDevastator", 0) + "\n";//
        statsText.text += "Defeats (Surrender): " + PlayerPrefs.GetInt("OnlineLossesSurrender", 0) + "\n";//
        statsText.text += "Eaten pieces: " + PlayerPrefs.GetInt("EatenPieces", 0) + "\n";//
        statsText.text += "Resurrections: " + PlayerPrefs.GetInt("Resurrections", 0) + "\n";//
        statsText.text += "Traps triggered: " + PlayerPrefs.GetInt("TrapsTriggered", 0) + "\n";//
        statsText.text += "Pieces destroyed by traps: " + PlayerPrefs.GetInt("TrapKills", 0) + "\n";//
        statsText.text += "Turns taken: " + PlayerPrefs.GetInt("TurnsTaken", 0) + "\n";//
    }
}