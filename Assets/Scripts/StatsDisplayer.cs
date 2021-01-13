using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsDisplayer : MonoBehaviour
{
    public TMP_Text statsText;
    private bool _spanish;
    void Update()
    {
        if (_spanish != LanguageManager.isSpanish)
        {
            _spanish = LanguageManager.isSpanish;
        }
        else
        {
            return;
        }

        statsText.text =  LanguageManager.Stats_1();
        statsText.text += LanguageManager.Stats_2() + PlayerPrefs.GetInt("P1Victories", 0) + "\n";//
        statsText.text += LanguageManager.Stats_3() + PlayerPrefs.GetInt("P2Victories", 0) + "\n";//
        statsText.text += LanguageManager.Stats_4();
        statsText.text += LanguageManager.Stats_5() + PlayerPrefs.GetInt("OnlineGames", 0) + "\n";//
        statsText.text += LanguageManager.Stats_6() + PlayerPrefs.GetInt("OnlineWins", 0) + "\n";//
        statsText.text += LanguageManager.Stats_7() + PlayerPrefs.GetInt("OnlineWinsConqueror", 0) + "\n";//
        statsText.text += LanguageManager.Stats_8() + PlayerPrefs.GetInt("OnlineWinsDevastator", 0) + "\n";//
        statsText.text += LanguageManager.Stats_9() + PlayerPrefs.GetInt("OnlineWinsSurrender", 0) + "\n";//
        statsText.text += LanguageManager.Stats_10() + PlayerPrefs.GetInt("OnlineLosses", 0) + "\n";//
        statsText.text += LanguageManager.Stats_11() + PlayerPrefs.GetInt("OnlineLossesConqueror", 0) + "\n";//
        statsText.text += LanguageManager.Stats_12() + PlayerPrefs.GetInt("OnlineLossesDevastator", 0) + "\n";//
        statsText.text += LanguageManager.Stats_13() + PlayerPrefs.GetInt("OnlineLossesSurrender", 0) + "\n";//
        statsText.text += LanguageManager.Stats_14() + PlayerPrefs.GetInt("EatenPieces", 0) + "\n";//
        statsText.text += LanguageManager.Stats_15() + PlayerPrefs.GetInt("Resurrections", 0) + "\n";//
        statsText.text += LanguageManager.Stats_16() + PlayerPrefs.GetInt("TrapsTriggered", 0) + "\n";//
        statsText.text += LanguageManager.Stats_17() + PlayerPrefs.GetInt("TrapKills", 0) + "\n";//
        statsText.text += LanguageManager.Stats_18() + PlayerPrefs.GetInt("TurnsTaken", 0) + "\n";//
    }
}