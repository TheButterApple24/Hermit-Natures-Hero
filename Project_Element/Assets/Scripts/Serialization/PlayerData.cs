/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PlayerData
Description:        Stores player data to be saved
Date Created:       21/10/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    21/10/2021
        - [Jeffrey] Created base PlayerData class
    27/01/2022
        - [Jeffrey] Added EXP variables

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // User Settings
    public float m_MasterVolume = 0.0f;
    public float m_MusicVolume = 0.0f;
    public float m_EffectsVolume = 0.0f;

    public float m_ResolutionWidth = 0.0f;
    public float m_ResolutionHeight = 0.0f;

    public bool m_IsFullscreen = false;

    public float m_Health;
    public float[] m_RespawnPosition;

    public int m_PlayerTotalEXP = 0;
    public int m_PlayerCurrentEXPForLevel = 0;
    public int m_PlayerEXPNeededForLevel = 0;
    public int m_PlayerLevel = 0;
    public int m_SkillPoints = 0;

    // Inventory 
    public int[] EquipmentIDs;
    public float[] EquipmentDamages;
    public float[] EquipmentDefenses;
    public int[] EquipmentRarity;
    public int[] EquipmentType;

    public int[] PotionAmounts;
    public int[] PotionIDs;

    public int[] ReagentAmounts;
    public int[] ReagentIDs;

    // Potion Hotbar
    public int Potion1ID = -1;
    public int Potion2ID = -1;

    // Lore Menu
    public bool[] LoreUnlockedIDs;

    // Teleporters
    public string[] UnlockedTeleportersIDs;

    // Quests
    public string[] AcceptedQuestIDs;
    public int[] TaskIndexs;
    public string[] CompletedQuestIDs;

    // Pets
    public string[] UnlockedPetIDs;
    public string[] EquippedPetIDs;

    // Gameplay
    public bool HasPlayedIntro = false;

    // Save Obejcts
    public bool[] DestroyObjectChecks;

    // Skill Tree
    public bool[] SkillButtonStates;
    public float MaxHealth;
    public float MaxStamina;
    public float MaxDefense;
    public int InventorySize;

    public PlayerData(Player player)
    {
        if (player.PlayerUI != null)
        {
            m_Health = player.m_HealthComp.m_CurrentHealth;

            m_RespawnPosition = new float[3];
            m_RespawnPosition[0] = player.m_RespawnPosition.x;
            m_RespawnPosition[1] = player.m_RespawnPosition.y;
            m_RespawnPosition[2] = player.m_RespawnPosition.z;

            m_PlayerTotalEXP = player.m_PlayerTotalEXP;
            m_PlayerCurrentEXPForLevel = player.m_PlayerCurrentEXPForLevel;
            m_PlayerEXPNeededForLevel = player.m_PlayerEXPNeededForLevel;
            m_PlayerLevel = player.m_PlayerLevel;
            m_SkillPoints = player.m_SkillPoints;

            SaveEquipment(player);
            SavePotions(player);
            SaveReagents(player);

            SavePotionHotbar(player);

            LoreUnlockedIDs = new bool[GameManager.Instance.m_LoreUnlockedIDs.Length];
            LoreUnlockedIDs = GameManager.Instance.m_LoreUnlockedIDs;

            UnlockedTeleportersIDs = new string[MenuManager.Instance.TeleportMenu.FastTravelManager.UnlockedTeleportersList.Count];
            for (int i = 0; i < UnlockedTeleportersIDs.Length; i++)
            {
                UnlockedTeleportersIDs[i] = MenuManager.Instance.TeleportMenu.FastTravelManager.UnlockedTeleportersList[i].SaveId;
            }

            SaveQuests(player);

            SavePets(player);

            HasPlayedIntro = IntroCutscene.Instance.HasBeenPlayed;

            DestroyObjectChecks = new bool[GameManager.Instance.SaveObjectList.Length];
            for (int i = 0; i < GameManager.Instance.SaveObjectList.Length; i++)
            {
                if (GameManager.Instance.SaveObjectList[i] == null)
                {
                    DestroyObjectChecks[i] = true;
                }
                else
                {
                    if (GameManager.Instance.SaveObjectList[i].activeSelf == false)
                    {
                        DestroyObjectChecks[i] = true;
                    }
                    else
                    {
                        DestroyObjectChecks[i] = false;
                    }
                }
            }

            SaveSkillTree();
        }
        else
        {
            // Create Empty Stuff
            m_Health = 20;
        }
    }

    void SaveEquipment(Player player)
    {
        EquipmentIDs = new int[player.m_Inventory.m_CurrentEquipment.Count];
        EquipmentDamages = new float[player.m_Inventory.m_CurrentEquipment.Count];
        EquipmentDefenses = new float[player.m_Inventory.m_CurrentEquipment.Count];
        EquipmentRarity = new int[player.m_Inventory.m_CurrentEquipment.Count];
        EquipmentType = new int[player.m_Inventory.m_CurrentEquipment.Count];

        for (int i = 0; i < EquipmentIDs.Length; i++)
        {
            if (player.m_Inventory.m_CurrentEquipment[i].ItemID != -1)
            {
                EquipmentIDs[i] = player.m_Inventory.m_CurrentEquipment[i].ItemID;
                EquipmentDamages[i] = player.m_Inventory.m_CurrentEquipment[i].ItemDamage;
                EquipmentDefenses[i] = player.m_Inventory.m_CurrentEquipment[i].ItemDefense;
                EquipmentRarity[i] = (int)player.m_Inventory.m_CurrentEquipment[i].ItemRarity;
                EquipmentType[i] = (int)player.m_Inventory.m_CurrentEquipment[i].ItemType;
            }
            else
            {
                EquipmentIDs[i] = -1;
            }
        }
    }

    void SavePotions(Player player)
    {

        PotionAmounts = new int[player.m_Inventory.m_CurrentPotions.Count];
        PotionIDs = new int[player.m_Inventory.m_CurrentPotions.Count];

        for (int i = 0; i < PotionAmounts.Length; i++)
        {
            if (player.m_Inventory.m_CurrentPotions[i].ItemAmount > 0)
            {
                PotionAmounts[i] = player.m_Inventory.m_CurrentPotions[i].ItemAmount;
                PotionIDs[i] = player.m_Inventory.m_CurrentPotions[i].ItemID;
            }
            else
            {
                PotionAmounts[i] = -1;
                PotionIDs[i] = -1;
            }

        }
    }

    void SaveReagents(Player player)
    {
        ReagentAmounts = new int[player.m_Inventory.m_CurrentReagents.Count];
        ReagentIDs = new int[player.m_Inventory.m_CurrentReagents.Count];

        for (int i = 0; i < ReagentAmounts.Length; i++)
        {
            if (player.m_Inventory.m_CurrentReagents[i].ItemAmount > 0)
            {
                ReagentAmounts[i] = player.m_Inventory.m_CurrentReagents[i].ItemAmount;
                ReagentIDs[i] = player.m_Inventory.m_CurrentReagents[i].ItemID;
            }
            else
            {
                ReagentAmounts[i] = -1;
                ReagentIDs[i] = -1;
            }
        }
    }

    void SavePotionHotbar(Player player)
    {
        if (HUDManager.Instance.PotionHotbarObject.GetPotion1() != null && HUDManager.Instance.PotionHotbarObject.GetPotion1Count() > 0)
            Potion1ID = HUDManager.Instance.PotionHotbarObject.GetPotion1().m_ItemID;
        else
            Potion1ID = -1;

        if (HUDManager.Instance.PotionHotbarObject.GetPotion2() != null && HUDManager.Instance.PotionHotbarObject.GetPotion2Count() > 0)
            Potion2ID = HUDManager.Instance.PotionHotbarObject.GetPotion2().m_ItemID;
        else
            Potion2ID = -1;
    }

    void SaveQuests(Player player)
    {
        AcceptedQuestIDs = new string[player.m_AcceptedQuests.Count];
        TaskIndexs = new int[player.m_AcceptedQuests.Count];
        CompletedQuestIDs = new string[player.CompletedQuestIDs.Count];

        for (int i = 0; i < AcceptedQuestIDs.Length; i++)
        {
            AcceptedQuestIDs[i] = player.m_AcceptedQuests[i].SaveId;
            TaskIndexs[i] = player.m_AcceptedQuests[i].m_TaskIndex;
        }

        for (int i = 0; i < CompletedQuestIDs.Length; i++)
        {
            CompletedQuestIDs[i] = player.CompletedQuestIDs[i];
        }


    }

    void SavePets(Player player)
    {
        UnlockedPetIDs = new string[6];
        PetManager petManager = GameObject.Find("PetManager").GetComponent<PetManager>();
        for (int i = 0; i < petManager.PetContainers.Length; i++)
        {
            if (petManager.Pets[i] != null)
            {
                if (!petManager.Pets[i].IsLocked)
                {
                    UnlockedPetIDs[i] = petManager.Pets[i].SaveId;
                }
            }
        }

        EquippedPetIDs = new string[2];

        if (player.m_PrimaryPet != null)
        {
            EquippedPetIDs[0] = player.m_PrimaryPet.SaveId;
        }

        if (player.m_SecondaryPet != null)
        {
            EquippedPetIDs[1] = player.m_SecondaryPet.SaveId;
        }
    }

    void SaveSkillTree()
    {
        SkillButtonStates = new bool[SkillTreeManager.Instance.SkillTreeButtons.Length];

        for (int i = 0; i < SkillButtonStates.Length; i++)
        {
            SkillButtonStates[i] = SkillTreeManager.Instance.SkillTreeButtons[i].SkillButton.interactable;
        }

        MaxHealth = PlayerManager.Instance.Player.m_HealthComp.MaxHealth;
        MaxStamina = PlayerManager.Instance.Player.m_StaminaComp.MaxStamina;
        MaxDefense = PlayerManager.Instance.Player.m_HealthComp.MaxDefense;
        InventorySize = PlayerManager.Instance.Player.m_Inventory.EquipmentUI.m_MaxSize;
    }
}
