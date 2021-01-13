using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeImage : MonoBehaviour
{
    public GameObject[] possibleImages = new GameObject[6];

    public void ChangeImage(string i, bool act) 
    {
        Debug.Log("Se llama a "+i);
        switch (i) 
        {
            case "0":
                possibleImages[0].SetActive(act);
                break;
            case "1":
                possibleImages[1].SetActive(act);
                break;
            case "2":
                possibleImages[2].SetActive(act);
                break;
            case "3":
                possibleImages[3].SetActive(act);
                break;
            case "4":
                possibleImages[4].SetActive(act);
                break;
            case "5":
                possibleImages[5].SetActive(act);
                break;
        }
    }

}
