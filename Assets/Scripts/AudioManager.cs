﻿using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        gameObject.AddComponent<AudioListener>();
        Play("Theme");
    }
    /* void start()
    {
        Play("Theme");
    } */
    public void Play(string name)
    {
        try
        {
            Sound s  = Array.Find(sounds, sound => sound.name == name);
            s.source.Play();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
        
    }

}