/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              TeleportMenu
Description:        Handles the Teleport Menu UI
Date Created:       19/11/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    16/11/2021
        - [Max] Created base class
    19/11/2021
        - [Max] Fixed Teleporter Menu
    30/11/2021
        - [Max] Removed m_IsFlipped bool
        - [Max] Added Comments
    05/12/2021
        - [Zoe] Using a teleporter will not update the player's respawn location
    08/12/2021
        - [Max] Fixed Bug where Pets wouldn't teleport to Player
    11/02/2022
        - [Max] Added Current Teleporter Text
    12/02/2022
        - [Max] Modified Teleport Height
    15/02/2022
        - [Max] Added Teleport Transform and disabled player kinematic when teleported
    24/02/2022
        - [Max] Added No Shrines Added Check
    11/03/2022
        - [Zoe] Added Audio Source and SFX for Teleporting
    25/03/2022
        - [Max] Allowed teleportation in combat
    01/04/2022
        - [Max] Added Shine Unlock Popup Object

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportMenu : MonoBehaviour
{
    public FastTravelManager FastTravelManager;
    public List<Teleporters> AllTeleportersList;
    public List<Teleporters> UnlockedTeleportersList;
    public Text CurrentTeleporterText;
    public Text NoTeleporterAvailableText;

    [Header("Sound")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_TeleportSFX;

    private void Start()
    {
        List<Teleporters> m_UnlockedTeleportersList = new List<Teleporters>();
        List<Teleporters> m_AllTeleportersList = new List<Teleporters>();
    }

    public void AssignTeleporters()
    {
        // If FastTravelManager exists
        if (FastTravelManager != null)
        {
            // Set AllTeleportersList to FastTravelManager's AllTeleportersList
            AllTeleportersList = FastTravelManager.AllTeleportersList;

            // Set UnlockedTeleportersList to FastTravelManager's UnlockedTeleportersList
            UnlockedTeleportersList = FastTravelManager.UnlockedTeleportersList;

            // If Unlocked Teleporter list is not empty
            if (UnlockedTeleportersList.Count > 0)
            {
                // For each teleporter
                foreach (Teleporters teleporters in UnlockedTeleportersList)
                {
                    // If teleporter isn't the current teleporter
                    if (teleporters != FastTravelManager.CurrentTeleporter)
                    {
                        // Create Button for teleporter passing its name and unlocked state
                        GetComponentInChildren<ControlButtonList>().CreateButton(teleporters.name, teleporters.m_IsUnlocked);
                    }
                    else if (teleporters == FastTravelManager.CurrentTeleporter)
                    {
                        // If the current teleporter is the only teleporter unlocked
                        if (UnlockedTeleportersList.Count == 1)
                        {
                            // Show "No Available Shrines" Text
                            if (NoTeleporterAvailableText != null)
                            {
                                NoTeleporterAvailableText.enabled = true;
                            }
                        }
                        // If the current teleporter is NOT the only teleporter unlocked
                        else if (UnlockedTeleportersList.Count > 1)
                        {
                            // Hide "No Available Shrines" Text
                            if (NoTeleporterAvailableText != null)
                            {
                                NoTeleporterAvailableText.enabled = false;
                            }
                        }
                    }
                }
            }

            // Set Current Teleporter Text if it exists
            if (CurrentTeleporterText != null)
            {
                CurrentTeleporterText.text = FastTravelManager.CurrentTeleporter.name.ToString();
            }
        }

    }

    public void RemoveButtons()
    {
        // Removes all buttons from Control Button List
        GetComponentInChildren<ControlButtonList>().RemoveButtons();
    }


    public void Teleport(string teleporterName)
    {
        // For each unlocked teleporter
        foreach (Teleporters teleporter in UnlockedTeleportersList)
        {
            // if teleporter's name matches clicked button's teleporter name
            if (teleporter.name == teleporterName)
            {
                // if Player exists and is NOT in combat
                if (FastTravelManager.Player != null)
                {
                    // Set Target Teleporter to teleporter
                    FastTravelManager.TargetTeleporter = teleporter;

                    // Set Player Kinematic to false
                    Rigidbody rigidbody = FastTravelManager.Player.transform.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;
                    }

                    // Set Player's position to Target Teleporter's position and rotation
                    if (FastTravelManager.TargetTeleporter.TeleportTransform != null)
                    {
                        FastTravelManager.Player.transform.position = FastTravelManager.TargetTeleporter.TeleportTransform.position;
                        FastTravelManager.Player.transform.rotation = FastTravelManager.TargetTeleporter.TeleportTransform.rotation;
                    }
                    //FastTravelManager.m_Player.transform.position = FastTravelManager.m_TargetTeleporter.transform.Find("Mesh").gameObject.transform.Find("TeleportTransform").gameObject.transform.position;
                    //FastTravelManager.m_Player.transform.rotation = FastTravelManager.m_TargetTeleporter.transform.Find("Mesh").gameObject.transform.Find("TeleportTransform").gameObject.transform.rotation;

                    // Move pets to Player's new position
                    if (FastTravelManager.Player.m_PrimaryPet != null)
                    {
                        FastTravelManager.Player.m_PrimaryPet.TeleportToPlayer();
                    }

                    if (FastTravelManager.Player.m_SecondaryPet != null)
                    {
                        FastTravelManager.Player.m_SecondaryPet.TeleportToPlayer();
                    }

                    // Update the player's respawn location
                    FastTravelManager.Player.m_RespawnPosition = FastTravelManager.TargetTeleporter.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

					// Close Teleport Menu
                    FastTravelManager.Player.PlayerUI.CloseTeleportMenu();

                    // Play SFX
                    m_AudioSource.PlayOneShot(m_TeleportSFX);
                }

            }
        }
    }
}
