using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSrc;
    [SerializeField] [Range(0, 1)] private float vol;
    public AudioClip[] audioClips;
    
    [SerializeField] private Image soundOn;
    [SerializeField] private Image soundOff;
    private bool state = true;
    private int curSong = -1;
    public static AudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            audioSrc.clip = audioClips[0];
            audioSrc.Play();
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void changeTheme(int index)
    {
        if (index != curSong)
        {
            curSong = index;
            StartCoroutine(AudioFade(audioClips[index], 0.2f, audioSrc));
        }
    }
    static IEnumerator AudioFade(AudioClip newClip, float speed, AudioSource auSrc)
    {
        if (auSrc.clip != null)
        {
            while (AudioListener.volume > 0)
            {
                AudioListener.volume -= speed;
                yield return new WaitForSeconds(0.1f);
            }
            auSrc.Stop();
        }
        auSrc.clip = newClip;
        if (newClip == null) yield break;
        auSrc.Play();
        while (AudioListener.volume < 1)
        {
            AudioListener.volume += speed;
            yield return new WaitForSeconds(0.1f);
        }
        AudioListener.volume = 1;
    }
}
