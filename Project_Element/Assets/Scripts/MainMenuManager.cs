/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              MainMenuManager
Description:        Stores all the MainMenu objects in the game
Date Created:       04/03/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    04/03/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager m_Instance = null;
    public static MainMenuManager Instance { get { return m_Instance; } }

    [SerializeField]
    private SettingsMenu m_SettingsMenu;
    public SettingsMenu SettingsMenu { get { return m_SettingsMenu; } }

    [SerializeField]
    private Button m_AudioButton;
    public Button AudioButton { get { return m_AudioButton; } }

    [SerializeField]
    private InputField m_SaveNameInput;
    public InputField SaveNameInput { get { return m_SaveNameInput; } }

    [SerializeField]
    private Button m_CreateButton;
    public Button CreateButton { get { return m_CreateButton; } }

    [SerializeField]
    private Button m_NewGameCancelButton;
    public Button NewGameCancelButton { get { return m_NewGameCancelButton; } }

    [SerializeField]
    private Text m_SaveNameUI;
    public Text SaveNameUI { get { return m_SaveNameUI; } }

    [SerializeField]
    private GameObject m_NewGameScreen;
    public GameObject NewGameScreen { get { return m_NewGameScreen; } }

    [SerializeField]
    private GameObject m_LoadGameScreen;
    public GameObject LoadGameScreen { get { return m_LoadGameScreen; } }

    [SerializeField]
    private Button m_LoadGameCancelButton;
    public Button LoadGameCancelButton { get { return m_LoadGameCancelButton; } }

    [SerializeField]
    private GameObject m_ConfirmQuitScreen;
    public GameObject ConfirmQuitScreen { get { return m_ConfirmQuitScreen; } }

    [SerializeField]
    private Button m_ContinueButton;
    public Button ContinueButton { get { return m_ContinueButton; } }

    [SerializeField]
    private Button m_NewButton;
    public Button NewButton { get { return m_NewButton; } }

    [SerializeField]
    private Button m_LoadButton;
    public Button LoadButton { get { return m_LoadButton; } }

    [SerializeField]
    private Button m_SettingsButton;
    public Button SettingsButton { get { return m_SettingsButton; } }

    [SerializeField]
    private Button m_CreditsButton;
    public Button CreditsButton { get { return m_CreditsButton; } }

    [SerializeField]
    private Button m_QuitButton;
    public Button QuitButton { get { return m_QuitButton; } }

    [SerializeField]
    private Button m_ConfirmQuitButton;
    public Button ConfirmQuitButton { get { return m_ConfirmQuitButton; } }

    [SerializeField]
    private Button m_CancelQuitButton;
    public Button CancelQuitButton { get { return m_CancelQuitButton; } }

    [SerializeField]
    private Button m_DeleteSavesButton;
    public Button DeleteSavesButton { get { return m_DeleteSavesButton; } }

    [SerializeField]
    private MainMenu m_MainMenu;
    public MainMenu MainMenu { get { return m_MainMenu; } }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

}
