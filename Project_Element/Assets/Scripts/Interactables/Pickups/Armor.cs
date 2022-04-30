/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Armor
Description:        Handles armor item
Date Created:       17/10/2021
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    17/10/2021
        - [Max] Started Armor.
    18/10/2021
        - [Max] Set m_IsPickupable to false.
    19/10/2021
        - [Max] Integrated Pickups class (old Item class)
    30/10/2021
        - [Max] Added AddToInventory functionality (Missing Inventory System)
    07/11/2021
        - [Max] Implemented Inventory implementation
    08/11/2021
        - [Max] Called base.Activate. Removed m_IsPickupable
    12/11/2021
        - [Max] Included LootTiers for future implementation
    17/11/2021
        - [Zoe] Removed the base.Start, as well as Update since it was commented out
    26/01/2022
        - [Max] Refactored LootTier Calculation
    08/02/2022
        - [Max] Added Comments
    20/02/2022
        - [Zoe] Adjusted loot tier rolling so it only roles if the tier is None
        - [Zoe] Added functionality to change the interaction particles depending on rarity

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PickupTypes;
using LootTiers;

public class Armor : Pickups
{
    public float m_BaseDefense;
    public LootTier m_LootTier;
    public bool m_IsEquipped;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        // Set this type of pickup to Armor
        m_PickupType = PickupType.Armor;

        // Assigns random rarity to Weapon
        AssignLootRarity();

        // Assign random Base Defense stat based on rarity
        BaseDefenseTierCheck();
    }

    public override void Update()
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = m_InteractionParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        if (m_Player.InteractionHandler.TargetObject == this)
        {
            // If the weapon is Godlike, set the particle colour to the gradient of r/g/b
            if (m_LootTier == LootTier.Godlike)
            {
                colorOverLifetime.color = HUDManager.Instance.GodlikeColorGradient;
            }
            // If any other rarity, set the particle colour to the one that corresponds to its rarity
            else
            {
                colorOverLifetime.color = m_ParticleLootColours[(int)m_LootTier];
            }
        }
    }

    public override void Activate()
    {
        // If PlayerController exists
        if (m_PlayerController)
        {
            // If this or any other Armor not already equipped
            if (!m_IsEquipped)
            {
                // If Inventory is valid
                if (m_Player.m_Inventory != null)
                {
                    base.Activate();

                    // Add to Inventory
                    m_Player.m_Inventory.AddToInventory(this);
                }
            }
        }
    }

    public override void Deactivate()
    {
        // Call InteractableBase Activate()
        base.Deactivate();

        // If Player Controller exists
        if (m_PlayerController)
        {
            // If Armor is equipped
            if (m_IsEquipped)
            {
                // Remove Armor buff and unequip armor
                m_IsEquipped = false;
                UnEquip();
            }
        }
    }

    public void AssignLootRarity()
    {
        // If Weapon Loot Tier isn't Godlike (Dropped by Elemental Gods)
        if (m_LootTier == LootTier.None)
        {
            // Assign this armor's loot tier
            m_LootTier = LootTierCalculation.AssignLootRarity(true);
        }
    }

    private void BaseDefenseTierCheck()
    {
        // Assign Base Damage Range based on Weapon's Loot Tier. Godlike Weapons are assigned a set value.
        switch (m_LootTier)
        {
            case LootTier.Common:
                m_BaseDefense = (float)Random.Range(5, 8);
                break;

            case LootTier.Rare:
                m_BaseDefense = (float)Random.Range(8, 13);
                break;

            case LootTier.Heroic:
                m_BaseDefense = (float)Random.Range(13, 17);
                break;

            case LootTier.Mythic:
                m_BaseDefense = (float)Random.Range(17, 21);
                break;

            case LootTier.Godlike:
                m_BaseDefense = (float)25;
                break;

            case LootTier.None:
                m_BaseDefense = (float)0;
                break;

            default:
                break;
        }
    }

    public void Equip()
    {
        // If Player Object exists
        if (m_Player == null)
        {
            // Assign Player and PlayerController Objects
            m_Player = GameObject.Find("Player").GetComponent<Player>();
            m_PlayerController = GameObject.Find("Player").GetComponent<ThirdPersonController>();
        }

        // Hide gameobject
        //gameObject.SetActive(false);

        // Set armor to equipped
        m_IsEquipped = true;

        // Set main armor to this
        m_Player.m_MainArmor = this;

        // Apply damage resistance
        m_Player.m_HealthComp.m_DamageResistance += m_BaseDefense * 0.01f;
    }



    public void UnEquip()
    {
        // Instantiate armor GameObject
        //GameObject newArmor = GameObject.Instantiate(gameObject);

        //newArmor.GetComponent<Armor>().m_IsEquipped = false;
        //newArmor.GetComponent<Armor>().m_BaseDefense = m_BaseDefense;
        //newArmor.GetComponent<Armor>().m_LootTier = m_LootTier;
        //newArmor.transform.SetParent(null, false);
        //newArmor.transform.position = m_Player.transform.position + new Vector3(0.0f, 0.0f, 1.25f);

        // Enables object's BoxCollider and disables Rigidbody's Physics
        GetComponent<Rigidbody>().isKinematic = false;

        // Unparents the object from the player and lets them know they aren't holding an object anymore.
        transform.parent = null;

        // Show gameobject
        //gameObject.SetActive(true);

        // Remove Armor buff
        m_Player.m_HealthComp.m_DamageResistance -= m_BaseDefense * 0.01f;

        // Set armor to no longer equipped
        m_IsEquipped = false;


        // Tells the player that they are no longer holding an object
        m_Player.m_MainArmor = null;
    }
}
