using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MumbleSoundManager : MonoBehaviour
{
    private static MumbleSoundManager m_Instance = null;
    public static MumbleSoundManager Instance { get { return m_Instance; } }

    public AudioClip[] NeutralHighMumbleSounds;
    public AudioClip[] NeutralMedMumbleSounds;
    public AudioClip[] NeutralLowMumbleSounds;
    public AudioClip[] SadHighMumbleSounds;
    public AudioClip[] SadMedMumbleSounds;
    public AudioClip[] SadLowMumbleSounds;
    public AudioClip[] AngerHighMumbleSounds;
    public AudioClip[] AngerMedMumbleSounds;
    public AudioClip[] AngerLowMumbleSounds;
    public AudioClip[] FearHighMumbleSounds;
    public AudioClip[] FearMedMumbleSounds;
    public AudioClip[] FearLowMumbleSounds;
    public AudioClip[] JoyHighMumbleSounds;
    public AudioClip[] JoyMedMumbleSounds;
    public AudioClip[] JoyLowMumbleSounds;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }
}
