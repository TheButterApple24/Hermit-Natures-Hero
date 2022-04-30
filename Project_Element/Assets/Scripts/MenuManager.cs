/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              MenuManager
Description:        Stores all the in-game menus objects in the game
Date Created:       03/03/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/03/2022
        - [Jeffrey] Created base implementation
    08/03/2022
        - [Aaron] Added Tutorial Screen Object to the variables that is set in the editor with the others

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager m_Instance = null;
    public static MenuManager Instance { get { return m_Instance; } }

    [SerializeField]
    private GameObject m_GameHUD;
    public GameObject GameHUD { get { return m_GameHUD; } }

    [SerializeField]
    private GameObject m_PauseMenuScreen;
    public GameObject PauseMenuScreen { get { return m_PauseMenuScreen; } }
    [SerializeField]
    private GameObject m_SkillTreeScreen;
    public GameObject SkillTreeScreen { get { return m_SkillTreeScreen; } }
    [SerializeField]
    private GameObject m_BackpackMenu;
    public GameObject BackpackMenu { get { return m_BackpackMenu; } }
    [SerializeField]
    private GameObject m_InventoryMenu;
    public GameObject InventoryMenu { get { return m_InventoryMenu; } }
    [SerializeField]
    private GameObject m_QuestMenu;
    public GameObject QuestMenu { get { return m_QuestMenu; } }
    [SerializeField]
    private GameObject m_SettingsMenuObject;
    public GameObject SettingsMenuObject { get { return m_SettingsMenuObject; } }
    [SerializeField]
    private GameObject m_DeathMenu;
    public GameObject DeathMenu { get { return m_DeathMenu; } }
    [SerializeField]
    private GameObject m_PetLoadoutMenu;
    public GameObject PetLoadoutMenu { get { return m_PetLoadoutMenu; } }
    [SerializeField]
    private TeleportMenu m_TeleportMenu;
    public TeleportMenu TeleportMenu { get { return m_TeleportMenu; } }

    [SerializeField]
    private GameObject m_LoadingScreenObject;
    public GameObject LoadingScreenObject { get { return m_LoadingScreenObject; } }

    private LoadingScreenManager m_LoadingScreen;
    public LoadingScreenManager LoadingScreen { get { return m_LoadingScreen; } }

    [SerializeField]
    private GameObject m_TutorialScreenObject;
    public GameObject TutorialScreenObject { get { return m_TutorialScreenObject; } }

    [SerializeField]
    private GameObject m_CraftingMenu;
    public GameObject CraftingMenu { get { return m_CraftingMenu; } }

    [SerializeField]
    private GameObject m_MapMenu;
    public GameObject MapMenu { get { return m_MapMenu; } }

    [SerializeField]
    private AudioSource m_MenuAudioSource;
    public AudioSource MenuAudioSource { get { return m_MenuAudioSource; } }

    public void SetLoadingScreen(LoadingScreenManager aLoadingScreen)
    {
        m_LoadingScreen = aLoadingScreen;
    }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        if (m_LoadingScreen == null)
        {
            Instantiate(LoadingScreenObject);
        }
    }

}
