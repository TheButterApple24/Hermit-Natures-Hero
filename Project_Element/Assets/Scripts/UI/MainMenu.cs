/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              MainMenu
Description:        Handles main menu functions
Date Created:       20/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    20/10/2021
        - [Jeffrey] Implemented basic main menu implementation
    03/03/2022
        - [Jeffrey] Moved all MainMenu stuff here
    04/03/2022
        - [Jeffrey] Re-Factored menu system

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject OptionsMenuPanel;

    Navigation ContinueButtonNav;
    Navigation NewButtonNav;
    Navigation LoadButtonNav;
    Navigation SettingsButtonNav;
    Navigation CreditsButtonNav;
    Navigation QuitButtonNav;
    Navigation DeleteSavesButtonNav;
    Navigation SaveNameInputNav;
    Navigation CreateButtonNav;
    Navigation NewGameCancelButtonNav;
    Navigation ConfirmQuitButtonNav;
    Navigation CancelQuitButtonNav;

    const string m_PlayerNamePrefKey = "PlayerName";


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(m_PlayerNamePrefKey))
        {
            if (PlayerPrefs.GetString(m_PlayerNamePrefKey) != "")
            {
                PlayerManager.Instance.Player.SaveName = PlayerPrefs.GetString(m_PlayerNamePrefKey);
                MainMenuManager.Instance.SaveNameUI.text = "Profile: " + PlayerManager.Instance.Player.SaveName;
                LoadPlayer();
            }
            else
            {
                PlayerManager.Instance.Player.SaveName = "None";
                MainMenuManager.Instance.ContinueButton.interactable = false;
                MainMenuManager.Instance.SettingsButton.interactable = false;
                MainMenuManager.Instance.SaveNameUI.text = "Profile: None";
            }
        }
        else
        {
            MainMenuManager.Instance.ContinueButton.interactable = false;

            MainMenuManager.Instance.SettingsButton.interactable = false;

            MainMenuManager.Instance.SaveNameUI.text = "Profile: None";
        }

        ContinueButtonNav = MainMenuManager.Instance.ContinueButton.navigation;
        ContinueButtonNav.mode = Navigation.Mode.None;
        ContinueButtonNav.mode = Navigation.Mode.Explicit;
        ContinueButtonNav.selectOnUp = MainMenuManager.Instance.QuitButton;
        ContinueButtonNav.selectOnDown = MainMenuManager.Instance.NewButton;
        ContinueButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        ContinueButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        NewButtonNav = MainMenuManager.Instance.NewButton.navigation;
        NewButtonNav.mode = Navigation.Mode.None;
        NewButtonNav.mode = Navigation.Mode.Explicit;
        NewButtonNav.selectOnUp = MainMenuManager.Instance.ContinueButton;
        NewButtonNav.selectOnDown = MainMenuManager.Instance.LoadButton;
        NewButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        NewButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        LoadButtonNav = MainMenuManager.Instance.LoadButton.navigation;
        LoadButtonNav.mode = Navigation.Mode.None;
        LoadButtonNav.mode = Navigation.Mode.Explicit;
        LoadButtonNav.selectOnUp = MainMenuManager.Instance.NewButton;
        LoadButtonNav.selectOnDown = MainMenuManager.Instance.SettingsButton;
        LoadButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        LoadButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        SettingsButtonNav = MainMenuManager.Instance.SettingsButton.navigation;
        SettingsButtonNav.mode = Navigation.Mode.None;
        SettingsButtonNav.mode = Navigation.Mode.Explicit;
        SettingsButtonNav.selectOnUp = MainMenuManager.Instance.LoadButton;
        SettingsButtonNav.selectOnDown = MainMenuManager.Instance.CreditsButton;
        SettingsButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        SettingsButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        CreditsButtonNav = MainMenuManager.Instance.CreditsButton.navigation;
        CreditsButtonNav.mode = Navigation.Mode.None;
        CreditsButtonNav.mode = Navigation.Mode.Explicit;
        CreditsButtonNav.selectOnUp = MainMenuManager.Instance.SettingsButton;
        CreditsButtonNav.selectOnDown = MainMenuManager.Instance.QuitButton;
        CreditsButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        CreditsButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        QuitButtonNav = MainMenuManager.Instance.QuitButton.navigation;
        QuitButtonNav.mode = Navigation.Mode.None;
        QuitButtonNav.mode = Navigation.Mode.Explicit;
        QuitButtonNav.selectOnUp = MainMenuManager.Instance.CreditsButton;
        QuitButtonNav.selectOnDown = MainMenuManager.Instance.ContinueButton;
        QuitButtonNav.selectOnLeft = MainMenuManager.Instance.DeleteSavesButton;
        QuitButtonNav.selectOnRight = MainMenuManager.Instance.DeleteSavesButton;

        DeleteSavesButtonNav = MainMenuManager.Instance.DeleteSavesButton.navigation;
        DeleteSavesButtonNav.mode = Navigation.Mode.None;
        DeleteSavesButtonNav.mode = Navigation.Mode.Explicit;
        DeleteSavesButtonNav.selectOnUp = MainMenuManager.Instance.NewButton;
        DeleteSavesButtonNav.selectOnDown = MainMenuManager.Instance.NewButton;
        DeleteSavesButtonNav.selectOnLeft = MainMenuManager.Instance.NewButton;
        DeleteSavesButtonNav.selectOnRight = MainMenuManager.Instance.NewButton;

        SaveNameInputNav = MainMenuManager.Instance.SaveNameInput.navigation;
        SaveNameInputNav.mode = Navigation.Mode.None;
        SaveNameInputNav.mode = Navigation.Mode.Explicit;
        SaveNameInputNav.selectOnUp = MainMenuManager.Instance.NewGameCancelButton;
        SaveNameInputNav.selectOnDown = MainMenuManager.Instance.CreateButton;
        SaveNameInputNav.selectOnLeft = null;
        SaveNameInputNav.selectOnRight = null;

        CreateButtonNav = MainMenuManager.Instance.CreateButton.navigation;
        CreateButtonNav.mode = Navigation.Mode.None;
        CreateButtonNav.mode = Navigation.Mode.Explicit;
        CreateButtonNav.selectOnUp = MainMenuManager.Instance.SaveNameInput;
        CreateButtonNav.selectOnDown = MainMenuManager.Instance.NewGameCancelButton;
        CreateButtonNav.selectOnLeft = null;
        CreateButtonNav.selectOnRight = null;

        NewGameCancelButtonNav = MainMenuManager.Instance.NewGameCancelButton.navigation;
        NewGameCancelButtonNav.mode = Navigation.Mode.None;
        NewGameCancelButtonNav.mode = Navigation.Mode.Explicit;
        NewGameCancelButtonNav.selectOnUp = MainMenuManager.Instance.CreateButton;
        NewGameCancelButtonNav.selectOnDown = MainMenuManager.Instance.SaveNameInput;
        NewGameCancelButtonNav.selectOnLeft = null;
        NewGameCancelButtonNav.selectOnRight = null;

        ConfirmQuitButtonNav = MainMenuManager.Instance.ConfirmQuitButton.navigation;
        ConfirmQuitButtonNav.mode = Navigation.Mode.None;
        ConfirmQuitButtonNav.mode = Navigation.Mode.Explicit;
        ConfirmQuitButtonNav.selectOnUp = null;
        ConfirmQuitButtonNav.selectOnDown = null;
        ConfirmQuitButtonNav.selectOnLeft = MainMenuManager.Instance.CancelQuitButton;
        ConfirmQuitButtonNav.selectOnRight = MainMenuManager.Instance.CancelQuitButton;

        CancelQuitButtonNav = MainMenuManager.Instance.CancelQuitButton.navigation;
        CancelQuitButtonNav.mode = Navigation.Mode.None;
        CancelQuitButtonNav.mode = Navigation.Mode.Explicit;
        CancelQuitButtonNav.selectOnUp = null;
        CancelQuitButtonNav.selectOnDown = null;
        CancelQuitButtonNav.selectOnLeft = MainMenuManager.Instance.ConfirmQuitButton;
        CancelQuitButtonNav.selectOnRight = MainMenuManager.Instance.ConfirmQuitButton;

        if (MainMenuManager.Instance.ContinueButton.interactable == false)
        {
            NewButtonNav.selectOnUp = MainMenuManager.Instance.QuitButton;
            QuitButtonNav.selectOnDown = MainMenuManager.Instance.NewButton;
        }

        if (MainMenuManager.Instance.SettingsButton.interactable == false)
        {
            LoadButtonNav.selectOnDown = MainMenuManager.Instance.CreditsButton;
            CreditsButtonNav.selectOnUp = MainMenuManager.Instance.LoadButton;
        }

        MainMenuManager.Instance.ContinueButton.navigation = ContinueButtonNav;
        MainMenuManager.Instance.NewButton.navigation = NewButtonNav;
        MainMenuManager.Instance.LoadButton.navigation = LoadButtonNav;
        MainMenuManager.Instance.SettingsButton.navigation = SettingsButtonNav;
        MainMenuManager.Instance.CreditsButton.navigation = CreditsButtonNav;
        MainMenuManager.Instance.QuitButton.navigation = QuitButtonNav;
        MainMenuManager.Instance.DeleteSavesButton.navigation = DeleteSavesButtonNav;
        MainMenuManager.Instance.SaveNameInput.navigation = SaveNameInputNav;
        MainMenuManager.Instance.CreateButton.navigation = CreateButtonNav;
        MainMenuManager.Instance.NewGameCancelButton.navigation = NewGameCancelButtonNav;
        MainMenuManager.Instance.ConfirmQuitButton.navigation = ConfirmQuitButtonNav;
        MainMenuManager.Instance.CancelQuitButton.navigation = CancelQuitButtonNav;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(int index)
    {
        MenuManager.Instance.LoadingScreen.EnableLoadingScreen();
        SceneManager.LoadSceneAsync(index);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    public void EnableMainMenu()
    {
        MainMenuPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.NewButton.gameObject);

        if (MainMenuManager.Instance.ContinueButton.interactable == true)
        {
            NewButtonNav.selectOnUp = MainMenuManager.Instance.ContinueButton;
            QuitButtonNav.selectOnDown = MainMenuManager.Instance.ContinueButton;
        }

        if (MainMenuManager.Instance.SettingsButton.interactable == true)
        {
            LoadButtonNav.selectOnDown = MainMenuManager.Instance.SettingsButton;
            CreditsButtonNav.selectOnUp = MainMenuManager.Instance.SettingsButton;
        }

        MainMenuManager.Instance.NewButton.navigation = NewButtonNav;
        MainMenuManager.Instance.LoadButton.navigation = LoadButtonNav;
        MainMenuManager.Instance.CreditsButton.navigation = CreditsButtonNav;
        MainMenuManager.Instance.QuitButton.navigation = QuitButtonNav;

        OptionsMenuPanel.SetActive(false);
    }

    public void EnableSettingsMenu()
    {
        MainMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.AudioButton.gameObject);
    }

    public void OpenNewGameMenu()
    {
        MainMenuManager.Instance.NewGameScreen.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.SaveNameInput.gameObject);
    }

    public void SetSaveName(string name)
    {
        MainMenuManager.Instance.SaveNameInput.text = name;
    }

    public void OpenQuitDialog()
    {
        MainMenuManager.Instance.ConfirmQuitScreen.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.CancelQuitButton.gameObject);
    }
    public void CloseQuitDialog()
    {
        MainMenuManager.Instance.ConfirmQuitScreen.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.QuitButton.gameObject);
    }

    public void SavePlayer()
    {
        if (MainMenuManager.Instance.SaveNameInput.text != "")
        {
            SaveSystem.SaveGame(PlayerManager.Instance.Player, MainMenuManager.Instance.SaveNameInput.text);
            MainMenuManager.Instance.NewGameScreen.SetActive(false);

            PlayerManager.Instance.Player.SaveName = MainMenuManager.Instance.SaveNameInput.text;
            MainMenuManager.Instance.SettingsMenu.SaveSettings();
        }
        else
        {
            SaveSystem.SaveGame(PlayerManager.Instance.Player, PlayerManager.Instance.Player.SaveName);
            MainMenuManager.Instance.NewGameScreen.SetActive(false);

            PlayerManager.Instance.Player.SaveName = PlayerManager.Instance.Player.SaveName;
            MainMenuManager.Instance.SettingsMenu.SaveSettings();
        }

        PlayerManager.Instance.Player.m_Properties.m_SaveName = PlayerManager.Instance.Player.SaveName;
        MainMenuManager.Instance.SaveNameInput.text = "";

        MainMenuManager.Instance.SaveNameUI.text = "Profile: " + PlayerManager.Instance.Player.SaveName;
        MainMenuManager.Instance.ContinueButton.interactable = true;
        MainMenuManager.Instance.SettingsButton.interactable = true;

        NewButtonNav.selectOnUp = MainMenuManager.Instance.ContinueButton;
        QuitButtonNav.selectOnDown = MainMenuManager.Instance.ContinueButton;
        LoadButtonNav.selectOnDown = MainMenuManager.Instance.SettingsButton;
        CreditsButtonNav.selectOnUp = MainMenuManager.Instance.SettingsButton;

        MainMenuManager.Instance.NewButton.navigation = NewButtonNav;
        MainMenuManager.Instance.QuitButton.navigation = QuitButtonNav;
        MainMenuManager.Instance.LoadButton.navigation = LoadButtonNav;
        MainMenuManager.Instance.CreditsButton.navigation = CreditsButtonNav;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.ContinueButton.gameObject);

        PlayerPrefs.SetString(m_PlayerNamePrefKey, PlayerManager.Instance.Player.SaveName);
    }

    public void CancelNewScreen()
    {
        MainMenuManager.Instance.NewGameScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.NewButton.gameObject);
    }

    public void OpenLoadGameMenu()
    {
        MainMenuManager.Instance.LoadGameScreen.SetActive(true);
    }

    public void CancelLoadScreen()
    {
        MainMenuManager.Instance.LoadGameScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.LoadButton.gameObject);
    }

    public void LoadPlayer()
    {
        // Grab player data from save file
        PlayerData data = SaveSystem.LoadGame(Application.persistentDataPath + "/saves/" + PlayerManager.Instance.Player.SaveName + ".element");

        // If a file was found, set variables accordingly
        if (data != null)
        {
            MainMenuManager.Instance.LoadGameScreen.SetActive(false);

            MainMenuManager.Instance.SettingsMenu.m_PlayerSaveName = PlayerManager.Instance.Player.SaveName;

            MainMenuManager.Instance.SettingsMenu.LoadSettings(PlayerManager.Instance.Player.SaveName);

            PlayerManager.Instance.Player.m_Properties.m_SaveName = PlayerManager.Instance.Player.SaveName;

            MainMenuManager.Instance.SaveNameUI.text = "Profile: " + PlayerManager.Instance.Player.SaveName;
            MainMenuManager.Instance.ContinueButton.interactable = true;
            MainMenuManager.Instance.SettingsButton.interactable = true;

            PlayerManager.Instance.Player.m_PlayerTotalEXP = data.m_PlayerTotalEXP;
            PlayerManager.Instance.Player.m_PlayerEXPNeededForLevel = data.m_PlayerEXPNeededForLevel;
            PlayerManager.Instance.Player.m_PlayerLevel = data.m_PlayerLevel;
            PlayerManager.Instance.Player.m_PlayerCurrentEXPForLevel = data.m_PlayerCurrentEXPForLevel;
            PlayerManager.Instance.Player.m_SkillPoints = data.m_SkillPoints;

            EnableMainMenu();

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(MainMenuManager.Instance.ContinueButton.gameObject);

            PlayerPrefs.SetString(m_PlayerNamePrefKey, PlayerManager.Instance.Player.SaveName);
        }
        else
        {
            // Not Found
            SavePlayer();
        }
    }
}
