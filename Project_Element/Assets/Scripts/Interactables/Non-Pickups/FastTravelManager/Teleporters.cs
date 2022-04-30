/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Teleporters
Description:        Handles the Teleporter Objects
Date Created:       17/11/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/11/2021
        - [Max] Created base class
    19/11/2021
        - [Max] Flipped List of Teleporters
    30/11/2021
        - [Max] Added Comments
    05/12/2021
        - [Zoe] Unlocking a teleporter will update the player's respawn location
    10/02/2022
        - [Max] Fixed Teleporter Prefab and functionality
    12/02/2022
        - [Max] Fixed Teleporter List Sort + Added Japanese Lanterns
    15/02/2022
        - [Max] Added Teleport Transform and enabled player kinematic when opening teleport menu
        - [Max] Added inactive check for shrine objects
    24/02/2022
        - [Max] Modified OpenTeleport Menu behavior
    04/03/2022
        - [Jeffrey] Re-Factored menu system
    11/03/2022
        - [Max] Optimized code
    31/03/2022
        - [Max] Removed m_CanTeleport
    13/04/2022
        - [Max] Added null checks

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;
using System;

public class Teleporters : InteractableBase
{
    public Element m_ShrineElement;
    public Transform TeleportTransform;
    [HideInInspector] public bool m_IsUnlocked;

    [SerializeField]
    private ShrineStatue FireStatue;
    [SerializeField]
    private ShrineStatue WaterStatue;
    [SerializeField]
    private ShrineStatue PlantStatue;

    [SerializeField]
    private ShrineGate ShrineGate;
    [SerializeField]
    private JapaneseLantern JapaneseLanternLeft;
    [SerializeField]
    private JapaneseLantern JapaneseLanternRight;

    [UniqueIdentifier]
    public string SaveId;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // Set this Teleporter to LOCKED
        m_IsUnlocked = false;

        // Get FastTravelManager
        FastTravelManager fastTravel = transform.parent.GetComponent<FastTravelManager>();

        // If FastTravelManager exists
        if (fastTravel != null)
        {
            // Add this teleporter to FastTravelManager's list of all teleporters
            fastTravel.AddTeleporter(this);
        }
    }

    public override void Activate()
    {
        // Get FastTravelManager from parent
        FastTravelManager fastTravel = transform.parent.GetComponent<FastTravelManager>();

        // If FastTravelManager exists and Player can teleport
        if (fastTravel != null) // && !m_Player.m_IsInCombat
        {
            // Set Current Teleporter to this Teleporter
            fastTravel.CurrentTeleporter = this;

            // Update the Player's respawn location
            m_Player.m_RespawnPosition = gameObject.transform.position + new Vector3(0.0f, 1.0f, 2.5f);

            // If this Teleporter is NOT unlocked
            if (!m_IsUnlocked)
            {
                UnlockTeleporter(fastTravel);
            }
            else
            {
                // Open Teleport Menu
                OpenTeleportMenu(fastTravel);
            }

            // Save Player Game
            PlayerManager.Instance.Player.SavePlayer();
        }
    }

    public void UnlockTeleporter(FastTravelManager fastTravel)
    {
        if (fastTravel == null)
        {
            fastTravel = transform.parent.GetComponent<FastTravelManager>();
        }

        // Set this Teleporter to unlocked
        m_IsUnlocked = true;
        Debug.Log("Unlocked");

        // Pop up Unlocked Shrine Popup
        HUDManager.Instance.UnlockedPopUp.SetActive(true);
        HUDManager.Instance.UnlockedPopUpText.text = name.ToString() + " Unlocked!";

        //Increment number of unlocked teleporters
        fastTravel.NumTeleportersUnlocked++;

        //Add this teleporter to the FastTravelManager's Unlocked Teleporter List
        fastTravel.AddUnlockedTeleporter(this);

        // Set Activated materials to Shrine Statue and Gate based on the element of the shrine
        SetActivatedMaterial(m_ShrineElement);
    }

    public void UnlockTeleporterSaveSystem(FastTravelManager fastTravel)
    {
        if (fastTravel == null)
        {
            fastTravel = transform.parent.GetComponent<FastTravelManager>();
        }

        // Set this Teleporter to unlocked
        m_IsUnlocked = true;

        //Increment number of unlocked teleporters
        fastTravel.NumTeleportersUnlocked++;

        //Add this teleporter to the FastTravelManager's Unlocked Teleporter List
        fastTravel.AddUnlockedTeleporter(this);

        // Set Activated materials to Shrine Statue and Gate based on the element of the shrine
        SetActivatedMaterial(m_ShrineElement);
    }

    public void SetActivatedMaterial(Element element)
    {
        // Activate Statues
        if (FireStatue != null && FireStatue.isActiveAndEnabled)
        {
            FireStatue.Activate(element == Element.Fire || element == Element.All);
        }

        if (WaterStatue != null && WaterStatue.isActiveAndEnabled)
        {
            WaterStatue.Activate(element == Element.Water || element == Element.All);
        }

        if (PlantStatue != null && PlantStatue.isActiveAndEnabled)
        {
            PlantStatue.Activate(element == Element.Plant || element == Element.All);
        }

        // Activate Torii Gate
        if (ShrineGate != null && ShrineGate.isActiveAndEnabled)
        {
            ShrineGate.Activate(m_ShrineElement);
        }

        // Activate Japanese Lanterns
        if (JapaneseLanternLeft != null && JapaneseLanternLeft.isActiveAndEnabled)
        {
            JapaneseLanternLeft.Activate(m_ShrineElement);
        }

        if (JapaneseLanternRight != null && JapaneseLanternRight.isActiveAndEnabled)
        {
            JapaneseLanternRight.Activate(m_ShrineElement);
        }
    }

    void OpenTeleportMenu(FastTravelManager fastTravel)
    {
        // Place Player in middle of Tatami mat and make it face forwards
        Rigidbody rigidbody = m_Player.transform.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        // Sort Teleporter List
        fastTravel.AllTeleportersList.Sort((a, b) => (a.m_IsUnlocked.CompareTo(b.m_IsUnlocked)));
        fastTravel.AllTeleportersList.Sort((a, b) => (a.m_IsUnlocked.CompareTo(b.m_IsUnlocked)));
        fastTravel.AllTeleportersList.Reverse();

        // Open Teleport Menu
        m_Player.PlayerUI.OpenTeleportMenu();

        Debug.Log("TeleportMenu");
    }

    void OnValidate()
    {
        CreateSaveId();
    }

    //Creates a new save id if one isn't already created
    void CreateSaveId()
    {
        if (SaveId == "")
        {
            Guid guid = Guid.NewGuid();
            SaveId = guid.ToString();
        }

    }
}
