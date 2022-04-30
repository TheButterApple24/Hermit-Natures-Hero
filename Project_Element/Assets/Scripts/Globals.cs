/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              Globals
Description:        Handles global enums
Date Created:       05/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    05/10/2021
        - [Jeffrey] Added Element enum
    17/10/2021
        - [Max] Added ItemType, PlantType, PotionType enums.
    19/10/2021
        - [Max] Changed ItemType enum to PickupType enum.
    11/11/2021
        - [Max] Added Randomized Loot Tier enum.
    30/11/2021
        - [Max] Added Epic Loot Tier.
    03/12/2021
        - [Max] Removed 1 Loot Tier.
        - [Max] Renamed Purple Tier to Heroic Tier.
    14/01/2022
        - [Max] Added Element Type Matchup static class
	27/01/2022
        - [Max] Added LootTier Calculation
    30/01/2022
        - [Max] Added null check to LootTier Calculation + Added 5% chance of receiving Rare
    31/01/2021        
        - [Zoe] Fixed the order of Potions
        - [Zoe] Renamed reagent enum
    08/02/2022
        - [Max] Added Comments
    18/02/2022
        - [Max] Added Player Luck Modifier Bool to AssignLootRarity
    10/03/2022
        - [Max] Modified Player getter for singleton
        - [Max] Weapon class refactor

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public enum Element
    {
        Fire,
        Water,
        Plant,
        None,
        All,
        Undefined,
    }

    static class ElementTypeMatchup
    {
        public static float ElementalCheckMultiplier(Element objectElement, CharacterBase target)
        {
            // Check Object's Element Type
            switch (objectElement)
            {
                // Check Target's Element Type
                // Set Elemental Multiplier to Correct elemental typing
                case Element.Fire:
                    switch (target.m_Element)
                    {
                        case Element.Fire:
                            return 1.0f;
                        case Element.Water:
                            return 0.5f;
                        case Element.Plant:
                            return 1.5f;
                        case Element.None:
                            return 1.0f;
                        default:
                            return 1.0f;
                    }

                case Element.Water:
                    switch (target.m_Element)
                    {
                        case Element.Fire:
                            return 1.5f;
                        case Element.Water:
                            return 1.0f;
                        case Element.Plant:
                            return 0.5f;
                        case Element.None:
                            return 1.0f;
                        default:
                            return 1.0f;
                    }

                case Element.Plant:
                    switch (target.m_Element)
                    {
                        case Element.Fire:
                            return 0.5f;
                        case Element.Water:
                            return 1.5f;
                        case Element.Plant:
                            return 1.0f;
                        case Element.None:
                            return 1.0f;
                        default:
                            return 1.0f;
                    }

                case Element.None:
                    switch (target.m_Element)
                    {
                        case Element.Fire:
                            return 1.0f;
                        case Element.Water:
                            return 1.0f;
                        case Element.Plant:
                            return 1.0f;
                        case Element.None:
                            return 1.0f;
                        default:
                            return 1.0f;
                    }

                default:
                    return 1.0f;
            }
        }
    }
}


namespace PickupTypes
{
    public enum PickupType
    {
        Weapon,
        Reagent,
        Armor,
        Potion,
    }
}


namespace ReagentTypes
{
    public enum ReagentType
    {
        Nurseblossom,
        Sunpalm,
        BanditsGrace,
        GoldenSweetberry,
        Moonpalm,
        Knightsbane,
        Poisonwort,
        Witchberry
    }
}


namespace PotionTypes
{
    public enum PotionType
    {
        Health,
        Stamina,
        Attack,
        Defense,
        Poison,
    }
}

namespace LootTiers
{
    public enum LootTier
    {
        Common,
        Rare,
        Heroic,
        Mythic,
        Godlike,
        None,
    }

    static class LootTierCalculation
    {
        public static LootTier AssignLootRarity(bool isUsingLuck)
        {
            // Grab Player Object
            Player player = GameObject.Find("Player").GetComponent<Player>();

            // If Player exists
            if (player != null)
            {
                // Create Random Loot ID between 1 and 100. Adds Player's Luck Modifier to roll.
                int randomLootTierID = Random.Range(1, 101);
                    
                if (isUsingLuck)
                {
                    randomLootTierID += player.m_LuckModifier;
                }

                // Assign Loot Tier based on value of Loot ID. Clamp values if range is exceeded.
                if (randomLootTierID < 1)
                {
                    randomLootTierID = 1;
                    return LootTier.Common;
                }
                else if (randomLootTierID <= 55)
                {
                    return LootTier.Common;
                }
                else if (randomLootTierID > 55 && randomLootTierID <= 88)
                {
                    return LootTier.Rare;
                }
                else if (randomLootTierID > 88 && randomLootTierID <= 98)
                {
                    return LootTier.Heroic;
                }
                else if (randomLootTierID > 98 && randomLootTierID <= 100)
                {
                    return LootTier.Mythic;
                }
                else if (randomLootTierID > 100)
                {
                    randomLootTierID = 100;
                    return LootTier.Mythic;
                }
                else
                {
                    return LootTier.None;
                }
            }
            return LootTier.None;
        }
    }
}

namespace Movement
{
    public enum MovementState
    {
        OnGround,
        InAir,
        Sitting,
        Swimming,
        Climbing,
        Disable
    }
}