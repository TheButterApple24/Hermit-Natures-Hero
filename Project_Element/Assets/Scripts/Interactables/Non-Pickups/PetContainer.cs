/*===================================================
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Pet Container
Description:        A trigger placed on Pets that allows the player to interact with them, which adds the pet to the player
Date Created:       05/12/2021
Author:             Aaron Wilson
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    05/12/2021
        - [Aaron] Created the Pet Container class, inherited functions from InteractableBase, and added CheckPlayerPetSlots function to manage the logic
    07/12/2021
        - [Aaron] Added an 'unlock' to occur when the pet is interacted with. Unlocks pet in the Pet Manager for use.
    16/02/2022
        - [Jeffrey] Added pets to lore system
    09/03/2022
        - [Max] Renamed Weapon class public variables
    10/03/2022
        - [Aaron] Updated to work with Pet Refactoring 2.0
    11/03/2022
        - [Aaron] Added an element type check when trying to add the secondary pet
    13/03/202
        - [Aaron] Added a call to teleport to player when equiping the primary or secondary pet
    23/03/2022
        - [Aaron] Added tutorial pop up for getting your second pet.
    24/03/2022
        - [Aaron] Created an Unlock function that not only sets the lock bool
    05/04/2022
        - [Max] Added Pet unlock popup

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetContainer : InteractableBase
{
    string m_ContainerTutorialText = "When you have two Pets equipped, the Hermit can channel their power together into an Ultimate Attack by pressing Q";

    Pet m_ContainedPet = null;
    TutorialManager m_TutorialManager;

    public override void Start()
    {
        base.Start();
        m_ContainedPet = this.gameObject.GetComponentInParent<Pet>();
        m_TutorialManager = MenuManager.Instance.TutorialScreenObject.GetComponent<TutorialManager>();
        m_RemoveFromListOnTrigger = true;
    }

    public override void Activate()
    {
        GameManager manager = GameManager.Instance;
        manager.m_LoreUnlockedIDs[m_ItemID] = true;

        CheckPlayerPetSlots();
    }

    public void UnlockPetSaveSystem()
    {
        GameManager manager = GameManager.Instance;
        manager.m_LoreUnlockedIDs[m_ItemID] = true;

        if (m_Player != null && m_ContainedPet != null)
        {
            // if the player has no primary pet, unlock the pet and set as the player's primary pet
            if (m_Player.m_PrimaryPet == null && m_Player.m_SecondaryPet == null)
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_Player.SetPrimaryPet(m_ContainedPet);
                m_Player.m_PrimaryPet.TeleportToPlayer();

                Destroy(this.gameObject);
            }

            // if the player has a primary pet but no secondary pet, and that the new pet doesn't match the primary pets element type unlock the pet and set as the player's secondary pet
            else if (m_Player.m_PrimaryPet != null && m_Player.m_PrimaryPet.PetElementType != m_ContainedPet.PetElementType && m_Player.m_SecondaryPet == null)
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_Player.SetSecondaryPet(m_ContainedPet);
                m_Player.m_SecondaryPet.TeleportToPlayer();

                m_TutorialManager.DisplayPopup(m_ContainerTutorialText, 6.0f);

                Destroy(this.gameObject);
            }

            // if the player has both pets, unlock the pet and send the pet to the PetManager
            else
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_ContainedPet.gameObject.SetActive(false);

                Destroy(this.gameObject);
            }
        }

        // Set the player and their weapon's element type to match their primary pet's type
        if (m_Player.m_PrimaryPet != null)
        {
            m_Player.m_Element = m_Player.m_PrimaryPet.PetElementType;

            if (m_Player.m_MainWeapon != null)
            {
                m_Player.m_MainWeapon.WeaponElement = m_Player.m_PrimaryPet.PetElementType;
                m_Player.m_MainWeapon.SetMaterial(m_Player);
            }
        }
    }

    void CheckPlayerPetSlots()
    {
        if (m_Player != null && m_ContainedPet != null)
        {
            // if the player has no primary pet, unlock the pet and set as the player's primary pet
            if (m_Player.m_PrimaryPet == null && m_Player.m_SecondaryPet == null)
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_Player.SetPrimaryPet(m_ContainedPet);
                m_Player.m_PrimaryPet.TeleportToPlayer();

                // Pop up Unlocked Pet Popup
                HUDManager.Instance.UnlockedPopUp.SetActive(true);
                HUDManager.Instance.UnlockedPopUpText.text = m_ContainedPet.PetName.ToString() + " has joined your party!";

                Destroy(this.gameObject);
            }

            // if the player has a primary pet but no secondary pet, and that the new pet doesn't match the primary pets element type unlock the pet and set as the player's secondary pet
            else if (m_Player.m_PrimaryPet != null && m_Player.m_PrimaryPet.PetElementType != m_ContainedPet.PetElementType && m_Player.m_SecondaryPet == null)
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_Player.SetSecondaryPet(m_ContainedPet);
                m_Player.m_SecondaryPet.TeleportToPlayer();

                m_TutorialManager.DisplayPopup(m_ContainerTutorialText, 6.0f);

                // Pop up Unlocked Pet Popup
                HUDManager.Instance.UnlockedPopUp.SetActive(true);
                HUDManager.Instance.UnlockedPopUpText.text = m_ContainedPet.PetName.ToString() + " has joined your party!";

                Destroy(this.gameObject);
            }

            // if the player has both pets, unlock the pet and send the pet to the PetManager
            else
            {
                //m_ContainedPet.IsLocked = false;
                UnlockPet();
                m_ContainedPet.gameObject.SetActive(false);

                // Pop up Pet Container Popup
                HUDManager.Instance.UnlockedPopUp.SetActive(true);
                HUDManager.Instance.UnlockedPopUpText.text = m_ContainedPet.PetName.ToString() + " was sent to your home!";

                Destroy(this.gameObject);
            }
        }

        // Set the player and their weapon's element type to match their primary pet's type
        if (m_Player.m_PrimaryPet != null)
        {
            m_Player.m_Element = m_Player.m_PrimaryPet.PetElementType;

            if (m_Player.m_MainWeapon != null)
            {
                m_Player.m_MainWeapon.WeaponElement = m_Player.m_PrimaryPet.PetElementType;
                m_Player.m_MainWeapon.SetMaterial(m_Player);
            }
        }
    }

    void UnlockPet()
    {
        m_ContainedPet.IsLocked = false;
        
        // Update the Pet Loadout Menu's stored pet image and text for the pet contained
        for (int i = 0; i < PetManager.pmInstance.Pets.Length; i++)
        {
            if (m_ContainedPet == PetManager.pmInstance.Pets[i])
            {
                MenuManager.Instance.PetLoadoutMenu.transform.GetChild(2).GetChild(i).GetChild(0).GetComponent<Image>().sprite = PetManager.pmInstance.Pets[i].PetIconFill;
                MenuManager.Instance.PetLoadoutMenu.transform.GetChild(2).GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }
        
        //MenuManager.Instance.PetLoadoutMenu.transform.GetChild(2)
    }

    public Pet GetContainedPet() { return m_ContainedPet; }
}
