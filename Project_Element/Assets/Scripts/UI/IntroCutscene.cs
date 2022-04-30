/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              IntroCutscene
Description:        Handles games intro cutscene
Date Created:       02/04/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/04/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class IntroCutscene : MonoBehaviour
{
    private static IntroCutscene m_Instance = null;
    public static IntroCutscene Instance { get { return m_Instance; } }

    public UnityEngine.Video.VideoPlayer VideoPlayer;

    public AudioMixer Mixer;

    public Slider HoldSlider;
    public GameObject SliderObject;
    public RawImage CutsceneImage;

    public bool HasBeenPlayed = false;

    public float HoldTime = 0.0f;

    float m_HoldTimer = 0.0f;
    float m_StartVolume = 0.0f;
    bool m_IsVideoReady = false;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        VideoPlayer.Prepare();
    }

    private void Start()
    {
        HoldSlider.maxValue = HoldTime;

        if (HasBeenPlayed)
        {
            gameObject.SetActive(false);
        }
        else
        {
            VideoPlayer.loopPointReached += EndReached;
            VideoPlayer.prepareCompleted += StartVideo;
        }
    }

    private void Update()
    {
        if (HasBeenPlayed)
        {
            CloseIntroVideo();
        }

        if (m_IsVideoReady)
        {
            if (Input.anyKey)
            {
                SliderObject.SetActive(true);
                HoldSlider.value = m_HoldTimer;
                m_HoldTimer += 1 * Time.deltaTime;
            }
            else
            {
                m_HoldTimer = 0.0f;
                SliderObject.SetActive(false);
            }

            if (m_HoldTimer > HoldTime)
            {
                CloseIntroVideo();
            }
        }
    }

    void CloseIntroVideo()
    {
        VideoPlayer.frame = 0;
        VideoPlayer.Stop();

        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = false;
        Time.timeScale = 1;
        PlayerManager.Instance.Player.m_IsMenuOpen = false;

        HasBeenPlayed = true;

        Mixer.SetFloat("MasterVolume", m_StartVolume);

        gameObject.SetActive(false);
    }

    void EndReached(UnityEngine.Video.VideoPlayer videoPlayer)
    {

        CloseIntroVideo();
    }

    void StartVideo(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        CutsceneImage.enabled = true;
        m_IsVideoReady = true;

        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = true;
        PlayerManager.Instance.Player.m_IsMenuOpen = true;

        Mixer.GetFloat("MasterVolume", out m_StartVolume);
        Mixer.SetFloat("MasterVolume", -100.0f);
    }
}
