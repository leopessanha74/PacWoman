using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager SMinstance = null;
    private AudioSource pacManAS;
    private AudioSource ghostAS;
    private AudioSource oneShotAS;
    public AudioClip eatingDots;
    public AudioClip eatingGhost;
    public AudioClip ghostMove;
    public AudioClip pacManDies;
    public AudioClip powerUp;
    // Start is called before the first frame update
    void Start()
    {
        if (SMinstance == null)
        {
            SMinstance = this;
        } else if(SMinstance != this)
        {
            Destroy(this.gameObject);
        }
        AudioSource[] audioSorces = this.gameObject.GetComponents<AudioSource>();
        pacManAS = audioSorces[0];
        ghostAS = audioSorces[1];
        oneShotAS = audioSorces[2];

        PlayClipOnLoop(pacManAS,eatingDots);
    }

    public void PlayClipOnLoop(AudioSource _source, AudioClip _clip)
    {
        if(_source != null && _clip != null)
        {
            _source.loop = true;
            _source.clip = _clip;
            _source.volume = 0.01f;
            _source.Play();
        }
    }
    public void PlayOneShot(AudioClip _clip)
    {
        oneShotAS.PlayOneShot(_clip);
    }
}
