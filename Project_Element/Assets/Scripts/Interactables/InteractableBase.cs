/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              InteractableBase
Description:        Base class for all interactable objects, both pickups and non-pickups
Date Created:       19/10/2021
Author:             Zoe Purcell
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/10/2021
        - [Zoe] Started InteractableBase.
    20/10/2021
        - [Zoe] Edited Debug Log Comment
    21/10/2021
        - [Zoe] Changed "Interact" to "Activate" and added a "Deactivate" function for objects that may need them.
        - [Zoe] InteractableBase now has both a Player and PlayerController reference.
    15/11/2021
        - [Zoe] Added functionality to enable and disable the button prompt to all interactable objects
    17/11/2021
        - [Zoe] Removed the GameObject.Find for the Player and PlayerController. Player and Player Controller need to be set in the editor now
    19/11/2021
        - [Zoe] Replaced tag checking with GameObject checking in OnTriggerEnter/Exit
	20/11/2021
        - [Jeffrey] Removed OnTriggerEnter and OnTriggerExit functions
    30/11/2021
        - [Zoe] Changes Start to Awake
    18/12/2021
        - [Zoe] Removed all trigger-related logic
    31/01/2022
        - [Zoe] Cleaned up unused code
        - [Zoe] Made Activate pure virtual. Kept Deactivate the way it is, since not every item utilizes
    02/02/2022
        - [Max] Removed m_IsInteractable
    15/02/2022
        - [Zoe] Made unneeded variables private, to make classes/prefabs easier to digest for artists
    17/02/2022
        - [Zoe] Added a gradient to handle the godlike tricolor effect
    22/02/2022
        - [Zoe] Added the bool RemoveFromListAfterTrigger, which will remove this item from the list of interactables. (Used for objects that get destroyed upon activation)

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public int m_ItemID = -1;
    public bool m_IsInteractable = true;
    public Transform m_ParticleLocation;
    protected bool m_RemoveFromListOnTrigger = false;
    public bool RemoveFromListOnTrigger { get { return m_RemoveFromListOnTrigger; } }

    [HideInInspector] public bool m_IsBeingHeld = false;
    [HideInInspector] public bool m_CanInteract = false;
    [HideInInspector] public ParticleSystem m_InteractionParticles;
    [HideInInspector] public Color[] m_ParticleLootColours;

    protected bool m_IsActivated;
    protected Player m_Player;
    protected ThirdPersonController m_PlayerController;
    public abstract void Activate();

    protected virtual void Awake()
    {
        // Assign the player and the player controller. If Player.Awake hasn't been called yet, this will be set again in Start
        if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
        {
            m_Player = PlayerManager.Instance.Player;
            m_PlayerController = m_Player.gameObject.GetComponent<ThirdPersonController>();
        }

        m_ParticleLootColours = new Color[] { };

        // If no transform / location is given, default to the object's centre position
        if (m_ParticleLocation == null)
        {
            m_ParticleLocation = gameObject.transform;
        }

        if (HUDManager.Instance != null)
        {
            m_InteractionParticles = HUDManager.Instance.InteractionParticles;
            m_ParticleLootColours = new Color[] { HUDManager.Instance.CommonRarityColor, HUDManager.Instance.RareRarityColor, HUDManager.Instance.HeroicRarityColor, HUDManager.Instance.MythicRarityColor };
        }
    }

    public virtual void Start()
    {
        // Assign the player and player controller, should it fail in awake
        if (m_Player == null)
        {
            m_Player = PlayerManager.Instance.Player;
            m_PlayerController = m_Player.gameObject.GetComponent<ThirdPersonController>();
        }
        if (m_InteractionParticles == null)
        {
            m_InteractionParticles = HUDManager.Instance.InteractionParticles;
        }
        if (m_ParticleLootColours.Length == 0)
        {
            m_ParticleLootColours = new Color[] { HUDManager.Instance.CommonRarityColor, HUDManager.Instance.RareRarityColor, HUDManager.Instance.HeroicRarityColor, HUDManager.Instance.MythicRarityColor };
        }
    }

    public virtual void Update()
    {
    }

    public virtual void Deactivate()
    {
    }

    public void ResetParticles()
    {
        if (m_CanInteract)
        {
            // Reset the playing
            m_InteractionParticles.Stop();
            m_InteractionParticles.Play();

            // Reset the colour to white (Ensures that weapon rarity colours don't carry over to other items
            ParticleSystem.MainModule settings = m_InteractionParticles.main;
            settings.startColor = HUDManager.Instance.CommonRarityColor;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = m_InteractionParticles.colorOverLifetime;
            colorOverLifetime.enabled = true;

            colorOverLifetime.color = HUDManager.Instance.CommonRarityColor;

            m_InteractionParticles.transform.position = m_ParticleLocation.transform.position;
        }
    }
}
