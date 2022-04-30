/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              EnemyHealthBar
Description:        Handles Enemy's health bar
Date Created:       03/02/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/02/2022
        - [Max] Started EnemyHealthBar.
    04/02/2022
        - [Max] Changed Health Bar Colour to match Enemy's Element
    08/02/2022
        - [Max] Added Comments
		
 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Elements;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider m_Slider;
    public Sprite FireHealthBarBackground;
    public Sprite PlantHealthBarBackground;
    public Sprite WaterHealthBarBackground;
    public Image ElementIconSlot;
    public Sprite FireElementIcon;
    public Sprite PlantElementIcon;
    public Sprite WaterElementIcon;
    public Sprite DeadCultIcon;
    float m_DamageTimeRemaining = 0.0f;
    float m_HealingTimeRemaining = 0.0f;

    public void Reset()
    {
        m_DamageTimeRemaining = 0.0f;
        m_HealingTimeRemaining = 0.0f;
    }

    public void Init(float currentValue, float minHealth, float maxHealth, Element element)
    {
        // Assign appropriate slider color based on Enemy's element
        switch (element)
        {
            case Element.Fire:
                m_Slider.fillRect.GetComponent<Image>().sprite = FireHealthBarBackground;
                ElementIconSlot.sprite = FireElementIcon;
                break;
            case Element.Water:
                m_Slider.fillRect.GetComponent<Image>().sprite = WaterHealthBarBackground;
                ElementIconSlot.sprite = WaterElementIcon;
                break;
            case Element.Plant:
                m_Slider.fillRect.GetComponent<Image>().sprite = PlantHealthBarBackground;
                ElementIconSlot.sprite = PlantElementIcon;
                break;
            default:
                Debug.LogError("EnemyHealthBar.cs - Element Set to None");
                break;
        }

        // Set Slider minValue, maxValue, value, and fill color to elemental color
        m_Slider.minValue = minHealth;
        m_Slider.maxValue = maxHealth;
        m_Slider.value = currentValue;
    }

    public void AdjustSlider(float healthChange, float currentHealth, bool isHealing)
    {
        // Modify appropriate timer depending on whether health is increasing or decreasing
        if (!isHealing)
        {
            m_DamageTimeRemaining = healthChange;
        }
        else
        {
            m_HealingTimeRemaining = -healthChange;
        }
    }

    public void Update()
    {
        // If Damage Timer is above 0
        if (m_DamageTimeRemaining > 0.0f)
        {
            // Decrement Timer and Slider value
            m_Slider.value -= 0.2f;
            m_DamageTimeRemaining -= 0.2f;
        }
        else
        {
            // Clamp Damage Timer to 0
            m_DamageTimeRemaining = 0.0f;
        }

        // If Healing Timer is above 0
        if (m_HealingTimeRemaining < 0.0f)
        {
            // Increment Timer and Slider value
            m_Slider.value += 0.25f;
            m_HealingTimeRemaining += 0.25f;
        }
        else
        {
            // Clamp Healing Timer to 0
            m_HealingTimeRemaining = 0.0f;
        }
    }
}
