/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              LowHealthIndicator
Description:        Handles the vignette that fades in and out when the Player is low on health
Date Created:       18/02/2021
Author:             Zoe Purcell
Verified by:        Jeffrey MacNab [13/04/2022]

Changelog:

    18/02/2022
        - [Zoe] Started LowHealthIndicator
    19/02/2022
        - [Zoe] Added Heartbeat sound effect
        - [Zoe] Fixed pulse visual effect

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowHealthIndicator : MonoBehaviour
{
    public HealthComponent m_HealthComp;
    public float m_LowHealthThreshold = 0.33f;
    public AudioSource m_Heartbeat;

    Image m_IndicatorImage;
    Color m_IndicatorColor;
    float m_FadeInTimer = 0.7f;
    float m_FadeOutTimer = 1.0f;
    bool m_OnScreen = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_IndicatorImage = gameObject.GetComponent<Image>();
        m_IndicatorColor = m_IndicatorImage.color;
        m_IndicatorColor.a = 0.0f;
        m_IndicatorImage.color = m_IndicatorColor;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is currently under a certain amount of HP (Default is 33%), and the Image isn't fully on screen yet, fade it in
        if (!m_OnScreen && m_HealthComp.m_CurrentHealth <= m_HealthComp.MaxHealth * m_LowHealthThreshold && m_IndicatorImage.color.a < 1.0f)
        {
            FadeIn(0.0f, 0.75f, m_FadeInTimer);

            // Once it's fully faded in, flip OnScreen to true
            if (m_IndicatorImage.color.a == 0.75f)
                m_OnScreen = true;
        }

        // If the low health image is in screen
        if (m_OnScreen)
        {
            // Temporarily store the last alpha value
            float temp = m_IndicatorColor.a;

            // Change the alpha with a cos wave
            m_IndicatorColor.a = Mathf.Cos(Time.time * 5.0f);

            // Clamp the alpha between 75% and 100%
            m_IndicatorColor.a = Mathf.Clamp(m_IndicatorColor.a, 0.75f, 1.0f);

            // Assign the image's alpha to this new value
            m_IndicatorImage.color = m_IndicatorColor;

            // If the curent alpha is higher than the old one (meaning the alpha currently is going up) (This ensures the SFX is synced with the effect)
            if (!m_Heartbeat.isPlaying && m_IndicatorColor.a > temp)
            {
                // Play the heartbeat sound effect
                m_Heartbeat.Play();
            }
            
            // When the pulse animation is at the end
            if (m_IndicatorColor.a == 0.0f)
            {
                // Stop the heartbeat sound effect (In order to restart is again)
                m_Heartbeat.Stop();
            }
        }

        // If the image is currently on screen, but we recieved enough healing to not be critical anymore
        if (m_OnScreen && m_HealthComp.m_CurrentHealth > m_HealthComp.MaxHealth * m_LowHealthThreshold && m_IndicatorImage.color.a > 0.0f)
        {
            // Fade out the image
            FadeOut(m_IndicatorImage.color.a, 0.0f, m_FadeOutTimer);

            // Stop the heartbeat sound effect
            m_Heartbeat.Stop();

            // When the fade out is complete, flip OnScreen to false
            if (m_IndicatorImage.color.a == 0.0f)
                m_OnScreen = false;
        }
    }

    void FadeIn(float startingValue, float targetValue, float time)
    {
        m_IndicatorColor.a = Mathf.Lerp(targetValue, startingValue, time);
        m_IndicatorImage.color = m_IndicatorColor;

        m_FadeInTimer -= Time.deltaTime;
        m_FadeOutTimer = 1.0f;
    }

    void FadeOut(float startingValue, float targetValue, float time)
    {
        m_IndicatorColor.a = Mathf.Lerp(targetValue, startingValue, time);
        m_IndicatorImage.color = m_IndicatorColor;

        m_FadeOutTimer -= Time.deltaTime;
        m_FadeInTimer = 0.7f;
    }
}
