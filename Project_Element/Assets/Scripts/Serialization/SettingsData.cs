/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              SettingsData
Description:        Handles saving/loading the setting values
Date Created:       20/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/2021
        - [Jeffrey] Created base class
    02/11/2021
        - [Jeffrey] Implemented a ton of settings
    04/11/2021
        - [Jeffrey] Settings can save and load
    09/03/2022
        - [Aaron] Added toggle sprint functionality 

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    // User Settings
    public float m_MasterVolume = 0.0f;
    public float m_MusicVolume = 0.0f;
    public float m_EffectsVolume = 0.0f;

    public float m_ResolutionWidth = 0.0f;
    public float m_ResolutionHeight = 0.0f;

    public bool m_IsFullscreen = false;
    public bool m_IsVsync = false;

    public int m_GraphicsIndex = 0;

    public string m_PlayerSaveName;
    public float m_BrightnessValue;

    public float m_MouseSensitivityValue = 500.0f;

    public bool m_InvertXAxis;
    public bool m_InvertYAxis;

    public bool m_ToggleSprint;
    public bool m_ToggleTutorial;
}
