using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    public AudioSource aSource;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    public void StartPlaying()
    {
        aSource.Play();
    }

    public void StopPlaying()
    {
        aSource.Stop();
    }
}