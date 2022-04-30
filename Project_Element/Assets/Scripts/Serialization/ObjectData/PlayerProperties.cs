/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PlayerProperties
Description:        Handles bringing settings to and from the main menu
Date Created:       04/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    04/11/2021
        - [Jeffrey] Created base class
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    09/03/2022
        - [Aaron] Added bool for toggling the Tutorial UI if the player (or devs) want to skip the tutorials

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    private static PlayerProperties m_Instance = null;
    public static PlayerProperties Instance { get { return m_Instance; } }

    public string m_SaveName = "Placeholder";

    public float m_MouseSensitivity = 500.0f;
    public Color m_BrightnessValue = Color.black;
    public bool m_InvertXAxis;
    public bool m_InvertYAxis;

    public bool m_ToggleSprint;
    public bool m_ToggleTutorial;

    //public int m_PlayerTotalEXP = 0;
    //public int m_PlayerCurrentEXPForLevel = 0;
    //public int m_PlayerEXPNeededForLevel = 0;
    //public int m_PlayerLevel = 0;
    //public int m_SkillPoints = 0;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        DontDestroyOnLoad(this);
    }
}
