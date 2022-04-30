/*===================================================

Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Pet Manager
Description:        Manages the pet game objects and lets the player swap their pet loadout between the pets they've acquired.
Date Created:       05/12/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    05/12/2021
        - [Aaron] Created the Pet Manager class
    06/12/2021
        - [Aaron] Set up references to the Pet Objects that have been created and set them in Start.
    08/12/2021
        - [Aaron] Changed the Menu object to be set in the editor. This lets the gameobject be set as deactve on
                       start and become active when an interaction occurs.
    16/02/2022
        - [Aaron] Move weapon logic into a private function called UpdateWeapon
                        Created the Set Selected Pet functions and altered the SetPrimaryPet Function to set that selected pet.
                        Created the SetSecondaryPet function to function similar to the previously mentioned function, execpt altering the secondary pet slot.
    17/02/2022
        - [Aaron] Added functions to grab and change the sprite images in the pet loadout menu to display the currently equip pet's names and icons.
                        Updated the set pet functions to change the current pet UI icons when the pet is changed or whenever the menu is opened.
    18/02/2022
        - [Aaron] Updated the selection and clear selection functions to change the profile display image in the menu. Made sure to grab default image and reset it when the menu closes.
    09/03/2022
        - [Max] Renamed Weapon class public variables
        - [Aaron] Added varaibles to manage the pet menu's tutorial as well as storing the UI and using the stored references rather than finding and grabbing the UI piece each time.
        - [Aaron] Added a HandleEquipedPetIcon method to reduce code in the open pet menu function and add a check for equipped pets to diplay, incase the player doesn't have them.
    10/03/2022
        - [Aaron] Updated to work with Pet Refactor 2.0
    11/03/2022
        - [Aaron] Updated class to clean up the code as part of the Ability Refactoring 2.0 process
    16/03/2022
        - [Aaron] Adjusted the Set Primary and Secondary Pet functions to check for pets at the start of the function, and allow the same element type pets to swap with each other

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetManager : InteractableBase
{
    /* Pet */
    [HideInInspector]public GameObject m_SelectedPet = null;

    GameObject m_StoredPet1 = null;
    GameObject m_StoredPet2 = null;
    GameObject m_StoredPet3 = null;
    GameObject m_StoredPet4 = null;
    GameObject m_StoredPet5 = null;
    GameObject m_StoredPet6 = null;

    public PetContainer[] PetContainers;
    public Pet[] Pets;

    /* UI */
    public GameObject m_PetLoadoutMenuScreen;
    public string PetManagerTutorialText = "";

    Sprite m_DefaultProfileDisplayImage = null;
    GameObject m_EquipedPrimaryPetIcon = null;
    GameObject m_EquipedPrimaryPetText = null;
    GameObject m_EquipedSecondaryPetIcon = null;
    GameObject m_EquipedSecondaryPetText = null;
    bool m_PetLoadoutUsed = false;

    [HideInInspector]public static PetManager pmInstance;

    protected override void Awake()
    {
        if (pmInstance != null && pmInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        pmInstance = this;
    }

    public override void Start()
    {
        base.Start();

        InitStoredPets();

        m_DefaultProfileDisplayImage = m_PetLoadoutMenuScreen.transform.GetChild(5).GetComponent<Image>().sprite;
        m_EquipedPrimaryPetIcon = m_PetLoadoutMenuScreen.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
        m_EquipedPrimaryPetText = m_PetLoadoutMenuScreen.transform.GetChild(4).GetChild(0).GetChild(1).gameObject;
        m_EquipedSecondaryPetIcon = m_PetLoadoutMenuScreen.transform.GetChild(4).GetChild(1).GetChild(0).gameObject;
        m_EquipedSecondaryPetText = m_PetLoadoutMenuScreen.transform.GetChild(4).GetChild(1).GetChild(1).gameObject;
    }

    public override void Activate()
    {
        OpenPetLoadoutMenu();
    }

    /* Pet Loadout Menu Functions */

    public void OpenPetLoadoutMenu()
    {
        if (!m_Player.m_IsMenuOpen)
        {
            if (m_PetLoadoutUsed == false)
            {
                m_PetLoadoutUsed = true;
                string title = "Pet Loadout Menu";
                m_Player.PlayerUI.DisplayTutorialOnFirstRun(title, PetManagerTutorialText);
            }

            // Turn off the player's HUD and turn on the Pet Loadout Menu
            m_Player.PlayerUI.DisableGameHUD();
            m_PetLoadoutMenuScreen.SetActive(true);

            HandleEquipedPetIcons();

            // Make the cursor available for use
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Change the camera to be in menu
            m_Player.m_FollowCamera.m_InMenu = true;

            Time.timeScale = 0;

            // Set player bools for this menu
            m_Player.m_IsInputDisabled = true;
            m_Player.m_IsMenuOpen = true;
        }
    }

    public void ClosePetLoadoutMenu()
    {
        // Make sure the selected pet is reset
        ClearSelectedPet();

        // Turn on the player's HUD and turn off the Pet Loadout Menu
        m_Player.PlayerUI.EnableGameHUD();
        m_PetLoadoutMenuScreen.SetActive(false);

        // Return the cursor to the game world and hide it.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Change the camera to be back in the game world
        m_Player.m_FollowCamera.m_InMenu = false;

        Time.timeScale = 1;

        // Set player bools for this menu
        m_Player.m_IsInputDisabled = false;
        m_Player.m_IsMenuOpen = false;
        StartCoroutine(m_Player.PlayerUI.DisableInputsTimer());
    }

    /* ***** Pet Loadout Menu's Button Functions ***** */
    public void SetSelectedAsPrimaryPet()
    {
        if (m_SelectedPet == null)
            return;

        // Grab the current pets if the player has both
        if (m_Player.m_PrimaryPet == null || m_Player.m_SecondaryPet == null)
            return;

        // Grab the current pets
        GameObject primPet = m_Player.m_PrimaryPet.gameObject;
        GameObject secPet = m_Player.m_SecondaryPet.gameObject;

        // Check if the slected pet is already a current pet
        if (m_SelectedPet == primPet || m_SelectedPet == secPet)
            return;

        // Check if the selected pet shares the same element type as the current pets
        if (m_SelectedPet.GetComponent<Pet>().PetElementType == secPet.GetComponent<Pet>().PetElementType)
            return;

        // Check if the pet has been unlocked
        if (m_SelectedPet.GetComponent<Pet>().IsLocked == false)
        {
            // Activate the new Pet
            m_SelectedPet.SetActive(true);
            primPet.GetComponent<Pet>().DeactivatePet();

            // Set the selected pet as the new primary pet and teleport it to the player
            m_Player.SetPrimaryPet(m_SelectedPet.GetComponent<Pet>());
            m_Player.m_PrimaryPet.TeleportToPlayer();
        }

        // Set the currently equipped pet display to the equipped pet's icons
        SetPrimaryPetDisplay(m_Player.m_PrimaryPet.PetIconFill, m_Player.m_PrimaryPet.PetName);
        SetSecondaryPetDisplay(m_Player.m_SecondaryPet.PetIconFill, m_Player.m_SecondaryPet.PetName);

        UpdateWeapon();
    }

    public void SetSelectedAsSecondaryPet()
    {
        if (m_SelectedPet == null)
            return;

        // Grab the current pets if the player has both
        if (m_Player.m_PrimaryPet == null || m_Player.m_SecondaryPet == null)
            return;

        GameObject primPet = m_Player.m_PrimaryPet.gameObject;
        GameObject secPet = m_Player.m_SecondaryPet.gameObject;

        // Check if the slected pet is already a current pet
        if (m_SelectedPet == primPet || m_SelectedPet == secPet)
            return;

        // Check if the selected pet shares the same element type as the current primary pet, they can't have 2 of the same type at the same time but they can swap out the same element type
        if (m_SelectedPet.GetComponent<Pet>().PetElementType == primPet.GetComponent<Pet>().PetElementType)
            return;

        // Check if the pet has been unlocked
        if (m_SelectedPet.GetComponent<Pet>().IsLocked == false)
        {
            // Activate the new Pet
            m_SelectedPet.SetActive(true);
            secPet.GetComponent<Pet>().DeactivatePet();

            // Set the selected pet as the new primary pet and teleport it to the player
            m_Player.SetSecondaryPet(m_SelectedPet.GetComponent<Pet>());
            m_Player.m_SecondaryPet.TeleportToPlayer();
        }

        // Set the currently equipped pet display to the equipped pet's icons
        SetPrimaryPetDisplay(m_Player.m_PrimaryPet.PetIconFill, m_Player.m_PrimaryPet.PetName);
        SetSecondaryPetDisplay(m_Player.m_SecondaryPet.PetIconFill, m_Player.m_SecondaryPet.PetName);

        UpdateWeapon();
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP1()
    {
        ClearSelectedPet();
        if (m_StoredPet1.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet1;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP2()
    {
        ClearSelectedPet();
        if (m_StoredPet2.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet2;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP3()
    {
        ClearSelectedPet();

        if (m_StoredPet3.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet3;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP4()
    {
        ClearSelectedPet();

        if (m_StoredPet4.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet4;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP5()
    {
        ClearSelectedPet();

        if (m_StoredPet5.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet5;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    // Clear the previous selection in case there is one, and sets the stored pet as the selected pet.
    public void SelectSP6()
    {
        ClearSelectedPet();

        if (m_StoredPet6.GetComponent<Pet>().IsLocked == true)
            return;

        m_SelectedPet = m_StoredPet6;

        SetPetProfileDisplayImage(m_SelectedPet.GetComponent<Pet>().PetProfileDisplayImage);
    }

    /* Manager specific functions */

    // Grab the manager's children, if any children have a pet component, add it to the storedpets container.
    void InitStoredPets()
    {
        m_StoredPet1 = transform.GetChild(1).gameObject;
        m_StoredPet2 = transform.GetChild(2).gameObject;
        m_StoredPet3 = transform.GetChild(3).gameObject;
        m_StoredPet4 = transform.GetChild(4).gameObject;
        m_StoredPet5 = transform.GetChild(5).gameObject;
        m_StoredPet6 = transform.GetChild(6).gameObject;
    }

    // Reset the selected pet to null (no selection) and reset the profile display to default
    void ClearSelectedPet()
    {
        m_SelectedPet = null;

        m_PetLoadoutMenuScreen.transform.GetChild(5).GetComponent<Image>().sprite = m_DefaultProfileDisplayImage;
    }

    // Check if a weapon is equipped and if there is a primary pet equipped. If so, change the weapon's element type and change the material
    void UpdateWeapon()
    {
        if (m_Player.m_MainWeapon != null)
        {
            if (m_Player.m_PrimaryPet != null)
            {
                m_Player.m_MainWeapon.WeaponElement = m_Player.m_PrimaryPet.PetElementType;
                m_Player.m_MainWeapon.SetMaterial(m_Player);
            }
        }
    }

    void SetPetProfileDisplayImage(Sprite profileSprite)
    {
        m_PetLoadoutMenuScreen.transform.GetChild(5).GetComponent<Image>().sprite = profileSprite;
    }

    // If the Player has pets, they will be displayed to the equipped pet's menu icons
    void HandleEquipedPetIcons()
    {
        if (m_Player.m_PrimaryPet != null)
        {
            m_EquipedPrimaryPetIcon.SetActive(true);
            m_EquipedPrimaryPetText.SetActive(true);

            SetPrimaryPetDisplay(m_Player.m_PrimaryPet.PetIconFill, m_Player.m_PrimaryPet.PetName);
        }
        else
        {
            m_EquipedPrimaryPetIcon.SetActive(false);
            m_EquipedPrimaryPetText.SetActive(false);
        }

        if (m_Player.m_SecondaryPet != null)
        {
            m_EquipedSecondaryPetIcon.SetActive(true);
            m_EquipedSecondaryPetText.SetActive(true);

            SetSecondaryPetDisplay(m_Player.m_SecondaryPet.PetIconFill, m_Player.m_SecondaryPet.PetName);
        }
        else
        {
            m_EquipedSecondaryPetIcon.SetActive(false);
            m_EquipedSecondaryPetText.SetActive(false);
        }
    }

    /* Current Equipped Pet Display Functions */

    void SetPrimaryPetDisplay(Sprite petSprite, string petName)
    {
        SetPrimaryPetDisplaySprite(petSprite);
        SetPrimaryPetDisplayText(petName);
    }

    void SetSecondaryPetDisplay(Sprite petSprite, string petName)
    {
        SetSecondaryPetDisplaySprite(petSprite);
        SetSecondaryPetDisplayText(petName);
    }

    void SetPrimaryPetDisplaySprite(Sprite petSprite)
    {
        m_EquipedPrimaryPetIcon.GetComponent<Image>().sprite = petSprite;
    }

    void SetPrimaryPetDisplayText(string petName)
    {
        m_EquipedPrimaryPetText.GetComponent<Text>().text = petName;
    }

    void SetSecondaryPetDisplaySprite(Sprite petSprite)
    {
        m_EquipedSecondaryPetIcon.GetComponent<Image>().sprite = petSprite;
    }

    void SetSecondaryPetDisplayText(string petName)
    {
        m_EquipedSecondaryPetText.GetComponent<Text>().text = petName;
    }
}
