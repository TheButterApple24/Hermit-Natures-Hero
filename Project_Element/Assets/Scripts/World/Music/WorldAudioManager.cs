/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              AudioZone
Description:        Handles the logic for triggering music and ambient sound effects represented my triggers (AudioZones) throughout the world
Date Created:       19/03/21
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/03/22
        - [Zoe] Created WorldAudioManager.
    20/03/22
        - [Zoe] Added functionality for combat music
        - [Zoe] Added priority system for when you're colliding with more than 1 audio trigger
        - [Zoe] Commented code

 ===================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAudioManager : MonoBehaviour
{
    private static WorldAudioManager m_Instance = null;
    public static WorldAudioManager Instance { get { return m_Instance; } }

    private AudioZone m_CurrentSong = null;
    public AudioZone CurrentSong { get { return m_CurrentSong; } set { m_CurrentSong = value; } }

    private AudioZone m_NextSong = null;
    public AudioZone NextSong { get { return m_NextSong; } set { m_NextSong = value; } }

    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioZone m_DefaultMusic;
    [SerializeField] private List<AudioZone> m_CombatMusic;

    private float m_TimeInTrigger = 0.0f;
    private float m_TimeOutOfTrigger = 0.0f;
    private List<AudioZone> m_FoundMusic;

    private void Start()
    {
        m_FoundMusic = new List<AudioZone>();
    }

    void Update()
    {
        // If the player is in combat, play a random combat music
        if (PlayerManager.Instance.Player.m_IsInCombat)
        {
            SetSong(m_CombatMusic[Random.Range(0, m_CombatMusic.Count)]);
        }
        else
        {
            // Grab all the colliders near the player
            Collider[] hitcolliders = Physics.OverlapSphere(PlayerManager.Instance.Player.transform.position, 1.0f);

            // Clear the list of found music
            m_FoundMusic.Clear();

            foreach (Collider collider in hitcolliders)
            {
                // If the collider has a parent object
                if (collider.gameObject.transform.parent != null)
                {
                    // If the parent has an audio zone component
                    if (collider.gameObject.transform.parent.TryGetComponent(out AudioZone musicZone))
                    {
                        // If the trigger is a music trigger, add it to the found music list
                        if (musicZone.AudioType == AudioType.Music)
                        {
                            m_FoundMusic.Add(musicZone);
                        }

                    }
                }
            }

            // If some music was found
            if (m_FoundMusic.Count > 0)
            {
                // Find the audio in the list that has the highest priority, and trigger it
                AudioZone audio = FindHighestPriorityAudio(m_FoundMusic);
                TriggerMusicZone(audio);
            }
            // If no music triggers were found, play the default exploration music.
            else
            {
                // After a small amount of buffer time has passed, play the exploration music
                // This ensures that you aren't constantly triggering multiple music triggers at the same time
                m_TimeInTrigger = 0.0f;
                m_TimeOutOfTrigger += Time.unscaledDeltaTime;

                if (m_TimeOutOfTrigger >= 0.75f)
                {
                    SetSong(m_DefaultMusic);
                    m_TimeOutOfTrigger = 0.0f;
                }
            }
        }
    }

    void TriggerMusicZone(AudioZone musicZone)
    {
        m_TimeOutOfTrigger = 0.0f;

        // Checking if the triggered audio isn't already playing
        if (musicZone != m_CurrentSong)
        {
            // After a small amount of buffer time has passed, play the selected music
            // This ensures that you aren't constantly triggering multiple music triggers at the same time

            m_TimeInTrigger += Time.unscaledDeltaTime;

            if (m_TimeInTrigger >= 0.75f)
            {
                SetSong(musicZone);
                m_TimeInTrigger = 0.0f;
            }
        }
    }
    void SetSong(AudioZone audioTrigger)
    {
        if (m_CurrentSong != null)
        {
            // If both the currently playing music, and the triggered music are combat music, return
            // This ensures that, when randomly generating the combat music, it isn't constantly being reset
            if (m_CurrentSong.AudioType == AudioType.Combat && audioTrigger.AudioType == AudioType.Combat)
                return;
        }

        // If the triggered audio isn't already the current song
        if (audioTrigger.Audio != m_CurrentSong)
        {
            m_CurrentSong = audioTrigger;

            // If the audio source is currently not playing the song
            if (m_AudioSource.clip != m_CurrentSong.Audio)
            {
                // If there is already music playing, transition between the songs
                if (m_AudioSource.isPlaying)
                {
                    StartCoroutine(TransitionSong());
                }
                // If there is no existing music, simply fade in the new song
                else
                {
                    m_AudioSource.clip = m_CurrentSong.Audio;
                    StartCoroutine(FadeIn());
                }
            }
        }
    }
    public IEnumerator FadeIn()
    {
        m_AudioSource.Play();
        float currentTime = 0;

        while (m_AudioSource.volume < 1)
        {
            currentTime += 2 * Time.unscaledDeltaTime;
            m_AudioSource.volume = Mathf.Lerp(1.0f, 0.0f, currentTime);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float currentTime = 0;

        while (m_AudioSource.volume > 0)
        {
            currentTime += 2 * Time.unscaledDeltaTime;
            m_AudioSource.volume = Mathf.Lerp(0.0f, 1.0f, currentTime);
            yield return null;
        }
        m_AudioSource.Stop();
    }

    // Fades out the old song, replaces it with the new song, and fades in the new song
    // Yes, I have tried just calling both FadeIn and FadeOut instead. Coroutines are mean and won't let it work. >:(
    public IEnumerator TransitionSong()
    {
        float currentTime = 0;

        while (m_AudioSource.volume > 0 && m_AudioSource.clip != m_CurrentSong.Audio)
        {
            currentTime += 2 * Time.unscaledDeltaTime;
            m_AudioSource.volume = Mathf.Clamp01(Mathf.Lerp(1.0f, 0.0f, currentTime));
            yield return null;
        }
        m_AudioSource.Stop();
        m_AudioSource.clip = m_CurrentSong.Audio;
        m_AudioSource.Play();
        currentTime = 0;

        while (m_AudioSource.volume < 1 && m_AudioSource.clip == m_CurrentSong.Audio)
        {
            currentTime += 2 * Time.unscaledDeltaTime;
            m_AudioSource.volume = Mathf.Clamp01(Mathf.Lerp(0.0f, 1.0f, currentTime));
            yield return null;
        }
    }

    private AudioZone FindHighestPriorityAudio(List<AudioZone> foundAudio)
    {
        // Start at index 0
        int musicIndex = 0;

        for (int i = 0; i < foundAudio.Count; i++)
        {
            // If the current audio is a higher priority than the current-highest index, update said index
            if (foundAudio[i].Priority > foundAudio[musicIndex].Priority)
            {
                musicIndex = i;
            }
        }
        
        // When the loop is finished we've found the highest priority audio
        // If the audio in the list had the same priority, it will just return the 0th element
        return foundAudio[musicIndex];
    }
}
