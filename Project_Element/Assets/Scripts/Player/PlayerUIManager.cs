/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PlayerUIManager
Description:        Handles functionality for all UI and HUD elements
Date Created:       03/03/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    03/03/2022
        - [Jeffrey] Created base implementation
    08/03/2022
        - [Aaron] Grab and store the tutorial maanger from the menu manager instance. Updated the open Backpack Menu function to check for first use and launch the Large Display Tutorial UI.
        - [Aaron] Turned the Backpack Menu function into a generic function that can be used by each menu to display tutorial information.
    09/03/2022
        - [Aaron] Updated to work with the tutorial toggle implemented in the settings menu and player properties
    11/03/2022
        - [Zoe] Added Audio Source and SFX for Menu Opening/Closing
    13/03/2022
        - [Aaron] Updated Init HUD function to remove the object that is no longer used in the HUD Manager
    23/03/2022
        - [Aaron] Removed the Pause and Settings Menu tutorial.

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Tutorial")]
    public string BackpackTutorialText = "";
    public string InventoryTutorialText = "";
    public string QuestTutorialText = "";
    public string SkillTreeTutorialText = "";

    TutorialManager m_TutorialManager;
    bool m_BackpackUsed = false;
    bool m_InventoryUsed = false;
    bool m_QuestUsed = false;
    bool m_SkillTreeUsed = false;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_QuestOpenSFX;
    [SerializeField] private AudioClip m_QuestCloseSFX;
    [SerializeField] private AudioClip m_InventorySFX;

    [Header("Temple")]
    public SkillTemple SkillTemple;

    private void Start()
    {
        MenuManager.Instance.InventoryMenu.SetActive(false);
        MenuManager.Instance.QuestMenu.SetActive(false);

        InitHUD();

        m_TutorialManager = MenuManager.Instance.TutorialScreenObject.GetComponent<TutorialManager>();
    }

    public void OpenPauseMenu()
    {
        if (!PlayerManager.Instance.Player.m_IsMenuOpen)
        {
            DisableGameHUD();

            MenuManager.Instance.PauseMenuScreen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = true;

            Time.timeScale = 0;

            PlayerManager.Instance.Player.m_IsInputDisabled = true;
            PlayerManager.Instance.Player.m_IsMenuOpen = true;
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;

        MenuManager.Instance.LoadingScreen.EnableLoadingScreen();
        SceneManager.LoadSceneAsync(0);
    }

    public void ClosePauseMenu()
    {
        // Turn on the HUD
        EnableGameHUD();

        MenuManager.Instance.PauseMenuScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = false;

        Time.timeScale = 1;

        PlayerManager.Instance.Player.m_IsMenuOpen = false;
        StartCoroutine(DisableInputsTimer());
    }

    // Activate or Deactive the Settings Menu
    public void OpenSettingsMenu()
    {
        MenuManager.Instance.PauseMenuScreen.SetActive(false);
        MenuManager.Instance.SettingsMenuObject.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        MenuManager.Instance.PauseMenuScreen.SetActive(true);
        MenuManager.Instance.SettingsMenuObject.SetActive(false);
    }

    // Activate or Deactive the Skill Tree Menu
    public void OpenSkillTreeMenu()
    {
        if (!PlayerManager.Instance.Player.m_IsMenuOpen)
        {
            if (m_SkillTreeUsed == false)
            {
                m_BackpackUsed = true;
                string titleText = "Skill Tree Menu";
                DisplayTutorialOnFirstRun(titleText, SkillTreeTutorialText);
            }

            // Turn off the HUD
            DisableGameHUD();

            SkillTreeManager.Instance.UpdateSkillTreeUI();

            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisableBackpackMenuButtons();
            MenuManager.Instance.SkillTreeScreen.SetActive(true);

            // Disable other inputs
            PlayerManager.Instance.Player.m_IsInputDisabled = true;

            PlayerManager.Instance.Player.m_IsMenuOpen = true;
        }
    }

    public void CloseSkillTreeMenu()
    {
        // Turn on the HUD
        EnableGameHUD();

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EnableBackpackMenuButtons();
        MenuManager.Instance.SkillTreeScreen.SetActive(false);

        if (SkillTemple != null && SkillTemple.isActiveAndEnabled)
        {
            SkillTemple.CloseDoors();
        }    

        // Enable other inputs
        PlayerManager.Instance.Player.m_IsInputDisabled = false;

        PlayerManager.Instance.Player.m_IsMenuOpen = false;
    }

    // Activate or Deactive the Backpack Menu
    public void OpenBackpackMenu()
    {
        if (!PlayerManager.Instance.Player.m_IsMenuOpen)
        {
            if (m_BackpackUsed == false)
            {
                m_BackpackUsed = true;
                string titleText = "Backpack Menu";
                DisplayTutorialOnFirstRun(titleText, BackpackTutorialText);
            }

            // Turn off the HUD
            DisableGameHUD();

            MenuManager.Instance.BackpackMenu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = true;

            Time.timeScale = 0;
            PlayerManager.Instance.Player.m_IsMenuOpen = true;
        }
    }

    public void CloseBackpackMenu()
    {
        // Turn on the HUD
        EnableGameHUD();

        MenuManager.Instance.BackpackMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = false;

        Time.timeScale = 1;

        StartCoroutine(DisableInputsTimer());
        PlayerManager.Instance.Player.m_IsMenuOpen = false;
    }

    // Activate or Deactive the Inventory Menu
    public void OpenInventoryMenu()
    {
        if (m_InventoryUsed == false)
        {
            m_InventoryUsed = true;
            string titleText = "Equipment Menu";
            DisplayTutorialOnFirstRun(titleText, InventoryTutorialText);
        }

        DisableBackpackMenuButtons();
        MenuManager.Instance.InventoryMenu.gameObject.SetActive(true);
        m_AudioSource.PlayOneShot(m_InventorySFX);
    }

    public void CloseInventoryMenu()
    {
        EnableBackpackMenuButtons();
        PlayerManager.Instance.Player.m_Inventory.DisableSelected();
        MenuManager.Instance.InventoryMenu.gameObject.SetActive(false);
        m_AudioSource.PlayOneShot(m_InventorySFX);
    }

    // Activate or Deactive the Quest Menu
    public void OpenQuestMenu()
    {
        if (m_QuestUsed == false)
        {
            m_QuestUsed = true;
            string titleText = "Quest Menu";
            DisplayTutorialOnFirstRun(titleText, QuestTutorialText);
        }
        DisableBackpackMenuButtons();
        MenuManager.Instance.QuestMenu.gameObject.SetActive(true);
        m_AudioSource.PlayOneShot(m_QuestOpenSFX);
    }

    public void CloseQuestMenu()
    {
        EnableBackpackMenuButtons();
        MenuManager.Instance.QuestMenu.gameObject.SetActive(false);
        m_AudioSource.PlayOneShot(m_QuestCloseSFX);
    }

    public void OpenDeathMenu()
    {
        // Turn off the HUD
        DisableGameHUD();

        // Activate the death menu
        MenuManager.Instance.DeathMenu.SetActive(true);

        // Unlock the cursor & make the cursor visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Set the camera to Menu mode
        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = true;

        // Stop time
        Time.timeScale = 0;

        // Disable other inputs & set m_IsMenuOpen to true
        PlayerManager.Instance.Player.m_IsInputDisabled = true;
        PlayerManager.Instance.Player.m_IsMenuOpen = true;
    }

    public void CloseDeathMenu()
    {
        // Turn the player's HUD on
        EnableGameHUD();

        // Disable the Death Menu
        MenuManager.Instance.DeathMenu.SetActive(false);

        // Lock the cursor & hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the camera back to Game mode
        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = false;

        // Start game time
        Time.timeScale = 1;

        // Re-enable other inputs after a brief period of time
        StartCoroutine(DisableInputsTimer());

        // No longer in a menu
        PlayerManager.Instance.Player.m_IsMenuOpen = false;
    }

    public void OpenTeleportMenu()
    {
        // If any other menus are open
        if (!PlayerManager.Instance.Player.m_IsMenuOpen)
        {
            // Turn the player's HUD off
            DisableGameHUD();

            // Shows the Teleporter Menu & assign teleporters
            MenuManager.Instance.TeleportMenu.gameObject.SetActive(true);
            MenuManager.Instance.TeleportMenu.AssignTeleporters();

            // Unlock the cursor & make the cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Set the camera to Menu mode
            PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = true;

            // Stop time
            Time.timeScale = 0;

            // Disable other inputs & set m_IsMenuOpen to true
            PlayerManager.Instance.Player.m_IsInputDisabled = true;
            PlayerManager.Instance.Player.m_IsMenuOpen = true;
        }
    }

    public void CloseTeleportMenu()
    {
        // Turn the player's HUD on
        EnableGameHUD();

        // Hide Teleporter Menu and removes all buttons in Button Control List
        MenuManager.Instance.TeleportMenu.gameObject.SetActive(false);
        MenuManager.Instance.TeleportMenu.RemoveButtons();

        // Lock the cursor and turn the cursor invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the camera to Play mode
        PlayerManager.Instance.Player.m_FollowCamera.m_InMenu = false;

        // Resume time
        Time.timeScale = 1;

        // Disable inputs for a brief amount of time
        StartCoroutine(DisableInputsTimer());

        // Set Kinematic to false
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        // Set m_IsMenuOpen to false
        PlayerManager.Instance.Player.m_IsMenuOpen = false;
    }

    public void EnableBackpackMenuButtons()
    {
        // Grab the child game object from the backpack menu game object
        GameObject ReturnButton = MenuManager.Instance.BackpackMenu.transform.GetChild(0).gameObject;
        GameObject InventoryButton = MenuManager.Instance.BackpackMenu.transform.GetChild(1).gameObject;
        GameObject CraftingButton = MenuManager.Instance.BackpackMenu.transform.GetChild(2).gameObject;
        GameObject MapButton = MenuManager.Instance.BackpackMenu.transform.GetChild(3).gameObject;
        GameObject QuestButton = MenuManager.Instance.BackpackMenu.transform.GetChild(4).gameObject;
        GameObject LoreButton = MenuManager.Instance.BackpackMenu.transform.GetChild(5).gameObject;

        // Activate the Backpack Menu's buttons
        ReturnButton.SetActive(true);
        InventoryButton.SetActive(true);
        CraftingButton.SetActive(true);
        MapButton.SetActive(true);
        QuestButton.SetActive(true);
        LoreButton.SetActive(true);
    }

    public void DisableBackpackMenuButtons()
    {
        // Grab the child game object from the backpack menu game object
        GameObject ReturnButton = MenuManager.Instance.BackpackMenu.transform.GetChild(0).gameObject;
        GameObject InventoryButton = MenuManager.Instance.BackpackMenu.transform.GetChild(1).gameObject;
        GameObject CraftingButton = MenuManager.Instance.BackpackMenu.transform.GetChild(2).gameObject;
        GameObject MapButton = MenuManager.Instance.BackpackMenu.transform.GetChild(3).gameObject;
        GameObject QuestButton = MenuManager.Instance.BackpackMenu.transform.GetChild(4).gameObject;
        GameObject LoreButton = MenuManager.Instance.BackpackMenu.transform.GetChild(5).gameObject;

        // Disable the Backpack Menu's buttons
        ReturnButton.SetActive(false);
        InventoryButton.SetActive(false);
        CraftingButton.SetActive(false);
        MapButton.SetActive(false);
        QuestButton.SetActive(false);
        LoreButton.SetActive(false);
    }

    public IEnumerator DisableInputsTimer()
    {
        yield return new WaitForSeconds(0.3f);
        PlayerManager.Instance.Player.m_IsInputDisabled = false;
    }

    // Disable the Player HUD elements that aren't immediately active
    void InitHUD()
    {
        HUDManager.Instance.PetSwapIcon.SetActive(false);

        HUDManager.Instance.PrimaryPetIconFill.SetActive(false);
        HUDManager.Instance.PrimaryPetIconOutline.SetActive(false);
        HUDManager.Instance.MainAbilityBorder.SetActive(false);

        HUDManager.Instance.UltimateAbilityIconFill.SetActive(false);
        HUDManager.Instance.UltimateAbilityIconOutline.SetActive(false);
        HUDManager.Instance.UltimateAbilityBorder.SetActive(false);
    }

    // Turn on the gameobject that manages all the Game's HUD elements
    public void EnableGameHUD()
    {
        MenuManager.Instance.GameHUD.SetActive(true);
    }

    // Turn off the gameobject that manages all the Game's HUD elements
    public void DisableGameHUD()
    {
        MenuManager.Instance.GameHUD.SetActive(false);
    }

    public void DisplayTutorialOnFirstRun(string title, string body)
    {
        if (PlayerManager.Instance.Player.m_Properties != null)
        {
            if (PlayerManager.Instance.Player.m_Properties.m_ToggleTutorial == true)
            {
                m_TutorialManager.SetupLargeDisplay(title, body);
            }
        }
    }
}
