/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              AudioZone
Description:        A trigger box that plays music or ambient sound effects
Date Created:       19/03/21
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/03/22
        - [Zoe] Created AudioZone.
    20/03/22
        - [Zoe] Added functionality for combat music
        - [Zoe] Added priority system for when you're colliding with more than 1 audio trigger

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Music,
    Combat,
    Ambience
}

public class AudioZone : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_Audio;
    public AudioClip Audio { get { return m_Audio; } }

    [SerializeField]
    private AudioType m_AudioType;
    public AudioType AudioType { get { return m_AudioType; } }

    [SerializeField]
    private int m_Priority;
    public int Priority { get { return m_Priority; } }
}
