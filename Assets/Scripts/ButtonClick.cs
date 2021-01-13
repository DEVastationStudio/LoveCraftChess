using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public AudioSource click;

    public void PlayClick()
    {
        click.Play();
    }
}
