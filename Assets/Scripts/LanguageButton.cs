using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    public bool spanish;
    public Button other;
    
    void Awake()
    {
        GetComponent<Button>().interactable = (LanguageManager.isSpanish != spanish);
    }

    public void Set()
    {
        LanguageManager.isSpanish = spanish;
        GetComponent<Button>().interactable = (LanguageManager.isSpanish != spanish);
        other.interactable = (LanguageManager.isSpanish == spanish);
    }
}
